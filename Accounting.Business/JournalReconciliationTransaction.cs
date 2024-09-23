using Accounting.Common;

namespace Accounting.Business
{
  public class JournalReconciliationTransaction : JournalTableBase, IIdentifiable<int>
  {
		public int JournalReconciliationTransactionID { get; set; }
		public int ReconciliationTransactionId { get; set; }
		public int? ReversedJournalReconciliationTransactionId { get; set; }
		public int CreatedById { get; set; }
		public int OrganizationId { get; set; }

    public int Identifiable => this.JournalReconciliationTransactionID;
  }
}