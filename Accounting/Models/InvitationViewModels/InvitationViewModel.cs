using FluentValidation.Results;

namespace Accounting.Models.InvitationViewModels
{
    public class InvitationViewModel
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ValidationResult ValidationResult { get; set; }
    }
}