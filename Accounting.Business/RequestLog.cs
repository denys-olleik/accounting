using Accounting.Common;

namespace Accounting.Business
{
  public class RequestLog : IIdentifiable<int>
  {
    public int RowNumber { get; set; }
    public int RequestLogID { get; set; }
    public string? RemoteIp { get; set; }
    public string? CountryCode { get; set; }
    public string? Referer { get; set; }
    public string? UserAgent { get; set; }
    public string? Path { get; set; }
    public long? ResponseLengthBytes { get; set; }
    public string? StatusCode { get; set; }
    public DateTime Created { get; set; }
    public int Identifiable => this.RequestLogID;
  }
}