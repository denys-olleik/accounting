using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class BusinessEntityService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public BusinessEntityService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<BusinessEntity> CreateAsync(BusinessEntity businessEntity)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetBusinessEntityManager().CreateAsync(businessEntity);
    }

    public async Task<List<BusinessEntity>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetBusinessEntityManager().GetAllAsync(organizationId);
    }

    public async Task<(List<BusinessEntity> BusinessEntities, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetBusinessEntityManager().GetAllAsync(page, pageSize, organizationId);
    }

    public async Task<BusinessEntity> GetAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetBusinessEntityManager().GetByIdAsync(id, organizationId);
    }

    public async Task<int> UpdateAsync(int id, string? firstName, string? lastName, string? companyName, string? selectedCustomerType, string selectedBusinessEntityTypes, int? selectedPaymentTermId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetBusinessEntityManager().UpdateAsync(id, firstName, lastName, companyName, selectedCustomerType, selectedBusinessEntityTypes, selectedPaymentTermId);
    }
  }
}