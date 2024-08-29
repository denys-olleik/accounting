# GAAP & IFRS Compliant ERP System Design

An implementation of a forward-only double-entry accounting method in a relational database.

* **Using**: C# • ASP.NET MVC • Vue.js • PostgreSQL
* **Avoiding**: Reach • Angular

**You're not an accountant. You are just a QuickBooks user.**

## Set Up

0. Have `dotnet` installed.
1. Clone the repository.
2. Check connection strings in `appsettings.json` to make sure you have database (`postgres`) with proper credentials.
3. Have PSQL and PostGIS installed.
4. Set `database-reset.json` to `true` and run. This will create/reset the database with some sample data.

## Introduction to general ledger and chart of accounts

In accounting, there are five types of accounts: Assets, Liabilities, Equity, Revenues, and Expenses. Every transaction affects at least two accounts. Examples of transactions include creating invoices, receiving payments, performing reconciliations, and adjusting inventory, among others.

To become familiar with the system, one effective approach is to examine the database schema. The `ChartOfAccount` and `GeneralLedger` is a good start. If you had to choose only two tables to back up, these would be the ones.

```sql
CREATE TABLE "ChartOfAccount"
(
  "ChartOfAccountID" SERIAL PRIMARY KEY NOT NULL,
  "Name" VARCHAR(200) NOT NULL,
  "Type" VARCHAR(50) NOT NULL CHECK ("Type" IN  ('assets', 'liabilities', 'equity',  'revenue',  'expense')),
  ...
  "Created" TIMESTAMPTZ NOT NULL DEFAULT  (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
  "ParentChartOfAccountId" INT NULL,
  "CreatedById" INT NOT NULL,
  "OrganizationId" INT NOT NULL,
  FOREIGN KEY ("ParentChartOfAccountId") REFERENCES   "ChartOfAccount"("ChartOfAccountID"),
  FOREIGN KEY ("CreatedById") REFERENCES "User" ("UserID"),
  FOREIGN KEY ("OrganizationId") REFERENCES   "Organization"("OrganizationID"),
  UNIQUE ("Name", "OrganizationId")
);

CREATE TABLE "GeneralLedger"
(
  "GeneralLedgerID" SERIAL PRIMARY KEY NOT NULL,
  "ChartOfAccountId" INT NOT NULL,
  "Credit" DECIMAL(18, 2) NULL,
  "Debit" DECIMAL(18, 2) NULL,
  "Memo" TEXT NULL,
  "Created" TIMESTAMPTZ NOT NULL DEFAULT  (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
  "CreatedById" INT NOT NULL,
  "OrganizationId" INT NOT NULL,
  FOREIGN KEY ("ChartOfAccountId") REFERENCES   "ChartOfAccount"("ChartOfAccountID"),
  FOREIGN KEY ("CreatedById") REFERENCES "User" ("UserID"),
  FOREIGN KEY ("OrganizationId") REFERENCES   "Organization"("OrganizationID")
);
```

The `GeneralLedger` is the journal table and contains the `credit` and `debit` columns.

A typical `ChartOfAccount` might look like...

| ChartOfAccountID | Name                          | Type       | ParentChartOfAccountId |
|------------------|-------------------------------|------------|------------------------|
| 1                | accounts-receivable           | assets     | NULL                   |
| 2                | revenue                       | revenue    | NULL                   |
| 3                | revenue-service               | revenue    | 2                      |
| 4                | revenue-product               | revenue    | 2                      |
| 5                | cash                          | assets     | NULL                   |
| 6                | chase-9988                    | assets     | NULL                   |
| 7                | chase-9988-debit-card-2323    | assets     | 6                      |
| 8                | expense                       | expense    | NULL                   |
| 9                | expense-diesel                | expense    | 8                      |
| 10               | expense-meals                 | expense    | 8                      |
| 11               | expense-maintenance           | expense    | 8                      |
| 12               | discover-5555                 | liabilities| NULL                   |
| 13               | inventory                     | assets     | NULL                   |
| 14               | sales-tax-payable             | liabilities| NULL                   |

