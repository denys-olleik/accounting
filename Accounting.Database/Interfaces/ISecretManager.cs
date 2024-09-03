using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ISecretManager : IGenericRepository<Secret, int>
  {
    Task<Secret> CreateAsync(string? key, string? value, string? vendor, string? purpose, int organizationId, int createdById);
    Task<int> DeleteAsync(int id, int organizationId);
    Task<List<Secret>> GetAllAsync(int organizationId);
    Task<Secret> GetAsync(int id, int organizationId);
  }
}