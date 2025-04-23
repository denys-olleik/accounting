namespace Accounting.Models.DiagnosticsViewModels
{
  public class RequestLogsPaginatedViewModel : PaginatedViewModel
  {
    public List<RequestLogViewModel>? RequestLogs { get; set; }

    public class RequestLogViewModel
    {
      public int RowNumber { get; set; }
      public string RequestLogID { get; set; }
      public string RemoteIp { get; set; }
      public string CountryCode { get; set; }
      public string Referer { get; set; }
      public string UserAgent { get; set; }
      public string Path { get; set; }
      public long ResponseLengthBytes { get; set; }
      public string StatusCode { get; set; }
      public DateTime Created { get; set; }
    }
  }
}