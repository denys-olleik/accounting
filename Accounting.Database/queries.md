```sql
-- Invoice creation and updates
SELECT * FROM "Invoice";
SELECT * FROM "InvoiceLine";
SELECT * FROM "Journal";
SELECT * FROM "JournalInvoiceInvoiceLine";
```

```sql
-- Receiving payment
SELECT * FROM "Invoice" order by "InvoiceID" desc;
SELECT * FROM "Payment" order by "PaymentID" desc;
SELECT * FROM "InvoiceInvoiceLinePayment" order by "InvoiceInvoiceLinePaymentID" desc;
SELECT * FROM "Journal" order by "JournalID" desc;
SELECT * FROM "JournalInvoiceInvoiceLinePayment" order by "JournalInvoiceInvoiceLinePaymentID" desc;
```

```sql
-- Expense reconciliation
select * from "Account";
select * from "ReconciliationTransaction";
select * from "Journal";
select * from "JournalReconciliationTransaction";
```