using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IBlogManager : IGenericRepository<Blog, int>
  {
    Task<(List<Blog> blogs, int? nextPage)> GetAllAsync(int page, int pageSize);
  }
}