```sql
-- Invoice creation and updates
SELECT * FROM "Invoice";
SELECT * FROM "InvoiceLine";
SELECT * FROM "GeneralLedger";
SELECT * FROM "GeneralLedgerInvoiceInvoiceLine";
```

```sql
-- Receiving payment
SELECT * FROM "Invoice" order by "InvoiceID" desc;
SELECT * FROM "Payment" order by "PaymentID" desc;
SELECT * FROM "InvoiceInvoiceLinePayment" order by "InvoiceInvoiceLinePaymentID" desc;
SELECT * FROM "GeneralLedger" order by "GeneralLedgerID" desc;
SELECT * FROM "GeneralLedgerInvoiceInvoiceLinePayment" order by "GeneralLedgerInvoiceInvoiceLinePaymentID" desc;
```

```sql
-- Expense reconciliation
select * from "Account";
select * from "ReconciliationTransaction";
select * from "GeneralLedger";
select * from "GeneralLedgerReconciliationTransaction";
```