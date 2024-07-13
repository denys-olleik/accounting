using Accounting.Common;

namespace Accounting.Business
{
  public class GeneralLedgerReconciliationTransaction : GeneralLedgerTableBase, IIdentifiable<int>
  {
		public int GeneralLedgerReconciliationTransactionID { get; set; }
		public int ReconciliationTransactionId { get; set; }
		public int? ReversedGeneralLedgerReconciliationTransactionId { get; set; }
		public int CreatedById { get; set; }
		public int OrganizationId { get; set; }

    public int Identifiable => this.GeneralLedgerReconciliationTransactionID;
  }
}