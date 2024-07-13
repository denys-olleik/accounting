using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IBusinessEntityManager : IGenericRepository<BusinessEntity, int>
  {
    Task<List<BusinessEntity>> GetAllAsync();
    Task<(List<BusinessEntity> BusinessEntities, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId);
    Task<BusinessEntity> GetByIdAsync(int id, int organizationId);
    Task<int> UpdateAsync(int id, string? firstName, string? lastName, string? companyName, string? selectedCustomerType, string? businessEntityTypesCsv);
  }
}