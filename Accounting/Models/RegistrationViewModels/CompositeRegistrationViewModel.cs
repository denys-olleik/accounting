namespace Accounting.Models.RegistrationViewModels
{
  public enum RegistrationType
  {
    Shared,
    Dedicated,
    Independent
  }

  public class CompositeRegistrationViewModel
  { 
    public RegistrationType SelectedRegistrationType { get; set; }
    public SharedRegistrationViewModel Shared { get; set; } = new();
    public DedicatedRegistrationViewModel Dedicated { get; set; } = new();
    public IndependentRegistrationViewModel Independent { get; set; } = new();
    public FluentValidation.Results.ValidationResult? ValidationResult { get; set; } = new();
  }
}