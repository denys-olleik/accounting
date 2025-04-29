INSERT INTO "Tenant" ("PublicId", "Email", "DatabaseName", "DatabasePassword") VALUES ('1', 'test@example.com', 'Accounting', 'password');

INSERT INTO "User" ("Email", "FirstName", "LastName", "Password", "CreatedById")
VALUES ('test@example.com', 'Herp', 'Derp', 'sha1:64000:18:IofuE0pk3LtysdvPabvlsENb9NJ4x7XZ:Ui8pLvVoSzlwUXVARJj8MFEL', 1);

INSERT INTO "User" ("Email", "FirstName", "LastName", "Password", "CreatedById")
VALUES ('test2@example.com', 'Derp', 'Herp', 'sha1:64000:18:IofuE0pk3LtysdvPabvlsENb9NJ4x7XZ:Ui8pLvVoSzlwUXVARJj8MFEL', 1);

INSERT INTO "Organization" (
  "Name", 
  "Address", 
  "AccountsReceivableEmail", 
  "AccountsPayableEmail", 
  "AccountsReceivablePhone", 
  "AccountsPayablePhone", 
  "Website",
  "TenantId"
) VALUES (
  'Farm To Market LLC', 
  '123 Greenway Blvd
Suite 101
Beverly Hills, CA 90210', 
  'ar@farmtomarketllc.com', 
  'ap@farmtomarketllc.com', 
  '(555) 123-4567', 
  '(555) 765-4321', 
  'http://www.farmtomarketllc.com',
  1
);

INSERT INTO "Organization" (
  "Name", 
  "Address", 
  "AccountsReceivableEmail", 
  "AccountsPayableEmail", 
  "AccountsReceivablePhone", 
  "AccountsPayablePhone", 
  "Website",
  "TenantId"
) VALUES (
  'Oil Fields Corp', 
  '456 Oil Patch Rd
Room 202
Bakersfield, CA 93308', 
  'ar@oilfieldscorp.com', 
  'ap@oilfieldscorp.com', 
  '(555) 987-6543', 
  '(555) 654-3210', 
  'http://www.oilfieldscorp.com',
  1
);

INSERT INTO "Claim" ("UserId", "ClaimType", "ClaimValue", "CreatedById", "OrganizationId", "TenantId")
VALUES (1, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', 'tenant-manager', 1, 1, 1);

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
('company', 'customer', NULL, NULL, 'Globex Corp.', 'www.globex.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'Initech', 'www.initech.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'Umbrella Corp.', 'www.umbrellacorp.com', 1, 1, 1),
('company', 'customer', 'Bruce', 'Wayne', 'Wayne Enterprises', 'www.wayneenterprises.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'Stark Industries', 'www.starkindustries.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'LexCorp', 'www.lexcorp.com', 1, 1, 1),
('company', 'customer', NULL, NULL, 'Tyrell Corp.', 'www.tyrellcorp.com', 1, 1, 1);

insert into "Address" ("ExtraAboveAddress", "AddressLine1", "City", "StateProvince", "PostalCode", "Country", "BusinessEntityId", "CreatedById", "OrganizationId")
values
('Attention Imaging', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 1, 1, 1),
('Attention Shipping', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 1, 1, 1),
('Attention Imaging', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 2, 1, 1),
('Attention Shipping', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 2, 1, 1),
('Attention Imaging', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 3, 1, 1),
('Attention Shipping', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 3, 1, 1),
('Attention Imaging', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 4, 1, 1),
('Attention Shipping', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 4, 1, 1),
('Attention Imaging', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 5, 1, 1),
('Attention Shipping', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 5, 1, 1),
('Attention Imaging', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 6, 1, 1),
('Attention Shipping', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 6, 1, 1),
('Attention Imaging', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 7, 1, 1),
('Attention Shipping', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 7, 1, 1),
('Attention Imaging', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 8, 1, 1),
('Attention Shipping', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 8, 1, 1),
('Attention Imaging', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 9, 1, 1),
('Attention Shipping', '789 Main St.', 'Anytown', 'CA', '12345', 'USA', 9, 1, 1),
('Attention Imaging', '123 Main St.', 'Anytown', 'CA', '12345', 'USA', 10, 1, 1),
('Attention Shipping', '456 Main St.', 'Anytown', 'CA', '12345', 'USA', 10, 1, 1);

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