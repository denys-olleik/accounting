using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ISecretManager : IGenericRepository<Secret, int>
  {
    Task<Secret> CreateAsync(string? key, string? value, int organizationId);
    Task<List<Secret>> GetAllAsync(int organizationId);
  }
}