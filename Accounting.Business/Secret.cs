using Accounting.Common;

namespace Accounting.Business
{
  public class Secret : IIdentifiable<int>
  {
    public int SecretID { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
    public bool Master { get; set; }
    public string? Vendor { get; set; }
    public string? Purpose { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public User? CreatedBy { get; set; }
    public Organization? Organization { get; set; }

    public int Identifiable => this.SecretID;
  }
}