﻿using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class SecretService
  {
    public async Task<Secret> CreateAsync(
      string? key, 
      bool master,
      string? value, 
      string? vendor,
      string? purpose,
      int organizationId, 
      int createdById)
    {
      FactoryManager manager = new FactoryManager();
      return await manager
        .GetSecretManager()
        .CreateAsync(key, master, value, vendor, purpose, organizationId, createdById);
    }

    public async Task<int> DeleteAsync(int id, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetSecretManager().DeleteAsync(id, organizationId);
    }

    public async Task<int> DeleteMasterAsync(int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetSecretManager().DeleteMasterAsync(organizationId);
    }

    public async Task<List<Secret>> GetAllAsync(int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetSecretManager().GetAllAsync(organizationId);
    }

    public async Task<Secret> GetAsync(int id, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager
        .GetSecretManager()
        .GetAsync(id, organizationId);
    }

    public async Task<Secret> GetAsync(string key, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager
        .GetSecretManager()
        .GetAsync(key, organizationId);
    }
  }
}