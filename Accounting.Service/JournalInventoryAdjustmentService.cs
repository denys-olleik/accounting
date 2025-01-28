namespace Accounting.Service
{
  public class JournalInventoryAdjustmentService : BaseService
  {
    public JournalInventoryAdjustmentService() : base()
    {

    }

    public JournalInventoryAdjustmentService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {
    }
  }
}