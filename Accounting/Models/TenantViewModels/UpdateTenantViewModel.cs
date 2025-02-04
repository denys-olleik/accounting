﻿using Accounting.Business;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class UpdateTenantViewModel
  {
    public int TenantId { get; set; }
    public string? Email { get; set; }
    public Tenant? ExistingTenant { get; set; }
    public ValidationResult? ValidationResult { get; set; }

    public class UpdateTenantViewModelValidator : AbstractValidator<UpdateTenantViewModel>
    {
      public UpdateTenantViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .EmailAddress()
          .WithMessage("Invalid email format.");

        RuleFor(x => x.Email)
          .Must((model, email) =>
          {
            bool isNewEmail = model.ExistingTenant!.Email != email;
            return isNewEmail;
          })
          .WithMessage("The email is already the current email.")
          .When(x => x.ExistingTenant != null);

        RuleFor(x => x.Email)
          .Must((model, email) =>
          {
            bool noConflict = model.ExistingTenant == null;
            bool isUniqueEmail = model.ExistingTenant?.Email != email;
            return noConflict || isUniqueEmail;
          })
          .WithMessage("A tenant with this email already exists.")
          .When(x => x.ExistingTenant != null);
      }
    }
  }
}