using Accounting.Common;

namespace Accounting.Business
{
  public class Exception : IIdentifiable<int>
  {
    public int RowNumber { get; set; }
    public int ExceptionID { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Source { get; set; }
    public int? HResult { get; set; }
    public string TargetSite { get; set; }
    public string InnerException { get; set; }
    public int? RequestLogId { get; set; }
    public DateTime Created { get; set; }
    public int Identifiable => this.ExceptionID;
  }
}