Keep an eye on number 14.

The `ParentChartOfAccountId` allows for hierarchical relationships which allow for better reporting and analysis. However, ensuring that the `ParentChartOfAccountId` is of the same `Type` as the `ChartOfAccountID` would have to be enforced at the application level.

### Double entry is misleading

Double entry accounting is misleading because a transaction doesn't always have to have only a pair of credit and debit entries.

For example, consider an invoice with one line item where the sales tax has to be collected. Here is what the journal entries might look like...

| GeneralLedgerID | ChartOfAccountId | Credit  | Debit   |
|-----------------|------------------|---------|---------|
| 1               | 1  (ar)          | 0.00    | 1100.00 |
| 2               | 2  (revenue)     | 1000.00 | 0.00    |
| 3               | 14 (liabilities) | 100.00  | 0.00    |

As you can see there is one   debit and two credit entries.

Essentially a tripple entry transaction.

### Forward only

When an invoice is created, initial entries are made in the journal. If the invoice needs to be modified, those entries are reversed, and new ones are appended.

Because of the requirements to track the revenue at the line item level, each line item will have it's own set of ledger entries that represent its current and historical state.

The `GeneralLedgerInvoiceInvoiceLine` links the journal entries with the creation, update, and removal of invoice line item events.

```sql
CREATE TABLE "GeneralLedgerInvoiceInvoiceLine"
(
  "GeneralLedgerInvoiceInvoiceLineID" SERIAL PRIMARY KEY NOT NULL,
  "GeneralLedgerId" INT NOT NULL,
  "InvoiceId" INT NOT NULL,
  "InvoiceLineId" INT NOT NULL,
  "ReversedGeneralLedgerInvoiceInvoiceLineId" INT NULL,
  "TransactionGuid" UUID NOT NULL,
  "Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
  "CreatedById" INT NOT NULL,
  "OrganizationId" INT NOT NULL,
  FOREIGN KEY ("GeneralLedgerId") REFERENCES "GeneralLedger"("GeneralLedgerID"),
  FOREIGN KEY ("InvoiceId") REFERENCES "Invoice"("InvoiceID"),
  FOREIGN KEY ("InvoiceLineId") REFERENCES "InvoiceLine"("InvoiceLineID"),
  FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
  FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);
```

Below is the result of `SELECT * FROM "GeneralLedgerInvoiceInvoiceLine";` after the invoice was created, updated, and had one of its line items removed. You can tell the sequence of events by examining the `TransactionGuid`.

|ID|GeneralLedgerId|InvoiceId|InvoiceLineId|ReversedId|TransactionGuid|
|---|---|---|---|---|---|
|1|2|1|1|NULL|e0b84e1c-e62d-60f2-0a96-61f895c14fcf|2024-06-19 22:55:38.882187-05|
|2|1|1|1|NULL|e0b84e1c-e62d-60f2-0a96-61f895c14fcf|2024-06-19 22:55:38.882187-05|
|3|4|1|2|NULL|e0b84e1c-e62d-60f2-0a96-61f895c14fcf|2024-06-19 22:55:38.882187-05|
|4|3|1|2|NULL|e0b84e1c-e62d-60f2-0a96-61f895c14fcf|2024-06-19 22:55:38.882187-05|
|5|5|1|1|1|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|6|6|1|1|NULL|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|7|7|1|1|2|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|8|8|1|1|NULL|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|9|9|1|2|3|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|10|10|1|2|NULL|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|11|11|1|2|4|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|12|12|1|2|NULL|91977936-5d6a-401b-7ea6-9092eee92918|2024-06-19 22:59:32.477332-05|
|13|13|1|1|6|227fa74a-3166-01b6-61db-0f6a6af4a52e|2024-06-19 23:07:32.54741-05|
|14|14|1|1|8|227fa74a-3166-01b6-61db-0f6a6af4a52e|2024-06-19 23:07:32.54741-05|

Note the reversing entries and the three separate transactions.

