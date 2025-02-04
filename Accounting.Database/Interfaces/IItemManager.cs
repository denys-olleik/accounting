﻿using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IItemManager : IGenericRepository<Item, int>
  {
    Task<int> DeleteAsync(int itemId, bool deleteChildren);
    Task<List<Item>> GetAllAsync(int organizationId);
    Task<(List<Item> items, int? nextPage)> GetAllAsync(int page, int pageSize, int organizationId, bool includeDescendants, bool includeInventories);
    Task<Item> GetAsync(int itemId, int organizationId);
    Task<List<Item>?> GetChildrenAsync(int itemId, int organizationId);
  }
}