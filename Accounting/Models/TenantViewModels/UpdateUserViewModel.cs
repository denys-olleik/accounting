using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class UpdateUserViewModel
  {
    public int TenantId { get; set; }
    public TenantViewModel? Tenant { get; set; }
    public int UserID { get; set; }
    public string? Email { get; set; }

    private string? _firstName;
    public string? FirstName
    {
      get => _firstName;
      set => _firstName = value?.Trim();
    }

    private string? _lastName;
    public string? LastName
    {
      get => _lastName;
      set => _lastName = value?.Trim();
    }

    public List<OrganizationViewModel> AvailableOrganizations { get; set; } = new List<OrganizationViewModel>();
    public string? SelectedOrganizationIdsCsv { get; set; }

    public class TenantViewModel
    {
      public int TenantID { get; set; }
      public string? DatabaseName { get; set; }
    }

    public ValidationResult? ValidationResult { get; set; } = new ValidationResult();

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string? Name { get; set; }
    }

    public class UpdateUserViewModelValidator : AbstractValidator<UpdateUserViewModel>
    {
      public UpdateUserViewModelValidator()
      {
        RuleFor(x => x.FirstName)
          .NotEmpty()
          .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
          .NotEmpty()
          .WithMessage("Last name is required.");
      }
    }
  }
}