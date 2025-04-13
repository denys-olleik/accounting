using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.PlaylistLoverViewModels
{
  public class ProcessLoverViewModel
  {
    private string _email;
    private string _address;

    public string Email
    {
      get => _email;
      set => _email = value?.Trim();
    }

    public string Address
    {
      get => _address;
      set => _address = value?.Trim();
    }

    public ValidationResult? ValidationResult { get; set; }

    public class ProcessLoverViewModelValidator : AbstractValidator<ProcessLoverViewModel>
    {
      public ProcessLoverViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .EmailAddress()
          .WithMessage("Invalid email format.");
        RuleFor(x => x.Address)
          .NotEmpty()
          .WithMessage("Address is required.");
      }
    }
  }
}