1. IDs 1-4 invoice is created.
2. IDs 5-12 both line items updated, the reversal and new entries are same transaction. 
3. IDs 13-14 one line item is removed.

To prevent confusion regarding how to determine if a line item was removed from an invoice, and without adding additional flags or columns, the line items whose transactions end with a reversal are considered removed from the invoice.

## Payments

Payments are received against invoice line items, can be partial, and can apply against multiple invoices.

The `GeneralLedgerInvoiceInvoiceLinePayment` links the journal entries with the payment.

```sql
CREATE TABLE "GeneralLedgerInvoiceInvoiceLinePayment"
(
  "GeneralLedgerInvoiceInvoiceLinePaymentID" SERIAL PRIMARY KEY NOT NULL,
  "GeneralLedgerId" INT NOT NULL,
  "InvoiceInvoiceLinePaymentId" INT NOT NULL,
  "ReversedGeneralLedgerInvoiceInvoiceLinePaymentId" INT NULL,
  "TransactionGuid" UUID NOT NULL,
  "Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE  'UTC'),
  "CreatedById" INT NOT NULL,
  "OrganizationId" INT NOT NULL,
  FOREIGN KEY ("GeneralLedgerId") REFERENCES "GeneralLedger"  ("GeneralLedgerID"),
  FOREIGN KEY ("InvoiceInvoiceLinePaymentId") REFERENCES  "InvoiceInvoiceLinePayment"("InvoiceInvoiceLinePaymentID"),
  FOREIGN KEY ("ReversedGeneralLedgerInvoiceInvoiceLinePaymentId")  REFERENCES "GeneralLedgerInvoiceInvoiceLinePayment"  ("GeneralLedgerInvoiceInvoiceLinePaymentID"),
  FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
  FOREIGN KEY ("OrganizationId") REFERENCES "Organization"  ("OrganizationID")
);
```

The pattern repeats in the `GeneralLedgerReconciliationTransaction` table. A bank or credit card statement contains rows of transactions. Credit card statements will mostly have expense transactions while bank statements will have both expense and revenue transactions.

Each transaction in the statement should be uniquely identifiable, but they rarely are—especially when the statement is imported from a CSV file.

```sql
CREATE TABLE "GeneralLedgerReconciliationTransaction"
(
  "GeneralLedgerReconciliationTransactionID" SERIAL PRIMARY KEY NOT NULL,
  "GeneralLedgerId" INT NOT NULL,
  "ReconciliationTransactionId" INT NOT NULL,
  "ReversedGeneralLedgerReconciliationTransactionId" INT NULL,
  "TransactionGuid" UUID NOT NULL,
  "Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE  'UTC'),
  "CreatedById" INT NOT NULL,
  "OrganizationId" INT NOT NULL,
  FOREIGN KEY ("GeneralLedgerId") REFERENCES "GeneralLedger"  ("GeneralLedgerID"),
  FOREIGN KEY ("ReconciliationTransactionId") REFERENCES  "ReconciliationTransaction"("ReconciliationTransactionID"),
  FOREIGN KEY ("ReversedGeneralLedgerReconciliationTransactionId")  REFERENCES "GeneralLedgerReconciliationTransaction"  ("GeneralLedgerReconciliationTransactionID"),
  FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
  FOREIGN KEY ("OrganizationId") REFERENCES "Organization"  ("OrganizationID")
);
```

There are integration options with banks and credit cards, but the option to import a CSV file is still a requirement.

Note: An incoming check is usually entered into the system before it's deposited. Ideally, the check should be entered into a check-in-transit or similar account and further reconciled when the deposit appears on the statement.

## Backups

The system will have three levels of backups.

1. **Ledger** - System will attempt to perform a backup upon every action/transaction that affects the ledger. Backups will be in human and machine readable format such as csv, json, or xml (you'll get to pick whichever you hate the least). 
2. **Database** - Performed as often as resources allow, ideally every hour.
3. **Instance** - Entire virtual machine instance is backed up daily.
