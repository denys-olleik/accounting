using Accounting.Common;

namespace Accounting.Business
{
  public class PaymentInstruction : IIdentifiable<int>
  {
    public int PaymentInstructionID { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.PaymentInstructionID;
  }
}