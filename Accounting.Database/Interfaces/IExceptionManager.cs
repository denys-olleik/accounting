
namespace Accounting.Database.Interfaces
{
  public interface IExceptionManager : IGenericRepository<Business.Exception, int>
  {
    Task<(IEnumerable<Business.Exception> exceptions, int? nextPage)> GetAllAsync(int page, int pageSize);
  }
}