using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.Account;
using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.InvoiceViewModels;
using Accounting.Models.Item;
using Accounting.Models.PaymentTermViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("i")]
  public class InvoiceController : BaseController
  {
    private readonly BusinessEntityService _businessEntityService;
    private readonly AddressService _addressService;
    private readonly PaymentTermsService _paymentTermsService;
    private readonly InvoiceService _invoiceService;
    private readonly InvoiceLineService _invoiceLineService;
    private readonly ItemService _itemService;
    private readonly JournalService _journalService;
    private readonly JournalInvoiceInvoiceLineService _journalInvoiceInvoiceLineService;
    private readonly InvoiceInvoiceLinePaymentService _invoicePaymentService;
    private readonly InvoiceAttachmentService _invoiceAttachmentService;
    private readonly OrganizationService _organizationService;
    private readonly AccountService _accountService;

    public InvoiceController(
      AddressService addressService,
      BusinessEntityService businessEntityService,
      JournalService journalLedgerService,
      InvoiceAttachmentService invoiceAttachmentService,
      InvoiceLineService invoiceLineService,
      InvoiceInvoiceLinePaymentService invoicePaymentService,
      InvoiceService invoiceService,
      ItemService itemService,
      OrganizationService organizationService,
      PaymentTermsService paymentTermsService,
      AccountService accountService,
      JournalInvoiceInvoiceLineService journalInvoiceInvoiceLineService,
      RequestContext requestContext)
    {
      _addressService = new AddressService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _businessEntityService = new BusinessEntityService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _journalService = new JournalService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _invoiceAttachmentService = new InvoiceAttachmentService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _invoiceLineService = new InvoiceLineService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _invoicePaymentService = new InvoiceInvoiceLinePaymentService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _journalInvoiceInvoiceLineService = new JournalInvoiceInvoiceLineService(_invoiceLineService, _journalService, requestContext.DatabaseName, requestContext.DatabasePassword);
      _invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService, requestContext.DatabaseName, requestContext.DatabasePassword);
      _itemService = new ItemService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _organizationService = new OrganizationService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _paymentTermsService = new PaymentTermsService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _accountService = new AccountService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [Route("invoices")]
    [HttpGet]
    public IActionResult Invoices(
      int page = 1,
      int pageSize = 2)
    {
      var referer = Request.Headers["Referer"].ToString() ?? string.Empty;

      var vm = new InvoicesPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize,
        RememberPageSize = string.IsNullOrEmpty(referer),
      };

      return View(vm);
    }

    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      var model = await InitializeCreateInvoiceViewModel();
      return View(model);
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceViewModel model)
    {
      model.OrganizationId = GetOrganizationId();

      model.SelectedPaymentTerm = JsonConvert.DeserializeObject<PaymentTermViewModel>(model.SelectedPaymentTermJSON!);
      model.InvoiceLines = JsonConvert.DeserializeObject<List<InvoiceLineViewModel>>(model.InvoiceLinesJson!);
      model.InvoiceAttachments = JsonConvert.DeserializeObject<List<CreateInvoiceViewModel.InvoiceAttachmentViewModel>>(model.InvoiceAttachmentsJSON!);

      if (model.SelectedBillingAddressId.HasValue)
      {
        model.SelectedBillingAddress = await GetSelectedAddress(model.SelectedBillingAddressId.Value);
      }

      if (model.SelectedShippingAddressId.HasValue)
      {
        model.SelectedShippingAddress = await GetSelectedAddress(model.SelectedShippingAddressId.Value);
      }

      var validationResult = await ValidateCreateInvoiceModel(model);


      if (!validationResult.IsValid)
      {
        var errorModel = await InitializeErrorCreateInvoiceViewModel(model, validationResult);
        return View(errorModel);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        Invoice invoice = await CreateInvoiceAttachmentsAndLedgerEntries(model);
        await _invoiceService.UpdatePaymentInstructions(invoice.InvoiceID, model.PaymentInstructions, GetOrganizationId());

        if (model.RememberPaymentInstructions)
        {
          await _organizationService.UpdatePaymentInstructions(model.OrganizationId, model.PaymentInstructions);
        }

        scope.Complete();
      }

      return RedirectToAction("Invoices");
    }

    private async Task<CreateInvoiceViewModel> InitializeCreateInvoiceViewModel()
    {
      int orgId = GetOrganizationId();

      var paymentTerms = await GetAllPaymentTerms();
      var defaultPaymentTerm = paymentTerms.FirstOrDefault();

      var model = new CreateInvoiceViewModel
      {
        Customers = await GetAllCustomersWithAddresses(),
        PaymentTerms = paymentTerms,
        SelectedPaymentTerm = defaultPaymentTerm,
        InvoiceDate = DateTime.UtcNow,
        InvoiceStatuses = Invoice.InvoiceStatusConstants.All.ToList(),
        ProductsAndServices = await GetAllProductsAndServices(orgId),
        PaymentInstructions = await _organizationService.GetPaymentInstructions(orgId)
      };

      await SetupAccrualAccounting(model, orgId);

      return model;
    }

    private async Task<ValidationResult> ValidateCreateInvoiceModel(CreateInvoiceViewModel model)
    {
      var validator = new CreateInvoiceViewModel.CreateInvoiceViewModelValidator();

      return await validator.ValidateAsync(model);
    }

    private async Task<CreateInvoiceViewModel> InitializeErrorCreateInvoiceViewModel(CreateInvoiceViewModel model, ValidationResult validationResult)
    {
      model.Customers = await GetAllCustomersWithAddresses();
      model.PaymentTerms = await GetAllPaymentTerms();
      model.InvoiceStatuses = Invoice.InvoiceStatusConstants.All.ToList();
      model.ProductsAndServices = await GetAllProductsAndServices(GetOrganizationId());
      model.ValidationResult = validationResult;

      var selectedCustomer = model.Customers.FirstOrDefault(c => c.ID == model.SelectedCustomerId);
      if (selectedCustomer != null)
      {
        model.SelectedCustomer = selectedCustomer;
      }

      await SetupAccrualAccounting(model, GetOrganizationId());

      return model;
    }

    private async Task<Invoice> CreateInvoiceAttachmentsAndLedgerEntries(CreateInvoiceViewModel model)
    {
      Invoice invoice = await _invoiceService.CreateAsync(new Invoice()
      {
        BusinessEntityId = model.SelectedCustomerId!.Value,
        BillingAddressJSON = JsonConvert.SerializeObject(model.SelectedBillingAddress),
        ShippingAddressJSON = JsonConvert.SerializeObject(model.SelectedShippingAddress),
        DueDate = model.DueDate,
        PaymentInstructions = model.PaymentInstructions,
        TotalAmount = model.InvoiceLines!.Sum(x => x.Price * x.Quantity)!.Value,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId(),
      });

      if (model.RememberPaymentInstructions)
      {
        await _organizationService.UpdatePaymentInstructions(model.OrganizationId, model.PaymentInstructions);
      }

      List<InvoiceAttachment> invoiceAttachments = await _invoiceAttachmentService.GetAllAsync(model.InvoiceAttachments.Select(x => x.InvoiceAttachmentID).ToArray(), GetOrganizationId());

      foreach (var invoiceAttachment in invoiceAttachments)
      {
        await _invoiceAttachmentService.UpdateInvoiceIdAsync(invoiceAttachment.InvoiceAttachmentID, invoice.InvoiceID, GetOrganizationId());
        await _invoiceAttachmentService.MoveAndUpdateInvoiceAttachmentPathAsync(invoiceAttachment, ConfigurationSingleton.Instance.PermPath, GetOrganizationId());

        await MoveFileFromTempToPermDirectory(invoiceAttachment);
      }

      foreach (var invoiceLine in model.InvoiceLines)
      {
        InvoiceLine newInvoiceLine = await _invoiceLineService.CreateAsync(new InvoiceLine()
        {
          Title = invoiceLine.Title,
          Description = invoiceLine.Description,
          Quantity = invoiceLine.Quantity,
          Price = invoiceLine.Price,
          InvoiceId = invoice.InvoiceID,
          CreatedById = GetUserId(),
          RevenueAccountId = invoiceLine.RevenueAccountId,
          AssetsAccountId = invoiceLine.AssetsAccountId,
          OrganizationId = GetOrganizationId(),
        });

        invoiceLine.ID = newInvoiceLine.InvoiceLineID;
      }

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (var invoiceLine in model.InvoiceLines!)
      {
        Journal debitGlEntry = await _journalService.CreateAsync(new Journal()
        {
          AccountId = invoiceLine.AssetsAccountId!.Value,
          Debit = invoiceLine.Price * invoiceLine.Quantity,
          Credit = null,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId(),
        });

        Journal creditGlEntry = await _journalService.CreateAsync(new Journal()
        {
          AccountId = invoiceLine.RevenueAccountId!.Value,
          Debit = null,
          Credit = invoiceLine.Price * invoiceLine.Quantity,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId(),
        });

        await _journalInvoiceInvoiceLineService.CreateAsync(new JournalInvoiceInvoiceLine()
        {
          JournalId = creditGlEntry.JournalID,
          InvoiceId = invoice.InvoiceID,
          InvoiceLineId = invoiceLine.ID,
          TransactionGuid = transactionGuid,
          OrganizationId = GetOrganizationId(),
          CreatedById = GetUserId(),
        });

        await _journalInvoiceInvoiceLineService.CreateAsync(new JournalInvoiceInvoiceLine()
        {
          JournalId = debitGlEntry.JournalID,
          InvoiceId = invoice.InvoiceID,
          InvoiceLineId = invoiceLine.ID,
          TransactionGuid = transactionGuid,
          OrganizationId = GetOrganizationId(),
          CreatedById = GetUserId(),
        });
      }

      return invoice;
    }

    private async Task CreateLedgerEntries(List<InvoiceLine> invoiceLines, Guid transactionGuid)
    {
      foreach (var line in invoiceLines)
      {
        Journal debitGlEntry = await _journalService.CreateAsync(new Journal()
        {
          AccountId = line.AssetsAccountId!.Value,
          Debit = line.Price * line.Quantity,
          Credit = null,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId(),
        });

        await _journalInvoiceInvoiceLineService.CreateAsync(new JournalInvoiceInvoiceLine()
        {
          JournalId = debitGlEntry.JournalID,
          InvoiceLineId = line.InvoiceLineID,
          TransactionGuid = transactionGuid,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId(),
        });

        Journal creditGlEntry = await _journalService.CreateAsync(new Journal()
        {
          AccountId = line.RevenueAccountId!.Value,
          Debit = null,
          Credit = line.Price * line.Quantity,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId(),
        });

        await _journalInvoiceInvoiceLineService.CreateAsync(new JournalInvoiceInvoiceLine()
        {
          JournalId = creditGlEntry.JournalID,
          InvoiceLineId = line.InvoiceLineID,
          TransactionGuid = transactionGuid,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId(),
        });
      }
    }

    private async Task MoveFileFromTempToPermDirectory(InvoiceAttachment invoiceAttachment)
    {
      string tempFilePath = Path.Combine(ConfigurationSingleton.Instance.TempPath, invoiceAttachment.OriginalFileName);
      string permFilePath = Path.Combine(ConfigurationSingleton.Instance.PermPath, invoiceAttachment.OriginalFileName);

      if (System.IO.File.Exists(tempFilePath))
      {
        await Task.Run(() => System.IO.File.Move(tempFilePath, permFilePath));
      }
    }

    [Route("update/{id}")]
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
      Invoice invoice = await _invoiceService.GetAsync(id, GetOrganizationId());

      if (invoice == null)
      {
        return NotFound();
      }

      if (!string.IsNullOrWhiteSpace(invoice.VoidReason))
      {
        return RedirectToAction("InvoiceIsVoid");
      }

      invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      invoice.BusinessEntity.Addresses = await _addressService.GetByAsync(invoice.BusinessEntityId);
      invoice.InvoiceLines = await _journalInvoiceInvoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);

      var model = new UpdateInvoiceViewModel
      {
        PaymentTerms = await GetAllPaymentTerms(),
        InvoiceDate = invoice.Created,
        DueDate = invoice.DueDate,
        LastUpdated = invoice.LastUpdated,
        InvoiceStatuses = Invoice.InvoiceStatusConstants.All.ToList(),
        ProductsAndServices = await GetAllProductsAndServices(GetOrganizationId()),
        Attachments = (await _invoiceAttachmentService.GetAllAsync(invoice.InvoiceID, GetOrganizationId()))
          .Select(a => new UpdateInvoiceViewModel.InvoiceAttachmentViewModel
          {
            InvoiceAttachmentID = a.InvoiceAttachmentID,
            FileName = a.OriginalFileName
          }).ToList(),
        Customer = new BusinessEntityViewModel()
        {
          ID = invoice.BusinessEntity.BusinessEntityID,
          FirstName = invoice.BusinessEntity.FirstName,
          LastName = invoice.BusinessEntity.LastName,
          CompanyName = invoice.BusinessEntity.CompanyName,
          Website = invoice.BusinessEntity.Website,
          PaymentTermId = invoice.BusinessEntity.PaymentTermId,
          CreatedById = invoice.BusinessEntity.CreatedById,
          Created = invoice.BusinessEntity.Created,
          Addresses = invoice!.BusinessEntity!.Addresses!.Select(x => new AddressViewModel()
          {
            ID = x.AddressID.ToString(),
            ExtraAboveAddress = x.ExtraAboveAddress,
            AddressLine1 = x.AddressLine1,
            AddressLine2 = x.AddressLine2,
            ExtraBelowAddress = x.ExtraBelowAddress,
            City = x.City,
            StateProvince = x.StateProvince,
            PostalCode = x.PostalCode,
            Country = x.Country,
          }).ToList(),
        },
        ExistingInvoiceLines = invoice.InvoiceLines!.Select(x => new InvoiceLineViewModel()
        {
          ID = x.InvoiceLineID,
          Title = x.Title,
          Description = x.Description,
          Quantity = x.Quantity,
          Price = x.Price,
          RevenueAccountId = x.RevenueAccountId,
          AssetsAccountId = x.AssetsAccountId,
        }).ToList(),
        BillingAddress = JsonConvert.DeserializeObject<AddressViewModel>(invoice.BillingAddressJSON),
        ShippingAddress = JsonConvert.DeserializeObject<AddressViewModel>(invoice.ShippingAddressJSON),
      };

      return View(model);
    }

    [Route("update/{id}")]
    [HttpPost]
    public async Task<IActionResult> Update(UpdateInvoiceViewModel model, int id)
    {
      Invoice invoice = await _invoiceService.GetAsync(id, GetOrganizationId());
      if (invoice == null)
      {
        return NotFound();
      }

      invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      invoice.BusinessEntity.Addresses = await _addressService.GetByAsync(invoice.BusinessEntityId);

      model.InvoiceNumber = invoice.InvoiceNumber;
      model.PaymentTerms = await GetAllPaymentTerms();
      model.InvoiceDate = invoice.Created;
      model.DueDate = invoice.DueDate;
      model.InvoiceStatuses = Invoice.InvoiceStatusConstants.All.ToList();
      model.ProductsAndServices = await GetAllProductsAndServices(GetOrganizationId());
      model.Customer = new BusinessEntityViewModel()
      {
        ID = invoice.BusinessEntity.BusinessEntityID,
        FirstName = invoice.BusinessEntity.FirstName,
        LastName = invoice.BusinessEntity.LastName,
        CompanyName = invoice.BusinessEntity.CompanyName,
        PaymentTermId = invoice.BusinessEntity.PaymentTermId,
        CreatedById = invoice.BusinessEntity.CreatedById,
        Created = invoice.BusinessEntity.Created,
        Addresses = invoice!.BusinessEntity!.Addresses!.Select(x => new AddressViewModel()
        {
          ID = x.AddressID.ToString(),
          AddressLine1 = x.AddressLine1,
          AddressLine2 = x.AddressLine2,
          City = x.City,
          StateProvince = x.StateProvince,
          PostalCode = x.PostalCode,
          Country = x.Country,
        }).ToList(),
      };
      model.BillingAddress = await GetSelectedAddress(model.SelectedAddressId);
      model.ExistingInvoiceLines = JsonConvert.DeserializeObject<List<InvoiceLineViewModel>>(model.InvoiceLinesJson!)!.Where(x => x.ID > 0).ToList();
      model.NewInvoiceLines = JsonConvert.DeserializeObject<List<InvoiceLineViewModel>>(model.InvoiceLinesJson!)!.Where(x => x.ID < 0).ToList();
      model.DeletedInvoiceLines = JsonConvert.DeserializeObject<List<InvoiceLineViewModel>>(model.DeletedInvoiceLinesJson!);

      UpdateInvoiceViewModelValidator validator = new UpdateInvoiceViewModelValidator(_invoiceService, GetOrganizationId());
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        model.ExistingInvoiceLines.AddRange(model.NewInvoiceLines);
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        if (!string.IsNullOrEmpty(model.DeletedAttachmentIdsCsv))
        {
          var ids = model.DeletedAttachmentIdsCsv
              .Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(id => int.Parse(id.Trim()))
              .ToList();
          await _invoiceAttachmentService.DeleteAttachmentsAsync(ids, invoice.InvoiceID, GetOrganizationId());
        }

        if (!string.IsNullOrEmpty(model.NewAttachmentIdsCsv))
        {
          var newAttachmentIds = model.NewAttachmentIdsCsv
              .Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(id => int.Parse(id.Trim()))
              .ToList();

          var newAttachments = await _invoiceAttachmentService.GetAllAsync(newAttachmentIds.ToArray(), GetOrganizationId());

          foreach (var attachment in newAttachments)
          {
            await _invoiceAttachmentService.UpdateInvoiceIdAsync(attachment.InvoiceAttachmentID, invoice.InvoiceID, GetOrganizationId());
            //Optionally move file or update path if needed
            await _invoiceAttachmentService.MoveAndUpdateInvoiceAttachmentPathAsync(attachment, ConfigurationSingleton.Instance.PermPath, GetOrganizationId());
          }
        }

        List<InvoiceLine> existingLines = model.ExistingInvoiceLines!.Select(x => new InvoiceLine()
        {
          InvoiceLineID = x.ID,
          Title = x.Title,
          Description = x.Description,
          Quantity = x.Quantity,
          Price = x.Price,
          TitleOrDescriptionModified = x.TitleOrDescriptionModified,
          QuantityOrPriceModified = x.QuantityOrPriceModified,
        }).ToList();

        await _invoiceLineService.UpdateTitleAndDescription(existingLines.Where(x => x.TitleOrDescriptionModified).ToList(), invoice.InvoiceID, GetUserId(), GetOrganizationId());

        if (model.NewInvoiceLines != null && model.NewInvoiceLines.Any())
        {
          foreach (var invoiceLine in model.NewInvoiceLines)
          {
            InvoiceLine newInvoiceLine = await _invoiceLineService.CreateAsync(new InvoiceLine()
            {
              Title = invoiceLine.Title,
              Description = invoiceLine.Description,
              Quantity = invoiceLine.Quantity,
              Price = invoiceLine.Price,
              InvoiceId = invoice.InvoiceID,
              CreatedById = GetUserId(),
              RevenueAccountId = invoiceLine.RevenueAccountId,
              AssetsAccountId = invoiceLine.AssetsAccountId,
              OrganizationId = GetOrganizationId(),
            });

            invoiceLine.ID = newInvoiceLine.InvoiceLineID;
          }
        }

        List<InvoiceLine> newLines = model.NewInvoiceLines!.Select(x => new InvoiceLine()
        {
          InvoiceLineID = x.ID,
          Title = x.Title,
          Description = x.Description,
          Quantity = x.Quantity,
          Price = x.Price,
          RevenueAccountId = x.RevenueAccountId,
          AssetsAccountId = x.AssetsAccountId,
        }).ToList();

        List<InvoiceLine> deletedLines = model.DeletedInvoiceLines!.Select(x => new InvoiceLine()
        {
          InvoiceLineID = x.ID,
          Title = x.Title,
          Description = x.Description,
          Quantity = x.Quantity,
          Price = x.Price,
          RevenueAccountId = x.RevenueAccountId,
          AssetsAccountId = x.AssetsAccountId,
        }).ToList();

        await _journalInvoiceInvoiceLineService
          .UpdateInvoiceLinesAsync(
            existingLines.Where(x => x.QuantityOrPriceModified).ToList(),
            newLines,
            deletedLines,
            invoice,
            GetUserId(),
            GetOrganizationId());

        await _invoiceService
          .ComputeAndUpdateInvoiceStatus(
            invoice.InvoiceID,
            GetOrganizationId());
        await _invoiceService
          .ComputeAndUpdateTotalAmountAndReceivedAmount(
            invoice.InvoiceID,
            GetOrganizationId());
        await _invoiceService
          .UpdateLastUpdated(
            invoice.InvoiceID,
            GetOrganizationId());

        scope.Complete();
      }

      return RedirectToAction("Invoices");
    }

    [HttpGet]
    [Route("void/{id}")]
    public async Task<IActionResult> Void(int id)
    {
      Invoice invoice = await _invoiceService.GetAsync(id, GetOrganizationId());


      if (invoice == null)
      {
        return NotFound();
      }

      VoidInvoiceViewModel voidInvoiceViewModel = new VoidInvoiceViewModel()
      {
        InvoiceID = invoice.InvoiceID,
        InvoiceNumber = invoice.InvoiceNumber,
      };

      return View(voidInvoiceViewModel);
    }

    [HttpPost]
    [Route("void/{id}")]
    public async Task<IActionResult> Void(VoidInvoiceViewModel model)
    {
      Invoice invoice = await _invoiceService.GetAsync(model.InvoiceID, GetOrganizationId());

      if (invoice == null)
      {
        return NotFound();
      }

      VoidInvoiceViewModelValidator validationRules = new VoidInvoiceViewModelValidator(_invoiceService, GetOrganizationId(), _invoicePaymentService);
      ValidationResult validationResult = await validationRules.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _invoiceService.VoidAsync(invoice, model.VoidReason, GetUserId(), GetOrganizationId());

        scope.Complete();
      }

      return RedirectToAction("Invoices");
    }

    private async Task<AddressViewModel?> GetSelectedAddress(int? selectedAddressId)
    {
      if (selectedAddressId == null || selectedAddressId <= 0)
      {
        return null;
      }

      Address selectedAddress = await _addressService.GetAsync(selectedAddressId.Value);

      if (selectedAddress == null)
      {
        return null;
      }

      return new AddressViewModel()
      {
        ID = selectedAddress.AddressID.ToString(),
        ExtraAboveAddress = selectedAddress.ExtraAboveAddress,
        AddressLine1 = selectedAddress.AddressLine1,
        AddressLine2 = selectedAddress.AddressLine2,
        ExtraBelowAddress = selectedAddress.ExtraBelowAddress,
        City = selectedAddress.City,
        StateProvince = selectedAddress.StateProvince,
        PostalCode = selectedAddress.PostalCode,
        Country = selectedAddress.Country
      };
    }

    private async Task<List<BusinessEntityViewModel>> GetAllCustomersWithAddresses()
    {
      var customers = await _businessEntityService.GetAllAsync(GetOrganizationId());

      foreach (var customer in customers)
      {
        customer.Addresses = await _addressService.GetByAsync(customer.BusinessEntityID);
      }

      return customers.Select(c => new BusinessEntityViewModel
      {
        ID = c.BusinessEntityID,
        FirstName = c.FirstName,
        LastName = c.LastName,
        CompanyName = c.CompanyName,
        Website = c.Website,
        CustomerType = c.CustomerType,
        PaymentTermId = c.PaymentTermId,
        CreatedById = c.CreatedById,
        Created = c.Created,
        Addresses = c.Addresses?.Select(a => new AddressViewModel
        {
          ID = a.AddressID.ToString(),
          ExtraAboveAddress = a.ExtraAboveAddress,
          AddressLine1 = a.AddressLine1,
          AddressLine2 = a.AddressLine2,
          ExtraBelowAddress = a.ExtraBelowAddress,
          City = a.City,
          StateProvince = a.StateProvince,
          PostalCode = a.PostalCode,
          Country = a.Country
        }).ToList()
      }).ToList();
    }

    private async Task<List<PaymentTermViewModel>> GetAllPaymentTerms()
    {
      var paymentTerms = await _paymentTermsService.GetAllAsync();

      return paymentTerms.Select(pt => new PaymentTermViewModel
      {
        ID = pt.PaymentTermID,
        Description = pt.Description,
        DaysUntilDue = pt.DaysUntilDue
      }).ToList();
    }

    private async Task<List<ItemViewModel>> GetAllProductsAndServices(int organizationId)
    {
      List<Item> productsAndServices = await _itemService.GetAllAsync(organizationId);

      return productsAndServices.Select(x => new ItemViewModel()
      {
        ItemID = x.ItemID,
        Name = x.Name,
        Description = x.Description,
        RevenueAccountId = x.RevenueAccountId,
        AssetsAccountId = x.AssetsAccountId,
        Quantity = x.Quantity,
        SellFor = x.SellFor,
      }).ToList();
    }

    private async Task SetupAccrualAccounting(CreateInvoiceViewModel model, int organizationId)
    {
      var creditAccounts = await _accountService.GetAccountOptionsForInvoiceCreationCredit(organizationId);
      var debitAccounts = await _accountService.GetAccountOptionsForInvoiceCreationDebit(organizationId);

      model.CreditAccounts = creditAccounts.Select(x => new AccountViewModel
      {
        AccountID = x.AccountID,
        Name = x.Name,
        Type = x.Type,
      }).ToList();

      model.DebitAccounts = debitAccounts.Select(x => new AccountViewModel
      {
        AccountID = x.AccountID,
        Name = x.Name,
        Type = x.Type,
      }).ToList();
    }
  }
}