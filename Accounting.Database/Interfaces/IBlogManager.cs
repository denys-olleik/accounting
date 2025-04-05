﻿using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IBlogManager : IGenericRepository<Blog, int>
  {
    Task<int> DeleteAsync(int blogId);
    Task<(List<Blog> blogs, int? nextPage)> GetAllAsync(int page, int pageSize);
    Task<Blog> GetAsync(int blogID);
  }
}