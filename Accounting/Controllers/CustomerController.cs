using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.PaymentTermViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Transactions;
using static Accounting.Business.BusinessEntity;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("c")]
  public class CustomerController : BaseController
  {
    private readonly BusinessEntityService _customerService;
    private readonly AddressService _addressService;
    private readonly PaymentTermsService _paymentTermsService;

    public CustomerController(BusinessEntityService businessEntityService)
    {
      _customerService = businessEntityService;
      _addressService = new AddressService(GetDatabaseName());
      _paymentTermsService = new PaymentTermsService(GetDatabaseName());
    }

    [Route("customers")]
    [HttpGet]
    public IActionResult Customers(int page = 1, int pageSize = 10)
    {
      BusinessEntitiesViewModel model = new BusinessEntitiesViewModel();
      model.Page = page;
      model.PageSize = pageSize;

      return View(model);
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
      CreateBusinessEntityViewModel model = new CreateBusinessEntityViewModel();
      await InitializeCreateViewModelAsync(model);

      return View(model);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(CreateBusinessEntityViewModel model)
    {
      model.Addresses = DeserializeAddresses(model.AddressesJson);

      CreateCustomerViewModelValidator validator = new CreateCustomerViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        await InitializeCreateViewModelAsync(model);
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await CreateCustomerAndAddressesAsync(model);
        scope.Complete();
      }

      return RedirectToAction("Customers");
    }

    [HttpGet]
    [Route("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
      BusinessEntity customer = await _customerService.GetAsync(id, GetOrganizationId());

      return View(customer);
    }

    [HttpGet]
    [Route("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
      BusinessEntity businessEntity = await _customerService.GetAsync(id, GetOrganizationId());
      businessEntity.Addresses = await _addressService.GetByAsync(id);
      businessEntity.PaymentTerm = await _paymentTermsService.GetAsync(businessEntity.PaymentTermId);

      var model = new EditBusinessEntityViewModel()
      {
        FirstName = businessEntity.FirstName,
        LastName = businessEntity.LastName,
        CompanyName = businessEntity.CompanyName,
        CustomerTypes = CustomerTypeConstants.All.ToList(),
        SelectedCustomerType = businessEntity.CustomerType,
        Addresses = businessEntity.Addresses != null
              && businessEntity.Addresses.Count > 0
              ? businessEntity.Addresses.Select(a => new AddressViewModel()
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
              }).ToList() : null,
        AvailablePaymentTerms = await GetPaymentTermsAsync(),
        PaymentTerm = businessEntity.PaymentTerm != null ? new PaymentTermViewModel()
        {
          ID = businessEntity.PaymentTerm.PaymentTermID,
          Description = businessEntity.PaymentTerm.Description,
          DaysUntilDue = businessEntity.PaymentTerm.DaysUntilDue
        } : null
      };
      InitializeEditViewModelAsync(model, businessEntity);

      return View(model);
    }

    [HttpPost]
    [Route("edit/{id}")]
    public async Task<IActionResult> Edit(EditBusinessEntityViewModel model)
    {
      BusinessEntity businessEntity = await _customerService.GetAsync(model.ID, GetOrganizationId());

      model.Addresses = DeserializeAddresses(model.AddressesJson);

      EditCustomerViewModelValidator validator = new EditCustomerViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.CustomerTypes = CustomerTypeConstants.All.ToList();
        model.AvailablePaymentTerms = await GetPaymentTermsAsync();
        model.PaymentTerm = model.AvailablePaymentTerms.SingleOrDefault(x => x.ID == model.SelectedPaymentTermId);
        InitializeEditViewModelAsync(model, businessEntity);

        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _customerService.UpdateAsync(model.ID, model.FirstName, model.LastName, model.CompanyName, model.SelectedCustomerType, model.SelectedBusinessEntityTypesCsv);
        await _addressService.DeleteAsync(model.ID);
        foreach (var address in model.Addresses!)
        {
          await _addressService.CreateAsync(new Address()
          {
            ExtraAboveAddress = address.ExtraAboveAddress,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            ExtraBelowAddress = address.ExtraBelowAddress,
            City = address.City,
            StateProvince = address.StateProvince,
            PostalCode = address.PostalCode,
            Country = address.Country,
            BusinessEntityId = model.ID,
            OrganizationId = GetOrganizationId(),
            CreatedById = GetUserId()
          });
        }

        scope.Complete();
      }

      return RedirectToAction("Customers");
    }

    private async Task CreateCustomerAndAddressesAsync(CreateBusinessEntityViewModel model)
    {
      BusinessEntity customer = await _customerService.CreateAsync(new BusinessEntity
      {
        CustomerType = model.SelectedCustomerType,
        BusinessEntityTypesCsv = model.SelectedBusinessEntityTypesCsv,
        FirstName = model.FirstName,
        LastName = model.LastName,
        CompanyName = model.CompanyName,
        PaymentTermId = model.SelectedPaymentTermId,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId()
      });

      foreach (var addressViewModel in model.Addresses)
      {
        await _addressService.CreateAsync(new Address
        {
          AddressLine1 = addressViewModel.AddressLine1,
          AddressLine2 = addressViewModel.AddressLine2,
          City = addressViewModel.City,
          StateProvince = addressViewModel.StateProvince,
          PostalCode = addressViewModel.PostalCode,
          Country = addressViewModel.Country,
          BusinessEntityId = customer.BusinessEntityID,
          OrganizationId = GetOrganizationId(),
          CreatedById = GetUserId(),
        });
      }
    }

    private async Task InitializeCreateViewModelAsync(CreateBusinessEntityViewModel model)
    {
      model.CustomerTypes = CustomerTypeConstants.All.ToList();
      model.AvailableBusinessEntityTypesCsv = string.Join(",", BusinessEntityTypeConstants.All);
      model.AvailablePaymentTerms = await GetPaymentTermsAsync();
    }

    private void InitializeEditViewModelAsync(EditBusinessEntityViewModel model, BusinessEntity entity)
    {
      model.AvailableBusinessEntityTypesCsv = string.Join(",", BusinessEntityTypeConstants.All);
      model.SelectedBusinessEntityTypesCsv =
          (HttpContext.Request.Method == "POST")
          ? model.SelectedBusinessEntityTypesCsv : entity.BusinessEntityTypesCsv;
    }

    private async Task<List<PaymentTermViewModel>> GetPaymentTermsAsync()
    {
      var paymentTerms = await _paymentTermsService.GetAllAsync();

      return paymentTerms.Select(pt => new PaymentTermViewModel
      {
        ID = pt.PaymentTermID,
        Description = pt.Description,
        DaysUntilDue = pt.DaysUntilDue
      }).ToList();
    }

    private List<AddressViewModel>? DeserializeAddresses(string? addressesJson)
    {
      return string.IsNullOrEmpty(addressesJson)
          ? new List<AddressViewModel>()
          : JsonConvert.DeserializeObject<List<AddressViewModel>>(addressesJson);
    }
  }
}