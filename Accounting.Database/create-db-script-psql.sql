CREATE TABLE "Organization"
(
  "OrganizationID" SERIAL PRIMARY KEY,
  "Name" VARCHAR(100) NOT NULL,
	"Address" VARCHAR(255) NULL,
	"AccountsReceivableEmail" VARCHAR(100) NULL,
	"AccountsPayableEmail" VARCHAR(100) NULL,
	"AccountsReceivablePhone" VARCHAR(20) NULL,
	"AccountsPayablePhone" VARCHAR(20) NULL,
	"BaseCurrency" VARCHAR(3) NULL,
	"Website" VARCHAR(100) NULL,
  "PaymentInstructions" TEXT,
	"ElevatedSecurity" BOOLEAN NOT NULL DEFAULT FALSE,
  "Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC')
);

CREATE TABLE "User"
(
	"UserID" SERIAL PRIMARY KEY,
	"Email" VARCHAR(100) NOT NULL UNIQUE,
	"FirstName" VARCHAR(100) NOT NULL,
	"LastName" VARCHAR(100) NOT NULL,
	"Password" VARCHAR(255),
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID")
);

CREATE TABLE "UserOrganization"
(
	"UserOrganizationID" SERIAL PRIMARY KEY NOT NULL,
	"UserId" INT NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("UserId") REFERENCES "User"("UserID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("UserId", "OrganizationId")
);

CREATE TABLE "Cloud"
(
	"CloudID" SERIAL PRIMARY KEY NOT NULL,
	"Name" VARCHAR(100) NOT NULL,
	"Description" TEXT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Tenant"
(
	"TenantID" SERIAL PRIMARY KEY NOT NULL,
	"FullyQualifiedDomainName" VARCHAR(100) NULL, -- accounting.example.com
	"Email" VARCHAR(100) NOT NULL,
	"DropletId" BIGINT NULL,
	"Ipv4" VARCHAR(15) NULL,
	"SshPublic" TEXT NULL,
	"SshPrivate" TEXT NULL,
	"CreatedById" INT NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC')
);

CREATE TABLE "Reconciliation"
(
	"ReconciliationID" SERIAL PRIMARY KEY NOT NULL,
	"Status" VARCHAR(20) CHECK ("Status" IN ('pending', 'processed')) DEFAULT 'pending' NOT NULL,
	"StatementType" VARCHAR(20) CHECK ("StatementType" IN ('bank', 'credit-card')) NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "ReconciliationAttachment"
(
	"ReconciliationAttachmentID" SERIAL PRIMARY KEY NOT NULL,
	"OriginalFileName" VARCHAR(255) NOT NULL,
	"FilePath" VARCHAR(1000) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"ReconciliationId" INT NULL, -- Attachment can be uploaded before reconciliation creation
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ReconciliationId") REFERENCES "Reconciliation"("ReconciliationID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("ReconciliationId", "OrganizationId")
);

CREATE TABLE "Account"
(
	"AccountID" SERIAL PRIMARY KEY NOT NULL,
	"Name" VARCHAR(200) NOT NULL,
	"Type" VARCHAR(50) NOT NULL CHECK ("Type" IN ('assets', 'liabilities', 'equity', 'revenue', 'expense')),
	"InvoiceCreationForCredit" BOOLEAN NOT NULL DEFAULT FALSE,
	"InvoiceCreationForDebit" BOOLEAN NOT NULL DEFAULT FALSE,
	"ReceiptOfPaymentForCredit" BOOLEAN NOT NULL DEFAULT FALSE,
	"ReceiptOfPaymentForDebit" BOOLEAN NOT NULL DEFAULT FALSE,
	"ReconciliationExpense" BOOLEAN NOT NULL DEFAULT FALSE,
	"ReconciliationLiabilitiesAndAssets" BOOLEAN NOT NULL DEFAULT FALSE,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"ParentAccountId" INT NULL,
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ParentAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("Name", "OrganizationId")
);

CREATE TABLE "ReconciliationTransaction"
(
	"ReconciliationTransactionID" SERIAL PRIMARY KEY NOT NULL,
	"Status" VARCHAR(20) CHECK ("Status" IN ('pending', 'processed', 'error')) DEFAULT 'pending' NOT NULL,
	"RawData" TEXT NULL,
	"ReconciliationInstruction" VARCHAR(20) NULL CHECK ("ReconciliationInstruction" IN ('expense', 'revenue')),
	"TransactionDate" TIMESTAMPTZ NOT NULL,
	"PostedDate" TIMESTAMPTZ NOT NULL,
	"Description" VARCHAR(1000) NOT NULL,
	"Amount" DECIMAL(18, 2) NOT NULL,
	"Category" VARCHAR(100) NULL,
	"ExpenseAccountId" INT NULL,
	"AssetOrLiabilityAccountId" INT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"ReconciliationId" INT NOT NULL,
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ReconciliationId") REFERENCES "Reconciliation"("ReconciliationID"),
	FOREIGN KEY ("ExpenseAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("AssetOrLiabilityAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "PaymentTerm"
(
	"PaymentTermID" SERIAL PRIMARY KEY NOT NULL,
	"Description" VARCHAR(100) NOT NULL,
	"DaysUntilDue" INT NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "BusinessEntity"
(
	"BusinessEntityID" SERIAL PRIMARY KEY NOT NULL,
	"CustomerType" VARCHAR(20) NOT NULL, -- e.g., "individual", "company"
	"BusinessEntityTypesCsv" VARCHAR(20) NOT NULL, -- e.g., "customer" and or "vendor"
	"FirstName" VARCHAR(100) NULL,
	"LastName" VARCHAR(100) NULL,
	"CompanyName" VARCHAR(200) NULL,
	"Website" VARCHAR(100) NULL,
	"PaymentTermId" INT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("PaymentTermId") REFERENCES "PaymentTerm"("PaymentTermID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Address"
(
	"AddressID" SERIAL PRIMARY KEY NOT NULL,
	"ExtraAboveAddress" VARCHAR(255) NULL,
	"AddressLine1" VARCHAR(255) NOT NULL,
	"AddressLine2" VARCHAR(255) NULL,
	"ExtraBelowAddress" VARCHAR(255) NULL,
	"City" VARCHAR(100) NOT NULL,
	"StateProvince" VARCHAR(100) NULL,
	"PostalCode" VARCHAR(20) NOT NULL,
	"Country" VARCHAR(100) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"BusinessEntityId" INT NOT NULL,
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("BusinessEntityId") REFERENCES "BusinessEntity"("BusinessEntityID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Claim"
(
	"ClaimID" SERIAL PRIMARY KEY NOT NULL,
	"UserId" INT NOT NULL,
	"ClaimType" VARCHAR(100) NOT NULL,
	"ClaimValue" VARCHAR(100) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("UserId") REFERENCES "User"("UserID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("OrganizationId", "UserId", "ClaimType", "ClaimValue")
);

CREATE TABLE "Tag"
(
	"TagID" SERIAL PRIMARY KEY NOT NULL,
	"Name" VARCHAR(100) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("Name")
);

CREATE TABLE "UnitType"
(
	"UnitTypeID" SERIAL PRIMARY KEY NOT NULL,
	"Name" VARCHAR(100) NOT NULL UNIQUE,
	"Deletable" BOOLEAN NOT NULL DEFAULT TRUE,
	"CreatedById" INT NULL,
	"OrganizationId" INT NULL,
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Item"
(
	"ItemID" SERIAL PRIMARY KEY NOT NULL,
	"Name" VARCHAR(100) NOT NULL,
	"Description" VARCHAR(1000) NULL,
	"Quantity" DECIMAL(18,2) NULL,
	"SellFor" DECIMAL(18,2) NULL,
	"UnitTypeId" INT NULL,
	"ItemType" VARCHAR(100) NOT NULL CHECK ("ItemType" IN ('product', 'service')),
	"InventoryMethod" VARCHAR(100) NOT NULL CHECK ("InventoryMethod" IN ('fifo', 'lifo', 'any', 'specific')) DEFAULT 'fifo',
	"RevenueAccountId" INT NULL,
	"AssetsAccountId" INT NULL,
	"ParentItemId" INT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("UnitTypeId") REFERENCES "UnitType"("UnitTypeID"),
	FOREIGN KEY ("RevenueAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("AssetsAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("ParentItemId") REFERENCES "Item"("ItemID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Location"
(
	"LocationID" SERIAL PRIMARY KEY NOT NULL,
	"Name" VARCHAR(100) NOT NULL,
	"Description" VARCHAR(1000) NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"ParentLocationId" INT NULL,
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ParentLocationId") REFERENCES "Location"("LocationID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Inventory"
(
	"InventoryID" SERIAL PRIMARY KEY NOT NULL,
	"ItemId" INT NOT NULL,
	"LocationId" INT NOT NULL,
	"Quantity" DECIMAL(18,2) NOT NULL,
	"SellFor" DECIMAL(18,2) NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ItemId") REFERENCES "Item"("ItemID"),
	FOREIGN KEY ("LocationId") REFERENCES "Location"("LocationID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("ItemId", "LocationId")
);

CREATE TABLE "InventoryAdjustment"
(
	"InventoryAdjustmentID" SERIAL PRIMARY KEY NOT NULL,
	"ItemId" INT NOT NULL,
	"FromLocationId" INT NULL,
	"ToLocationId" INT NULL,
	"Quantity" DECIMAL(18,2) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ItemId") REFERENCES "Item"("ItemID"),
	FOREIGN KEY ("ToLocationId") REFERENCES "Location"("LocationID"),
	FOREIGN KEY ("FromLocationId") REFERENCES "Location"("LocationID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "ToDo"
(
	"ToDoID" SERIAL PRIMARY KEY NOT NULL,
	"Title" VARCHAR(100) NOT NULL,
	"Content" TEXT NULL,
	"ParentToDoId" INT NULL,
	"Status" VARCHAR(100) NOT NULL CHECK ("Status" IN ('open', 'closed', 'completed')),
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ParentToDoId") REFERENCES "ToDo"("ToDoID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "ToDoTag"
(
	"ToDoTagID" SERIAL PRIMARY KEY NOT NULL,
	"ToDoId" INT NOT NULL,
	"TagId" INT NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("ToDoId") REFERENCES "ToDo"("ToDoID"),
	FOREIGN KEY ("TagId") REFERENCES "Tag"("TagID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("ToDoId", "TagId")
);

CREATE TABLE "UserToDo"
(
	"UserToDoID" SERIAL PRIMARY KEY NOT NULL,
	"UserId" INT NOT NULL,
	"ToDoId" INT NOT NULL,
	"Completed" BOOLEAN NOT NULL DEFAULT FALSE,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("UserId") REFERENCES "User"("UserID"),
	FOREIGN KEY ("ToDoId") REFERENCES "ToDo"("ToDoID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("UserId", "ToDoId")
);

CREATE TABLE "Invitation"
(
	"InvitationID" SERIAL PRIMARY KEY NOT NULL,
	"Guid" UUID NOT NULL,
	"Email" VARCHAR(100) NOT NULL,
	"FirstName" VARCHAR(100) NOT NULL,
	"LastName" VARCHAR(100) NOT NULL,
	"UserId" INT NOT NULL,
	"Expiration" TIMESTAMPTZ NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("UserId") REFERENCES "User"("UserID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "Payment"
(
	"PaymentID" SERIAL PRIMARY KEY NOT NULL,
	"ReferenceNumber" VARCHAR(100) NULL,
	"Amount" DECIMAL(18, 2) NOT NULL,
	"VoidReason" TEXT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE SEQUENCE "InvoiceNumberSeq" AS INT
START WITH 1500
INCREMENT BY 1;

CREATE TABLE "Invoice"
(
	"InvoiceID" SERIAL PRIMARY KEY NOT NULL,
	"InvoiceNumber" INT UNIQUE NOT NULL DEFAULT NEXTVAL('"InvoiceNumberSeq"'),
	"BusinessEntityId" INT NOT NULL,
	"BillingAddressJSON" TEXT NOT NULL,
	"ShippingAddressJSON" TEXT NULL,
	"DueDate" TIMESTAMPTZ,
	"Status" VARCHAR(100) NOT NULL CHECK ("Status" IN ('unpaid', 'partially-paid', 'paid', 'void')),
	"PaymentInstructions" TEXT,
	"TotalAmount" DECIMAL(18, 2) NOT NULL,
	"ReceivedAmount" DECIMAL(18, 2) NOT NULL DEFAULT 0,
	"VoidReason" TEXT,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"LastUpdated" TIMESTAMPTZ NOT NULL,
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("BusinessEntityId") REFERENCES "BusinessEntity"("BusinessEntityID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "InvoiceLine"
(
	"InvoiceLineID" SERIAL PRIMARY KEY NOT NULL,
	"Title" VARCHAR(100) NOT NULL,
	"Description" TEXT NULL,
	"Quantity" DECIMAL(18, 2) NOT NULL,
	"Price" DECIMAL(18, 2) NOT NULL,
	"InvoiceId" INT NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"RevenueAccountId" INT NOT NULL,
	"AssetsAccountId" INT NOT NULL,
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("InvoiceId") REFERENCES "Invoice"("InvoiceID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("RevenueAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("AssetsAccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "InvoiceAttachment"
(
	"InvoiceAttachmentID" SERIAL PRIMARY KEY NOT NULL,
	"InvoiceId" INT NULL,
	"PrintOrder" INT NULL,
	"OriginalFileName" VARCHAR(255) NOT NULL,
	"FilePath" VARCHAR(1000) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("InvoiceId") REFERENCES "Invoice"("InvoiceID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID"),
	UNIQUE ("InvoiceId", "PrintOrder")
);

CREATE TABLE "Journal"
(
	"JournalID" SERIAL PRIMARY KEY NOT NULL,
	"AccountId" INT NOT NULL,
	"Credit" DECIMAL(20, 4) NULL,
	"Debit" DECIMAL(20, 4) NULL,
	"CurrencyCode" VARCHAR(3) NULL, 
	"ExchangeRate" DECIMAL(12, 5) NULL, 
	"Memo" TEXT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("AccountId") REFERENCES "Account"("AccountID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "JournalInventoryAdjustment"
(
	"JournalInventoryAdjustmentID" SERIAL PRIMARY KEY NOT NULL,
	"JournalId" INT NOT NULL,
	"InventoryAdjustmentId" INT NOT NULL,
	"ReversedJournalInventoryAdjustmentId" INT NULL,
	"TransactionGuid" UUID NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("JournalId") REFERENCES "Journal"("JournalID"),
	FOREIGN KEY ("InventoryAdjustmentId") REFERENCES "InventoryAdjustment"("InventoryAdjustmentID"),
	FOREIGN KEY ("ReversedJournalInventoryAdjustmentId") REFERENCES "JournalInventoryAdjustment"("JournalInventoryAdjustmentID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "JournalInvoiceInvoiceLine"
(
	"JournalInvoiceInvoiceLineID" SERIAL PRIMARY KEY NOT NULL,
	"JournalId" INT NOT NULL,
	"InvoiceId" INT NOT NULL,
	"InvoiceLineId" INT NOT NULL,
	"ReversedJournalInvoiceInvoiceLineId" INT NULL,
	"TransactionGuid" UUID NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("JournalId") REFERENCES "Journal"("JournalID"),
	FOREIGN KEY ("InvoiceId") REFERENCES "Invoice"("InvoiceID"),
	FOREIGN KEY ("InvoiceLineId") REFERENCES "InvoiceLine"("InvoiceLineID"),
	FOREIGN KEY ("ReversedJournalInvoiceInvoiceLineId") REFERENCES "JournalInvoiceInvoiceLine"("JournalInvoiceInvoiceLineID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "InvoiceInvoiceLinePayment"
(
	"InvoiceInvoiceLinePaymentID" SERIAL PRIMARY KEY NOT NULL,
	"InvoiceId" INT NOT NULL,
	"InvoiceLineId" INT NOT NULL,
	"PaymentId" INT NOT NULL,
	"Amount" DECIMAL(18, 2) NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("InvoiceId") REFERENCES "Invoice"("InvoiceID"),
	FOREIGN KEY ("InvoiceLineId") REFERENCES "InvoiceLine"("InvoiceLineID"),
	FOREIGN KEY ("PaymentId") REFERENCES "Payment"("PaymentID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "JournalInvoiceInvoiceLinePayment"
(
	"JournalInvoiceInvoiceLinePaymentID" SERIAL PRIMARY KEY NOT NULL,
	"JournalId" INT NOT NULL,
	"InvoiceInvoiceLinePaymentId" INT NOT NULL,
	"ReversedJournalInvoiceInvoiceLinePaymentId" INT NULL,
	"TransactionGuid" UUID NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("JournalId") REFERENCES "Journal"("JournalID"),
	FOREIGN KEY ("InvoiceInvoiceLinePaymentId") REFERENCES "InvoiceInvoiceLinePayment"("InvoiceInvoiceLinePaymentID"),
	FOREIGN KEY ("ReversedJournalInvoiceInvoiceLinePaymentId") REFERENCES "JournalInvoiceInvoiceLinePayment"("JournalInvoiceInvoiceLinePaymentID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "JournalReconciliationTransaction"
(
	"JournalReconciliationTransactionID" SERIAL PRIMARY KEY NOT NULL,
	"JournalId" INT NOT NULL,
	"ReconciliationTransactionId" INT NOT NULL,
	"ReversedJournalReconciliationTransactionId" INT NULL,
	"TransactionGuid" UUID NOT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("JournalId") REFERENCES "Journal"("JournalID"),
	FOREIGN KEY ("ReconciliationTransactionId") REFERENCES "ReconciliationTransaction"("ReconciliationTransactionID"),
	FOREIGN KEY ("ReversedJournalReconciliationTransactionId") REFERENCES "JournalReconciliationTransaction"("JournalReconciliationTransactionID"),
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "PaymentInstruction"
(
	"PaymentInstructionID" SERIAL PRIMARY KEY NOT NULL,
	"Title" VARCHAR(100) NOT NULL,
	"Content" TEXT NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE TABLE "RequestLog"
(
	"RequestLogID" SERIAL PRIMARY KEY NOT NULL,
	"RemoteIp" VARCHAR(50) NULL,
	"CountryCode" VARCHAR(5) NULL,
	"Referer" VARCHAR(2000) NULL,
	"UserAgent" VARCHAR(1000) NULL,
	"Path" VARCHAR(1000) NULL,
	"ResponseLengthBytes" BIGINT NULL,
	"StatusCode" VARCHAR(3) NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC')
);

--CREATE EXTENSION postgis;

--CREATE TABLE "ZipCode"
--(
--    "ID" SERIAL PRIMARY KEY NOT NULL,
--    "Zip5" VARCHAR(5) NOT NULL,
--    "Latitude" FLOAT NULL,
--    "Longitude" FLOAT NULL,
--    "City" VARCHAR(50) NOT NULL,
--    "State2" VARCHAR(2) NOT NULL,
--    "Location" GEOGRAPHY NULL
--);

CREATE EXTENSION pgcrypto;

CREATE TABLE "Secret"
(
	"SecretID" SERIAL PRIMARY KEY NOT NULL,
	"Key" VARCHAR(100) NOT NULL UNIQUE,
	"Master" BOOLEAN DEFAULT FALSE,
	"Value" TEXT NOT NULL,
	"Type" VARCHAR(20) CHECK ("Type" IN ('email', 'sms', 'cloud')) NULL,
	"Purpose" VARCHAR(100) NULL,
	"Created" TIMESTAMPTZ NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
	"CreatedById" INT NOT NULL,
	"OrganizationId" INT NOT NULL,
	FOREIGN KEY ("CreatedById") REFERENCES "User"("UserID"),
	FOREIGN KEY ("OrganizationId") REFERENCES "Organization"("OrganizationID")
);

CREATE UNIQUE INDEX unique_master_per_organization
ON "Secret" ("OrganizationId")
WHERE "Master" = TRUE;

CREATE UNIQUE INDEX unique_type_per_organization
ON "Secret" ("OrganizationId", "Type");

INSERT INTO "User" ("Email", "FirstName", "LastName", "Password", "CreatedById")
VALUES ('test@example.com', 'Some', 'Dude', 'sha1:64000:18:IofuE0pk3LtysdvPabvlsENb9NJ4x7XZ:Ui8pLvVoSzlwUXVARJj8MFEL', 1);

INSERT INTO "User" ("Email", "FirstName", "LastName", "Password", "CreatedById")
VALUES ('test2@example.com', 'Some', 'Lady', 'sha1:64000:18:IofuE0pk3LtysdvPabvlsENb9NJ4x7XZ:Ui8pLvVoSzlwUXVARJj8MFEL', 1);


INSERT INTO "Organization" (
  "Name", 
  "Address", 
  "AccountsReceivableEmail", 
  "AccountsPayableEmail", 
  "AccountsReceivablePhone", 
  "AccountsPayablePhone", 
  "Website"
) VALUES (
  'Farm To Market LLC', 
  '123 Greenway Blvd
Suite 101
Beverly Hills, CA 90210', 
  'ar@farmtomarketllc.com', 
  'ap@farmtomarketllc.com', 
  '(555) 123-4567', 
  '(555) 765-4321', 
  'http://www.farmtomarketllc.com'
);

INSERT INTO "Organization" (
  "Name", 
  "Address", 
  "AccountsReceivableEmail", 
  "AccountsPayableEmail", 
  "AccountsReceivablePhone", 
  "AccountsPayablePhone", 
  "Website"
) VALUES (
  'Oil Fields Corp', 
  '456 Oil Patch Rd
Room 202
Bakersfield, CA 93308', 
  'ar@oilfieldscorp.com', 
  'ap@oilfieldscorp.com', 
  '(555) 987-6543', 
  '(555) 654-3210', 
  'http://www.oilfieldscorp.com'
);

INSERT INTO "UserOrganization" ("UserId", "OrganizationId") VALUES (1, 1);
INSERT INTO "UserOrganization" ("UserId", "OrganizationId") VALUES (1, 2);

INSERT INTO "UnitType" ("Name", "Deletable", "OrganizationId") VALUES ('each', false, 1);
INSERT INTO "UnitType" ("Name", "Deletable", "OrganizationId") VALUES ('hour', false, 1);

INSERT INTO "Account" ("Name", "Type", "InvoiceCreationForDebit", "CreatedById", "OrganizationId") VALUES ('accounts-receivable', 'assets', TRUE, 1, 1);
INSERT INTO "Account" ("Name", "Type", "InvoiceCreationForCredit", "CreatedById", "OrganizationId") VALUES ('revenue', 'revenue', TRUE, 1, 1);
INSERT INTO "Account" ("Name", "Type", "InvoiceCreationForCredit", "ParentAccountId", "CreatedById", "OrganizationId") VALUES ('revenue-service', 'revenue', TRUE, 2, 1, 1);
INSERT INTO "Account" ("Name", "Type", "InvoiceCreationForCredit", "ParentAccountId", "CreatedById", "OrganizationId") VALUES ('revenue-product', 'revenue', TRUE, 2, 1, 1);
INSERT INTO "Account" ("Name", "Type", "InvoiceCreationForDebit", "ReceiptOfPaymentForDebit", "CreatedById", "OrganizationId") VALUES ('cash', 'assets', TRUE, TRUE, 1, 1);
INSERT INTO "Account" ("Name", "Type", "CreatedById", "OrganizationId") VALUES ('chase-9988', 'assets', 1, 1);
INSERT INTO "Account" ("Name", "Type", "ParentAccountId", "ReconciliationLiabilitiesAndAssets", "CreatedById", "OrganizationId") VALUES ('chase-9988-debit-card-2323', 'assets', 6, TRUE, 1, 1);
INSERT INTO "Account" ("Name", "Type", "ReconciliationExpense", "CreatedById", "OrganizationId") VALUES ('expense', 'expense', TRUE, 1, 1);
INSERT INTO "Account" ("Name", "Type", "ReconciliationExpense", "ParentAccountId", "CreatedById", "OrganizationId") VALUES ('expense-diesel', 'expense', TRUE,  8, 1, 1);
INSERT INTO "Account" ("Name", "Type", "ReconciliationExpense", "ParentAccountId", "CreatedById", "OrganizationId") VALUES ('expense-meals', 'expense', TRUE,  8, 1, 1);
INSERT INTO "Account" ("Name", "Type", "ReconciliationExpense", "ParentAccountId", "CreatedById", "OrganizationId") VALUES ('expense-maintenance', 'expense', TRUE,  8, 1, 1);
INSERT INTO "Account" ("Name", "Type", "ReconciliationLiabilitiesAndAssets", "CreatedById", "OrganizationId") VALUES ('discover-5555', 'liabilities', TRUE, 1, 1);
INSERT INTO "Account" ("Name", "Type", "CreatedById", "OrganizationId") VALUES ('inventory', 'assets', 1, 1);

INSERT INTO "PaymentTerm" ("Description", "DaysUntilDue", "OrganizationId", "CreatedById") VALUES ('Net 30', 30, 1, 1);
INSERT INTO "PaymentTerm" ("Description", "DaysUntilDue", "OrganizationId", "CreatedById") VALUES ('Net 60', 60, 1, 1);

INSERT INTO "BusinessEntity" ("CustomerType", "BusinessEntityTypesCsv", "FirstName", "LastName", "CompanyName", "Website", "PaymentTermId", "CreatedById", "OrganizationId")
VALUES
('individual', 'customer', 'John', 'Smith', NULL, 'www.john-smith.com', 1, 1, 1),
('individual', 'customer', 'Jane', 'Doe', NULL, 'www.jane-doe.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'Acme Inc.', 'www.acmeinc.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'Globex Corp.', 'www.globex.com', 1, 1, 1);

insert into "Address" ("ExtraAboveAddress", "AddressLine1", "City", "StateProvince", "PostalCode", "Country", "BusinessEntityId", "CreatedById", "OrganizationId")
values
('Attention Imaging', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 1, 1, 1),
('Attention Shipping', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 1, 1, 1),
('Attention Imaging', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 2, 1, 1),
('Attention Shipping', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 2, 1, 1),
('Attention Imaging', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 3, 1, 1),
('Attention Shipping', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 3, 1, 1),
('Attention Imaging', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 4, 1, 1),
('Attention Shipping', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 4, 1, 1);

 -- Inserting realistic Products for a manufacturing company
INSERT INTO "Item" ("Name", "Description", "ItemType", "RevenueAccountId", "AssetsAccountId", "CreatedById", "OrganizationId") VALUES
('Industrial Pump', 'High-capacity industrial water pump for manufacturing and agricultural applications.', 'product', 4, 1, 1, 1),
('CNC Milling Machine', 'Precision CNC milling machine for metalworking and fabrication.', 'product', 4, 1, 1, 1),
('Hydraulic Press', '20-ton hydraulic press for shaping and forging metal components.', 'product', 4, 1, 1, 1),
('Automated Conveyor Belt', 'Customizable automated conveyor belt system for assembly lines.', 'product', 4, 1, 1, 1),
('Electric Motor', 'High-efficiency electric motor for industrial machinery.', 'product', 4, 1, 1, 1);

-- Inserting realistic Services for a manufacturing company
INSERT INTO "Item" ("Name", "Description", "ItemType", "RevenueAccountId", "AssetsAccountId", "CreatedById", "OrganizationId") VALUES
('Machine Maintenance', 'Comprehensive maintenance service for industrial machinery, including diagnostics, repair, and parts replacement.', 'service', 3, 1, 1, 1),
('Custom Fabrication', 'Custom fabrication services for metal components based on client specifications.', 'service', 3, 1, 1, 1),
('Consulting Services', 'Expert consulting on manufacturing process optimization and equipment selection.', 'service', 3, 1, 1, 1),
('Installation Services', 'Professional installation services for industrial machinery and equipment.', 'service', 3, 1, 1, 1),
('Training Services', 'On-site and remote training services for machinery operation and maintenance.', 'service', 3, 1, 1, 1);

INSERT INTO "Location" ("Name", "Description", "ParentLocationId", "CreatedById", "OrganizationId") VALUES
('Main Warehouse', 'Main warehouse for Farm To Market LLC.', NULL, 1, 1),
('Department A', 'Department A of the main warehouse.', 1, 1, 1),
('Shelf', 'Shelf in Department A.', 2, 1, 1),
('Bin', 'Bin in Shelf.', 3, 1, 1),
('Department B', 'Department B of the main warehouse.', 1, 1, 1),
('Shelf', 'Shelf in Department B.', 5, 1, 1),
('Bin', 'Bin in Shelf.', 6, 1, 1);