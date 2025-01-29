using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class BusinessEntityService : BaseService
  {
    public BusinessEntityService() : base()
    {

    }

    public BusinessEntityService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<BusinessEntity> CreateAsync(BusinessEntity businessEntity)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetBusinessEntityManager().CreateAsync(businessEntity);
    }

    public async Task<List<BusinessEntity>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetBusinessEntityManager().GetAllAsync(organizationId);
    }

    public async Task<(List<BusinessEntity> BusinessEntities, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetBusinessEntityManager().GetAllAsync(page, pageSize, organizationId);
    }

    public async Task<BusinessEntity> GetAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetBusinessEntityManager().GetByIdAsync(id, organizationId);
    }

    public async Task<int> UpdateAsync(int id, string? firstName, string? lastName, string? companyName, string? selectedCustomerType, string selectedBusinessEntityTypes, int? selectedPaymentTermId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetBusinessEntityManager().UpdateAsync(id, firstName, lastName, companyName, selectedCustomerType, selectedBusinessEntityTypes, selectedPaymentTermId);
    }
  }
}