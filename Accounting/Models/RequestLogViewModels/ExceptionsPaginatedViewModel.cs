namespace Accounting.Models.RequestLogViewModels
{
  public class ExceptionsPaginatedViewModel : PaginatedViewModel
  {
    public List<ExceptionLogViewModel>? Exceptions { get; set; }
  }
}