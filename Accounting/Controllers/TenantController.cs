using Microsoft.AspNetCore.Mvc;
using Accounting.Service;
using FluentValidation.Results;
using System.Transactions;
using Accounting.Business;
using FluentValidation;
using Accounting.Models.Tenant;
using Accounting.CustomAttributes;
using Accounting.Validators.Tenant;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("tenant")]
  public class TenantController : BaseController
  {
    private readonly TenantService _tenantService;
    private readonly CloudServices _cloudServices;
    private readonly SecretService _secretService;

    public TenantController(
      TenantService tenantService,
      CloudServices cloudServices,
      SecretService secretService)
    {
      _tenantService = tenantService;
      _cloudServices = cloudServices;
      _secretService = secretService;
    }

    [Route("tenants")]
    [HttpGet]
    public IActionResult Tenants(
      int page = 1,
      int pageSize = 2)
    {
      var vm = new TenantsPaginatedViewModel()
      {
        Page = page,
        PageSize = pageSize,
      };

      return View(vm);
    }

    [Route("provision-tenant")]
    [HttpGet]
    public IActionResult ProvisionTenant()
    {
      ProvisionTenantViewModel model = new ProvisionTenantViewModel();
      model.Shared = true;

      return View(model);
    }

    [Route("provision-tenant")]
    [HttpPost]
    public async Task<IActionResult> ProvisionTenant(
      ProvisionTenantViewModel model)
    {
      model.OrganizationId = GetOrganizationId();

      ProvisionTenantViewModelValidator validator
        = new ProvisionTenantViewModelValidator(_tenantService, _secretService);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        Tenant tenant;

        tenant = await _tenantService.CreateAsync(new Tenant()
        {
          Email = model.Email,
          FullyQualifiedDomainName = model.FullyQualifiedDomainName,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId()
        });

        await _cloudServices.GetDigitalOceanService(
          _secretService,
          _tenantService,
          GetOrganizationId()).CreateDropletAsync(tenant);

        scope.Complete();
      }

      return RedirectToAction("Tenants");
    }
  }

  [AuthorizeWithOrganizationId]
  [Route("api/tenant")]
  [ApiController]
  public class TenantApiController : BaseController
  {
    private readonly TenantService _tenantService;

    public TenantApiController(TenantService tenantService)
    {
      _tenantService = tenantService;
    }

    [HttpGet("get-all-tenants")]
    public async Task<IActionResult> GetAllTenants(
      int page = 1,
      int pageSize = 2)
    {
      (List<Tenant> tenants, int? nextPage) =
        await _tenantService.GetAllAsync(
          page,
          pageSize,
          GetOrganizationId());

      TenantViewModel ConvertToViewModel(Tenant tenant)
      {
        return new TenantViewModel
        {
          TenantID = tenant.TenantID,
          FullyQualifiedDomainName = tenant.FullyQualifiedDomainName,
          Email = tenant.Email,
          DropletId = tenant.DropletId,
          Ipv4 = tenant.Ipv4,
          SshPublic = !string.IsNullOrEmpty(tenant.SshPublic),
          SshPrivate = !string.IsNullOrEmpty(tenant.SshPrivate),
          Created = tenant.Created
        };
      }

      var viewModel = new GetAllTenantsViewModel
      {
        Tenants = tenants.Select(ConvertToViewModel).ToList(),
        Page = page,
        NextPage = nextPage,
        PageSize = pageSize
      };

      return Ok(viewModel);
    }
  }
}

namespace Accounting.Validators.Tenant
{
  public class ProvisionTenantViewModelValidator
    : AbstractValidator<ProvisionTenantViewModel>
  {
    private readonly TenantService _tenantService;
    private readonly SecretService _secretService;

    public ProvisionTenantViewModelValidator(
      TenantService tenantService,
      SecretService secretService)
    {
      _tenantService = tenantService;
      _secretService = secretService;

      RuleFor(x => x.Email)
        .NotEmpty()
        .WithMessage("Email is required.")
        .DependentRules(() =>
        {
          RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .DependentRules(() =>
            {
              RuleFor(x => x)
                .MustAsync(async (model, cancellation) =>
                {
                  return !await _tenantService.ExistsAsync(model.Email!, model.OrganizationId);
                })
                .WithMessage("A tenant with this email already exists for the specified organization.");
            });
        });

      RuleFor(x => x)
        .MustAsync(async (model, cancellation) =>
            await HasRequiredSecretsAsync(model.OrganizationId))
        .WithMessage("Both 'email' and 'cloud' secret keys are required to provision a tenant.");
    }

    private async Task<bool> HasRequiredSecretsAsync(int organizationId)
    {
      var emailSecret = await _secretService.GetByTypeAsync(
        Secret.SecretTypeConstants.Email,
        organizationId);
      var cloudSecret = await _secretService.GetByTypeAsync(
        Secret.SecretTypeConstants.Cloud,
        organizationId);

      return emailSecret != null && cloudSecret != null;
    }
  }
}

namespace Accounting.Models.Tenant
{
  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }
    public bool Shared { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public int OrganizationId { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }

  public class TenantsPaginatedViewModel : PaginatedViewModel
  {

  }

  public class TenantViewModel
  {
    public int TenantID { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public string? Email { get; set; }
    public long? DropletId { get; set; }
    public string? Ipv4 { get; set; }
    public bool SshPublic { get; set; }
    public bool SshPrivate { get; set; }
    public DateTime Created { get; set; }
  }

  public class GetAllTenantsViewModel : PaginatedViewModel
  {
    public List<TenantViewModel>? Tenants { get; set; }
  }
}