﻿using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ITenantManager : IGenericRepository<Tenant, int>
  {
    Task<Tenant> GetAsync(string tenantPublicId);

    Task<bool> ExistsAsync(string email);
    Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize);
    Task<int> UpdateDatabaseName(
      int tenantID, 
      string? databaseName);
    Task<int> UpdateDropletIdAsync(
      int tenantId, 
      long dropletId);
    Task<int> UpdateSshPrivateAsync(
      int tenantId, 
      string sshPrivate);
    Task<int> UpdateSshPublicAsync(
      int tenantId, 
      string sshPublic);
    Task<Tenant> GetAsync(int tenantId);
    Task<int> DeleteAsync(int tenantID);    
  }
}