﻿using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class BlogService : BaseService
  {
    public BlogService() 
      : base()
    {

    }

    public BlogService(string databaseName, string databasePassword) 
      : base(databaseName, databasePassword)
    {

    }

    public async Task CreateAsync(Blog blog)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var blogManager = factoryManager.GetBlogManager();
      await blogManager.CreateAsync(blog);
    }

    public async Task<int> DeleteAsync(int blogId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var blogManager = factoryManager.GetBlogManager();
      return await blogManager.DeleteAsync(blogId);
    }

    public async Task<(List<Blog> blogs, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var blogManager = factoryManager.GetBlogManager();
      var (blogs, nextPage) = await blogManager.GetAllAsync(page, pageSize);
      return (blogs, nextPage);
    }

    public async Task<Blog> GetAsync(int blogID)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var blogManager = factoryManager.GetBlogManager();
      return await blogManager.GetAsync(blogID);
    }
  }
}