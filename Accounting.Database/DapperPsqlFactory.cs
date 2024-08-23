using Dapper;
using Accounting.Business;
using Accounting.Common;
using Accounting.Database.Interfaces;
using Npgsql;
using System.Data;
using static Accounting.Business.Invoice;
using static System.Net.Mime.MediaTypeNames;

namespace Accounting.Database
{
  public class DapperPsqlFactory : IDatabaseFactoryDefinition
  {
    public IAddressManager GetAddressManager()
    {
      return new AddressManager();
    }

    public class AddressManager : IAddressManager
    {
      public Address Create(Address entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Address> CreateAsync(Address entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ExtraAboveAddress", entity.ExtraAboveAddress);
        p.Add("@AddressLine1", entity.AddressLine1);
        p.Add("@AddressLine2", entity.AddressLine2);
        p.Add("@ExtraBelowAddress", entity.ExtraBelowAddress);
        p.Add("@City", entity.City);
        p.Add("@StateProvince", entity.StateProvince);
        p.Add("@PostalCode", entity.PostalCode);
        p.Add("@Country", entity.Country);
        p.Add("@BusinessEntityId", entity.BusinessEntityId);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@CreatedById", entity.CreatedById);

        IEnumerable<Address> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Address>("""
          INSERT INTO "Address" 
          ("ExtraAboveAddress", "AddressLine1", "AddressLine2", "ExtraBelowAddress", "City", "StateProvince", "PostalCode", "Country", "BusinessEntityId", "OrganizationId", "CreatedById")
          VALUES 
          (@ExtraAboveAddress, @AddressLine1, @AddressLine2, @ExtraBelowAddress, @City, @StateProvince, @PostalCode, @Country, @BusinessEntityId, @OrganizationId, @CreatedById)
          RETURNING *;
          """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(int businessEntityId)
      {
        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "Address" WHERE "BusinessEntityId" = @BusinessEntityId
            """, new { BusinessEntityId = businessEntityId });
        }

        return rowsAffected;
      }


      public Address Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Address> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Address>?> GetAllAsync(int businessEntityId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@BusinessEntityId", businessEntityId);

        IEnumerable<Address> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Address>("""
            SELECT * 
            FROM "Address" 
            WHERE "BusinessEntityId" = @BusinessEntityId
            """, p);
        }

        return result.ToList();
      }

      public async Task<Address?> GetAsync(int selectedAddressId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ID", selectedAddressId);

        IEnumerable<Address> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Address>("""
            SELECT * 
            FROM "Address" 
            WHERE "AddressID" = @ID
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(Address entity)
      {
        throw new NotImplementedException();
      }
    }

    public IBusinessEntityManager GetBusinessEntityManager()
    {
      return new BusinessEntityManager();
    }

    public class BusinessEntityManager : IBusinessEntityManager
    {
      public BusinessEntity Create(BusinessEntity entity)
      {
        throw new NotImplementedException();
      }

      public async Task<BusinessEntity> CreateAsync(BusinessEntity entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@CustomerType", entity.CustomerType);
        p.Add("@BusinessEntityTypesCsv", entity.BusinessEntityTypesCsv);
        p.Add("@FirstName", entity.FirstName);
        p.Add("@LastName", entity.LastName);
        p.Add("@CompanyName", entity.CompanyName);
        p.Add("@PaymentTermId", entity.PaymentTermId);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<BusinessEntity> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<BusinessEntity>("""
            INSERT INTO "BusinessEntity" ("CustomerType", "BusinessEntityTypesCsv", "FirstName", "LastName", "CompanyName", "PaymentTermId", "CreatedById", "OrganizationId", "Created") 
            VALUES (@CustomerType, @BusinessEntityTypesCsv, @FirstName, @LastName, @CompanyName, @PaymentTermId, @CreatedById, @OrganizationId, CURRENT_TIMESTAMP)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public BusinessEntity Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<BusinessEntity> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<BusinessEntity>> GetAllAsync()
      {
        IEnumerable<BusinessEntity> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<BusinessEntity>("""
            SELECT * FROM "BusinessEntity"
            """);
        }

        return result.ToList();
      }

      public async Task<(List<BusinessEntity> BusinessEntities, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<BusinessEntity> result;
        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<BusinessEntity>("""
            SELECT * FROM (
                SELECT *, ROW_NUMBER() OVER (ORDER BY "BusinessEntityID" DESC) AS RowNumber
                FROM "BusinessEntity"
                WHERE "OrganizationId" = @OrganizationId    
            ) AS NumberedBusinessEntities
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page
            """, p);
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        return (resultList, nextPageNumber);
      }

      public async Task<BusinessEntity> GetByIdAsync(int id, int organizationId)
      {
        var p = new DynamicParameters();
        p.Add("@BusinessEntityID", id);
        p.Add("@OrganizationId", organizationId);

        BusinessEntity? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QuerySingleOrDefaultAsync<BusinessEntity>("""
            SELECT * 
            FROM "BusinessEntity" 
            WHERE "BusinessEntityID" = @BusinessEntityID AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }

      public int Update(BusinessEntity entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAsync(int id, string? firstName, string? lastName, string? companyName, string? selectedCustomerType, string? businessEntityTypesCsv)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@BusinessEntityID", id);
        p.Add("@FirstName", firstName);
        p.Add("@LastName", lastName);
        p.Add("@CompanyName", companyName);
        p.Add("@CustomerType", selectedCustomerType);
        p.Add("@BusinessEntityTypesCsv", businessEntityTypesCsv);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "BusinessEntity" SET 
              "FirstName" = @FirstName,
              "LastName" = @LastName,
              "CompanyName" = @CompanyName,
              "CustomerType" = @CustomerType,
              "BusinessEntityTypesCsv" = @BusinessEntityTypesCsv
            WHERE "BusinessEntityID" = @BusinessEntityID
            """, p);
        }

        return rowsModified;
      }
    }

    public IChartOfAccountManager GetChartOfAccountManager()
    {
      return new ChartOfAccountManager();
    }

    public class ChartOfAccountManager : IChartOfAccountManager
    {
      public ChartOfAccount Create(ChartOfAccount entity)
      {
        throw new NotImplementedException();
      }

      public async Task<ChartOfAccount> CreateAsync(ChartOfAccount entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", entity.Name);
        p.Add("@Type", entity.Type);
        p.Add("@InvoiceCreationForCredit", entity.InvoiceCreationForCredit);
        p.Add("@InvoiceCreationForDebit", entity.InvoiceCreationForDebit);
        p.Add("@ReceiptOfPaymentForCredit", entity.ReceiptOfPaymentForCredit);
        p.Add("@ReceiptOfPaymentForDebit", entity.ReceiptOfPaymentForDebit);
        p.Add("@ParentChartOfAccountId", entity.ParentChartOfAccountId);
        p.Add("@ReconciliationExpense", entity.ReconciliationExpense);
        p.Add("@ReconciliationLiabilitiesAndAssets", entity.ReconciliationLiabilitiesAndAssets);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            INSERT INTO "ChartOfAccount" 
            ("Name", "Type", "InvoiceCreationForCredit", "InvoiceCreationForDebit", "ReceiptOfPaymentForCredit", "ReceiptOfPaymentForDebit", "ReconciliationExpense", "ReconciliationLiabilitiesAndAssets", "ParentChartOfAccountId", "CreatedById", "OrganizationId") 
            VALUES 
            (@Name, @Type, @InvoiceCreationForCredit, @InvoiceCreationForDebit, @ReceiptOfPaymentForCredit, @ReceiptOfPaymentForDebit, @ReconciliationExpense, @ReconciliationLiabilitiesAndAssets, @ParentChartOfAccountId, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<bool> ExistsAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ChartOfAccountID", id);
        p.Add("@OrganizationId", organizationId);

        bool result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.ExecuteScalarAsync<bool>("""
            SELECT CASE WHEN COUNT(*) > 0 THEN true ELSE false END
            FROM "ChartOfAccount" 
            WHERE "ChartOfAccountID" = @ChartOfAccountID AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }

      public ChartOfAccount Get(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<List<ChartOfAccount>> GetAccountBalanceReport(int organizationId)
      {
        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string query = $""" 
            SELECT 
                coa."ChartOfAccountID", 
                coa."Name", 
                coa."Type", 
                CASE 
                    WHEN coa."Type" IN ('assets', 'expense') THEN SUM(COALESCE(gl."Debit", 0)) - SUM(COALESCE(gl."Credit", 0))
                    ELSE SUM(COALESCE(gl."Credit", 0)) - SUM(COALESCE(gl."Debit", 0))
                END AS "CurrentBalance"
            FROM "ChartOfAccount" coa
            LEFT JOIN "GeneralLedger" gl ON coa."ChartOfAccountID" = gl."ChartOfAccountId" AND coa."OrganizationId" = gl."OrganizationId"
            WHERE coa."OrganizationId" = @OrganizationId
            GROUP BY coa."ChartOfAccountID", coa."Name", coa."Type"
        """;

          result = await con.QueryAsync<ChartOfAccount>(query, new { OrganizationId = organizationId });
          await con.CloseAsync();
        }
        
        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAccountOptionsForInvoiceCreationCredit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "InvoiceCreationForCredit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAccountOptionsForInvoiceCreationDebit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "InvoiceCreationForDebit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAccountOptionsForPaymentReceptionCredit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "ReceiptOfPaymentForCredit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAccountOptionsForPaymentReceptionDebit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "ReceiptOfPaymentForDebit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public IEnumerable<ChartOfAccount> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<ChartOfAccount>> GetAllAsync(int organizationId)
      {
        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "OrganizationId" = @OrganizationId
            """, new { OrganizationId = organizationId });
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAllAsync(string accountType, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Type", accountType);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "Type" = @Type
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAllReconciliationExpenseAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "ReconciliationExpense" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "ReconciliationLiabilitiesAndAssets" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ChartOfAccount>> GetAsync(string[] accountNames, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountNames", accountNames);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "Name" = ANY(@AccountNames)
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<ChartOfAccount> GetAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ChartOfAccountID", id);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "ChartOfAccountID" = @ChartOfAccountID
            """, p);
        }

        return result.Single();
      }

      public async Task<ChartOfAccount> GetAsync(string accountName, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", accountName);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "Name" = @Name
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.Single();
      }

      public async Task<ChartOfAccount> GetAsync(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<ChartOfAccount> GetByAccountNameAsync(string accountName, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", accountName);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ChartOfAccount> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ChartOfAccount>("""
            SELECT * 
            FROM "ChartOfAccount" 
            WHERE "Name" = @Name
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public int Update(ChartOfAccount entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAsync(ChartOfAccount chartOfAccount)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ChartOfAccountID", chartOfAccount.ChartOfAccountID);
        p.Add("@Name", chartOfAccount.Name);
        p.Add("@Type", chartOfAccount.Type);
        p.Add("@InvoiceCreationForCredit", chartOfAccount.InvoiceCreationForCredit);
        p.Add("@InvoiceCreationForDebit", chartOfAccount.InvoiceCreationForDebit);
        p.Add("@ReceiptOfPaymentForCredit", chartOfAccount.ReceiptOfPaymentForCredit);
        p.Add("@ReceiptOfPaymentForDebit", chartOfAccount.ReceiptOfPaymentForDebit);
        p.Add("@ReconciliationExpense", chartOfAccount.ReconciliationExpense);
        p.Add("@ReconciliationLiabilitiesAndAssets", chartOfAccount.ReconciliationLiabilitiesAndAssets);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "ChartOfAccount" SET 
            "Name" = @Name,
            "Type" = @Type,
            "InvoiceCreationForCredit" = @InvoiceCreationForCredit,
            "InvoiceCreationForDebit" = @InvoiceCreationForDebit,
            "ReceiptOfPaymentForCredit" = @ReceiptOfPaymentForCredit,
            "ReceiptOfPaymentForDebit" = @ReceiptOfPaymentForDebit,
            "ReconciliationExpense" = @ReconciliationExpense,
            "ReconciliationLiabilitiesAndAssets" = @ReconciliationLiabilitiesAndAssets
            WHERE "ChartOfAccountID" = @ChartOfAccountID
            """, p);
        }

        return rowsModified;
      }
    }

    //public IGeneralLedgerInvoiceManager GetGeneralLedgerInvoiceManager()
    //{
    //  return new GeneralLedgerInvoiceManager();
    //}

    //public class GeneralLedgerInvoiceManager : IGeneralLedgerInvoiceManager
    //{
    //  public GeneralLedgerInvoice Create(GeneralLedgerInvoice entity)
    //  {
    //    throw new NotImplementedException();
    //  }

    //  public async Task<GeneralLedgerInvoice> CreateAsync(GeneralLedgerInvoice entity)
    //  {
    //    DynamicParameters p = new DynamicParameters();
    //    p.Add("@GeneralLedgerId", entity.GeneralLedgerId);
    //    p.Add("@InvoiceId", entity.InvoiceId);
    //    p.Add("@ReversedGeneralLedgerInvoiceId", entity.ReversedGeneralLedgerInvoiceId);
    //    p.Add("@TransactionGuid", entity.TransactionGuid);
    //    p.Add("@OrganizationId", entity.OrganizationId);
    //    p.Add("@CreatedById", entity.CreatedById);

    //    IEnumerable<GeneralLedgerInvoice> result;

    //    using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
    //    {
    //      result = await con.QueryAsync<GeneralLedgerInvoice>("""
    //        INSERT INTO "GeneralLedgerInvoice" 
    //        ("GeneralLedgerId", "InvoiceId", "ReversedGeneralLedgerInvoiceId", "TransactionGuid", "OrganizationId", "CreatedById") 
    //        VALUES 
    //        (@GeneralLedgerId, @InvoiceId, @ReversedGeneralLedgerInvoiceId, @TransactionGuid, @OrganizationId, @CreatedById)
    //        RETURNING *;
    //        """, p);
    //    }

    //    return result.Single();
    //  }

    //  public int Delete(int id)
    //  {
    //    throw new NotImplementedException();
    //  }

    //  public GeneralLedgerInvoice Get(int id)
    //  {
    //    throw new NotImplementedException();
    //  }

    //  public IEnumerable<GeneralLedgerInvoice> GetAll()
    //  {
    //    throw new NotImplementedException();
    //  }

    //  public async Task<List<GeneralLedgerInvoice>> GetAllAsync(int invoiceId, bool getReversedEntries, int organizationId)
    //  {
    //    DynamicParameters p = new DynamicParameters();
    //    p.Add("@InvoiceId", invoiceId);
    //    p.Add("@OrganizationId", organizationId);

    //    string query = """
    //    SELECT * FROM "GeneralLedgerInvoice" 
    //    WHERE "InvoiceId" = @InvoiceId 
    //    AND "TransactionGuid" = (
    //        SELECT "TransactionGuid" 
    //        FROM "GeneralLedgerInvoice" 
    //        WHERE "InvoiceId" = @InvoiceId
    //        AND "OrganizationId" = @OrganizationId
    //        ORDER BY "Created" DESC
    //        LIMIT 1)
    //    """;

    //    if (getReversedEntries)
    //    {
    //      query += " AND \"ReversedGeneralLedgerInvoiceId\" IS NOT NULL";
    //    }
    //    else
    //    {
    //      query += " AND \"ReversedGeneralLedgerInvoiceId\" IS NULL";
    //    }

    //    IEnumerable<GeneralLedgerInvoice> result;

    //    using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
    //    {
    //      result = await con.QueryAsync<GeneralLedgerInvoice>(query, p);
    //    }

    //    return result.ToList();
    //  }

    //  public async Task<List<GeneralLedgerInvoice>> GetLastTransaction(
    //    int invoiceId,
    //    int organizationId,
    //    bool loadChildren)
    //  {
    //    DynamicParameters p = new DynamicParameters();
    //    p.Add("@InvoiceId", invoiceId);
    //    p.Add("@OrganizationId", organizationId);

    //    IEnumerable<GeneralLedgerInvoice> result;

    //    if (loadChildren)
    //    {
    //      using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
    //      {
    //        result = await con.QueryAsync<GeneralLedgerInvoice, GeneralLedger, GeneralLedgerInvoice>($"""
    //        SELECT gli.*, gl.*
    //        FROM "GeneralLedgerInvoice" gli
    //        INNER JOIN "GeneralLedger" gl ON gli."GeneralLedgerId" = gl."GeneralLedgerID"
    //        WHERE gli."InvoiceId" = @InvoiceId
    //        AND "TransactionGuid" = (
    //            SELECT "TransactionGuid" 
    //            FROM "GeneralLedgerInvoice" 
    //            WHERE "InvoiceId" = @InvoiceId
    //            AND "OrganizationId" = @OrganizationId
    //            ORDER BY "GeneralLedgerInvoiceID" DESC
    //            LIMIT 1
    //        )
    //        """, (gli, gl) =>
    //        {
    //          gli.GeneralLedger = gl;
    //          return gli;
    //        }, p, splitOn: "GeneralLedgerID");
    //      }
    //    }
    //    else
    //    {
    //      using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
    //      {
    //        result = await con.QueryAsync<GeneralLedgerInvoice>("""
    //        SELECT * FROM "GeneralLedgerInvoice" 
    //        WHERE "InvoiceId" = @InvoiceId 
    //        AND "TransactionGuid" = (
    //            SELECT "TransactionGuid" 
    //            FROM "GeneralLedgerInvoice" 
    //            WHERE "InvoiceId" = @InvoiceId
    //            AND "OrganizationId" = @OrganizationId
    //            ORDER BY "GeneralLedgerInvoiceID" DESC
    //            LIMIT 1
    //        )
    //        """, p);
    //      }
    //    }
    //    return result.ToList();
    //  }

    //  public int Update(GeneralLedgerInvoice entity)
    //  {
    //    throw new NotImplementedException();
    //  }
    //}

    public IGeneralLedgerInvoiceInvoiceLinePaymentManager GetGeneralLedgerInvoiceInvoiceLinePaymentManager()
    {
      return new GeneralLedgerInvoiceInvoiceLinePaymentManager();
    }

    public class GeneralLedgerInvoiceInvoiceLinePaymentManager : IGeneralLedgerInvoiceInvoiceLinePaymentManager
    {
      public GeneralLedgerInvoiceInvoiceLinePayment Create(GeneralLedgerInvoiceInvoiceLinePayment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<GeneralLedgerInvoiceInvoiceLinePayment> CreateAsync(GeneralLedgerInvoiceInvoiceLinePayment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@GeneralLedgerId", entity.GeneralLedgerId);
        p.Add("@InvoiceInvoiceLinePaymentId", entity.InvoiceInvoiceLinePaymentId);
        p.Add("@ReversedGeneralLedgerInvoiceInvoiceLinePaymentId", entity.ReversedGeneralLedgerInvoiceInvoiceLinePaymentId);
        p.Add("@TransactionGuid", entity.TransactionGuid);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@CreatedById", entity.CreatedById);

        IEnumerable<GeneralLedgerInvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string insertQuery = """
            INSERT INTO "GeneralLedgerInvoiceInvoiceLinePayment" ("GeneralLedgerId", "InvoiceInvoiceLinePaymentId", "ReversedGeneralLedgerInvoiceInvoiceLinePaymentId", "TransactionGuid", "CreatedById", "OrganizationId") 
            VALUES (@GeneralLedgerId, @InvoiceInvoiceLinePaymentId, @ReversedGeneralLedgerInvoiceInvoiceLinePaymentId, @TransactionGuid, @CreatedById, @OrganizationId)
            RETURNING *;
            """;

          result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLinePayment>(insertQuery, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public GeneralLedgerInvoiceInvoiceLinePayment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<GeneralLedgerInvoiceInvoiceLinePayment> GetAll()
      {
        throw new NotImplementedException();
      }

      public Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries)
      {
        throw new NotImplementedException();
      }

      public async Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoicePaymentId", invoicePaymentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<GeneralLedgerInvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLinePayment>($"""
            SELECT * 
            FROM "GeneralLedgerInvoicePayment" 
            WHERE "InvoicePaymentId" = @InvoicePaymentId 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<GeneralLedgerInvoiceInvoiceLinePayment>?> GetAllByInvoiceIdAsync(int invoiceId, int organizationId, bool includeReversedEntries = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<GeneralLedgerInvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string query = $"""
            SELECT * 
            FROM "GeneralLedgerInvoiceInvoiceLinePayment" 
            WHERE "InvoiceInvoiceLinePaymentId" = @InvoiceId 
            AND "OrganizationId" = @OrganizationId
            """;

          if (!includeReversedEntries)
          {
            query += " AND \"ReversedGeneralLedgerInvoiceInvoiceLinePaymentId\" IS NULL";
          }

          result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLinePayment>(query, p);
        }

        return result.ToList();
      }

      public async Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllByPaymentIdAsync(int paymentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentId", paymentId);
        p.Add("@OrganizationId", organizationId);

        List<GeneralLedgerInvoiceInvoiceLinePayment> result = new List<GeneralLedgerInvoiceInvoiceLinePayment>();

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          var query = $"""
            SELECT glilp.*, gl.*
            FROM "GeneralLedgerInvoiceInvoiceLinePayment" glilp
            JOIN "InvoiceInvoiceLinePayment" ilp ON glilp."InvoiceInvoiceLinePaymentId" = ilp."InvoiceInvoiceLinePaymentID"
            JOIN "GeneralLedger" gl ON glilp."GeneralLedgerId" = gl."GeneralLedgerID"
            WHERE glilp."TransactionGuid" = (
                SELECT "TransactionGuid" 
                FROM "GeneralLedgerInvoiceInvoiceLinePayment" 
                WHERE "PaymentId" = @PaymentId
                AND "OrganizationId" = @OrganizationId
                ORDER BY "GeneralLedgerInvoiceInvoiceLinePaymentID" DESC
                LIMIT 1
            ) AND glilp."ReversedGeneralLedgerInvoiceInvoiceLinePaymentId" IS NULL
            """;

          var invoicePayments = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLinePayment, GeneralLedger, GeneralLedgerInvoiceInvoiceLinePayment>(
              query,
              (glilp, gl) =>
              {
                glilp.GeneralLedger = gl;
                return glilp;
              },
              splitOn: "GeneralLedgerID",
              param: p
          );

          result = invoicePayments.ToList();
        }

        return result;
      }

      public async Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetLastTransactionsAsync(int paymentId, int organizationId, bool loadChildren)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentId", paymentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<GeneralLedgerInvoiceInvoiceLinePayment> result;

        if (loadChildren)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            string query = """
                SELECT giiilp.*, gl.*
                FROM "GeneralLedgerInvoiceInvoiceLinePayment" giiilp
                JOIN "GeneralLedger" gl ON giiilp."GeneralLedgerId" = gl."GeneralLedgerID"
                WHERE giiilp."TransactionGuid" = (
                  SELECT "TransactionGuid"
                  FROM "GeneralLedgerInvoiceInvoiceLinePayment"
                  WHERE "InvoiceInvoiceLinePaymentId" IN (
                    SELECT "InvoiceInvoiceLinePaymentID"
                    FROM "InvoiceInvoiceLinePayment"
                    WHERE "PaymentId" = @PaymentId
                  )
                  ORDER BY "GeneralLedgerInvoiceInvoiceLinePaymentID" DESC
                  LIMIT 1
                )
                AND giiilp."ReversedGeneralLedgerInvoiceInvoiceLinePaymentId" IS NULL
                AND giiilp."OrganizationId" = @OrganizationId
                """;

            result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLinePayment, GeneralLedger, GeneralLedgerInvoiceInvoiceLinePayment>(query, (giiilp, gl) =>
            {
              giiilp.GeneralLedger = gl;
              return giiilp;
            }, p, splitOn: "GeneralLedgerID");
          }
        }
        else
        {
          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            string query = """
                SELECT *
                FROM "GeneralLedgerInvoiceInvoiceLinePayment"
                WHERE "TransactionGuid" = (
                  SELECT "TransactionGuid"
                  FROM "GeneralLedgerInvoiceInvoiceLinePayment"
                  WHERE "InvoiceInvoiceLinePaymentId" IN (
                    SELECT "InvoiceInvoiceLinePaymentID"
                    FROM "InvoiceInvoiceLinePayment"
                    WHERE "PaymentId" = @PaymentId
                  )
                  ORDER BY "GeneralLedgerInvoiceInvoiceLinePaymentID" DESC
                  LIMIT 1
                )
                AND "ReversedGeneralLedgerInvoiceInvoiceLinePaymentId" IS NULL
                AND "OrganizationId" = @OrganizationId
                """;

            result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLinePayment>(query, p);
          }
        }

        return result.ToList();
      }

      public int Update(GeneralLedgerInvoiceInvoiceLinePayment entity)
      {
        throw new NotImplementedException();
      }
    }

    public IGeneralLedgerManager GetGeneralLedgerManager()
    {
      return new GeneralLedgerManager();
    }

    public class GeneralLedgerManager : IGeneralLedgerManager
    {
      public GeneralLedger Create(GeneralLedger entity)
      {
        throw new NotImplementedException();
      }

      public async Task<GeneralLedger> CreateAsync(GeneralLedger generalLedger)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ChartOfAccountId", generalLedger.ChartOfAccountId);
        p.Add("@Debit", generalLedger.Debit);
        p.Add("@Credit", generalLedger.Credit);
        p.Add("@Memo", generalLedger.Memo);
        p.Add("@CreatedById", generalLedger.CreatedById);
        p.Add("@OrganizationId", generalLedger.OrganizationId);

        IEnumerable<GeneralLedger> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<GeneralLedger>("""
            INSERT INTO "GeneralLedger" 
            ("ChartOfAccountId", "Debit", "Credit", "Memo", "CreatedById", "OrganizationId") 
            VALUES 
            (@ChartOfAccountId, @Debit, @Credit, @Memo, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public GeneralLedger Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<GeneralLedger> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<GeneralLedger> GetAsync(int generalLedgerId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@GeneralLedgerId", generalLedgerId);
        p.Add("@OrganizationId", organizationId);

        GeneralLedger? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QuerySingleOrDefaultAsync<GeneralLedger>("""
            SELECT * 
            FROM "GeneralLedger" 
            WHERE "GeneralLedgerID" = @GeneralLedgerId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }


      public async Task<List<GeneralLedger>> GetLedgerEntriesAsync(int[] generalLedgerIds, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@GeneralLedgerIds", generalLedgerIds);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<GeneralLedger> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<GeneralLedger>("""
            SELECT * 
            FROM "GeneralLedger" 
            WHERE "GeneralLedgerID" = ANY(@GeneralLedgerIds)
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }


      public int Update(GeneralLedger entity)
      {
        throw new NotImplementedException();
      }
    }

    public IInvitationManager GetInvitationManager()
    {
      return new InvitationManager();
    }

    public class InvitationManager : IInvitationManager
    {
      public Invitation Create(Invitation entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Invitation> CreateAsync(Invitation entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Guid", entity.Guid);
        p.Add("@Email", entity.Email);
        p.Add("@FirstName", entity.FirstName);
        p.Add("@LastName", entity.LastName);
        p.Add("@UserId", entity.UserId);
        p.Add("@Expiration", entity.Expiration);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Invitation> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invitation>(
              """
            INSERT INTO "Invitation" ("Guid", "Email", "FirstName", "LastName", "UserId", "Expiration", "CreatedById", "OrganizationId")
            VALUES (@Guid, @Email, @FirstName, @LastName, @UserId, @Expiration, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(Guid guid)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Guid", guid);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync(
              """
            DELETE FROM "Invitation"
            WHERE "Guid" = @Guid;
            """, p);
        }

        return rowsAffected;
      }


      public Invitation Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Invitation> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<Invitation> GetAsync(Guid guid)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Guid", guid);

        IEnumerable<Invitation> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invitation>(
              """
            SELECT * FROM "Invitation"
            WHERE "Guid" = @Guid;
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(Invitation entity)
      {
        throw new NotImplementedException();
      }
    }

    public IInvoiceAttachmentManager GetInvoiceAttachmentManager()
    {
      return new InvoiceAttachmentManager();
    }

    public class InvoiceAttachmentManager : IInvoiceAttachmentManager
    {
      public InvoiceAttachment Create(InvoiceAttachment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<InvoiceAttachment> CreateAsync(InvoiceAttachment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", entity.InvoiceId);
        p.Add("@FileName", entity.FileName);
        p.Add("@StoredFileName", entity.StoredFileName);
        p.Add("@FilePath", entity.FilePath);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<InvoiceAttachment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceAttachment>("""
            INSERT INTO "InvoiceAttachment" 
            ("InvoiceId", "FileName", "StoredFileName", "FilePath", "CreatedById", "OrganizationId") 
            VALUES 
            (@InvoiceId, @FileName, @StoredFileName, @FilePath, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public InvoiceAttachment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<InvoiceAttachment> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<InvoiceAttachment>> GetAllAsync(int[] ids, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Ids", ids);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceAttachment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceAttachment>("""
            SELECT * 
            FROM "InvoiceAttachment" 
            WHERE "InvoiceAttachmentID" = ANY(@Ids) AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<InvoiceAttachment>> GetAllAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceAttachment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceAttachment>("""
            SELECT * 
            FROM "InvoiceAttachment" 
            WHERE "InvoiceId" = @InvoiceId AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }


      public int Update(InvoiceAttachment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateFilePathAsync(int id, string newPath, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@FilePathID", id);
        p.Add("@FilePath", newPath);
        p.Add("@OrganizationId", organizationId);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "InvoiceAttachment" 
            SET "FilePath" = @FilePath 
            WHERE "FilePathID" = @FilePathID 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsModified;
      }

      public async Task<int> UpdateInvoiceIdAsync(int invoiceAttachmentId, int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceAttachmentId", invoiceAttachmentId);
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "InvoiceAttachment"
            SET "InvoiceId" = @InvoiceId
            WHERE "InvoiceAttachmentID" = @InvoiceAttachmentId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdatePrintOrderAsync(int id, int newPrintOrder, int userId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceAttachmentID", id);
        p.Add("@NewPrintOrder", newPrintOrder);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "InvoiceAttachment"
            SET "PrintOrder" = @NewPrintOrder
            WHERE "InvoiceAttachmentID" = @InvoiceAttachmentID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }
    }

    public IInvoiceLineManager GetInvoiceLineManager()
    {
      return new InvoiceLineManager();
    }

    public class InvoiceLineManager : IInvoiceLineManager
    {
      public InvoiceLine Create(InvoiceLine entity)
      {
        throw new NotImplementedException();
      }

      public async Task<InvoiceLine> CreateAsync(InvoiceLine entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Title", entity.Title);
        p.Add("@Description", entity.Description);
        p.Add("@Quantity", entity.Quantity);
        p.Add("@Price", entity.Price);
        p.Add("@InvoiceId", entity.InvoiceId);
        p.Add("@RevenueChartOfAccountId", entity.RevenueChartOfAccountId);
        p.Add("@AssetsChartOfAccountId", entity.AssetsChartOfAccountId);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<InvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceLine>("""
            INSERT INTO "InvoiceLine" 
            ("Title", "Description", "Quantity", "Price", "RevenueChartOfAccountId", "AssetsChartOfAccountId", "CreatedById", "OrganizationId", "InvoiceId")
            VALUES 
            (@Title, @Description, @Quantity, @Price, @RevenueChartOfAccountId, @AssetsChartOfAccountId, @CreatedById, @OrganizationId, @InvoiceId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteByInvoiceIdAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "InvoiceLine"
            WHERE "InvoiceId" = @InvoiceId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public InvoiceLine Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<InvoiceLine> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<InvoiceLine>> GetByInvoiceId(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        List<InvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = (await con.QueryAsync<InvoiceLine>("""
            SELECT * 
            FROM "InvoiceLine"
            WHERE "InvoiceId" = @InvoiceId
            AND "OrganizationId" = @OrganizationId
            """, p)).ToList();
        }

        return result ?? new List<InvoiceLine>();
      }

      public int Update(InvoiceLine entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAsync(InvoiceLine line, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceLineID", line.InvoiceLineID);
        p.Add("@Title", line.Title);
        p.Add("@Description", line.Description);
        p.Add("@Quantity", line.Quantity);
        p.Add("@Price", line.Price);
        p.Add("@OrganizationId", organizationId);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
             UPDATE "InvoiceLine"
             SET "Title" = @Title, "Description" = @Description, "Quantity" = @Quantity, "Price" = @Price
             WHERE "InvoiceLineID" = @InvoiceLineID
             AND "OrganizationId" = @OrganizationId
             """, p);
        }

        return rowsModified;
      }

      public async Task<int> UpdateTitleAndDescription(List<InvoiceLine> invoiceLines, int invoiceID, int userId, int organizationId)
      {
        int rowsModified = 0;

        foreach (var line in invoiceLines)
        {
          DynamicParameters p = new DynamicParameters();
          p.Add("@InvoiceLineID", line.InvoiceLineID);
          p.Add("@Title", line.Title);
          p.Add("@Description", line.Description);
          p.Add("@OrganizationId", organizationId);

          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            rowsModified += await con.ExecuteAsync("""
              UPDATE "InvoiceLine"
              SET 
              "Title" = @Title,
              "Description" = @Description
              WHERE "InvoiceLineID" = @InvoiceLineID
              AND "OrganizationId" = @OrganizationId
              """, p);
          }
        }

        return rowsModified;
      }
    }

    public IInvoiceManager GetInvoiceManager()
    {
      return new InvoiceManager();
    }

    public class InvoiceManager : IInvoiceManager
    {
      public async Task<string> CalculateInvoiceStatusAsync(int invoiceId, int organizationId)
      {
        decimal invoiceTotal = 0;
        decimal receivedAmount = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          DynamicParameters p = new DynamicParameters();
          p.Add("@InvoiceId", invoiceId);
          p.Add("@OrganizationId", organizationId);

          invoiceTotal = await con.ExecuteScalarAsync<decimal>("""
            SELECT SUM("Quantity" * "Price")
            FROM "InvoiceLine"
            WHERE "InvoiceId" = @InvoiceId 
            AND "OrganizationId" = @OrganizationId
            AND NOT EXISTS (
                SELECT 1
                FROM "GeneralLedgerInvoiceInvoiceLine" AS GLIIL
                JOIN (
                    SELECT "InvoiceLineId", MAX("GeneralLedgerInvoiceInvoiceLineID") AS MaxGLIILId
                    FROM "GeneralLedgerInvoiceInvoiceLine"
                    GROUP BY "InvoiceLineId"
                ) AS LatestGLIIL ON GLIIL."InvoiceLineId" = LatestGLIIL."InvoiceLineId" 
                                 AND GLIIL."GeneralLedgerInvoiceInvoiceLineID" = LatestGLIIL.MaxGLIILId
                WHERE GLIIL."InvoiceLineId" = "InvoiceLine"."InvoiceLineID"
                AND GLIIL."ReversedGeneralLedgerInvoiceInvoiceLineId" IS NOT NULL
            )
            """, p);

          receivedAmount = await con.ExecuteScalarAsync<decimal>("""
            SELECT COALESCE(SUM(iilp."Amount"), 0)
            FROM "InvoiceInvoiceLinePayment" iilp
            JOIN "Payment" p ON iilp."PaymentId" = p."PaymentID"
            WHERE iilp."InvoiceId" = @InvoiceId 
            AND iilp."OrganizationId" = @OrganizationId
            AND p."VoidReason" IS NULL;
            """, p);
        }

        if (receivedAmount == 0)
        {
          return InvoiceStatusConstants.Unpaid;
        }
        else if (receivedAmount < invoiceTotal)
        {
          return InvoiceStatusConstants.PartiallyPaid;
        }
        else
        {
          return InvoiceStatusConstants.Paid;
        }
      }

      public async Task<int> ComputeAndUpdateTotalAmountAndReceivedAmount(int invoiceId, int organizationId)
      {
        int affectedRows = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string totalAmountQuery = """
            SELECT SUM("Quantity" * "Price")
            FROM "InvoiceLine"
            WHERE "InvoiceId" = @InvoiceId 
            AND "OrganizationId" = @OrganizationId
            AND NOT EXISTS (
                SELECT 1
                FROM "GeneralLedgerInvoiceInvoiceLine" AS GLIIL
                JOIN (
                    SELECT "InvoiceLineId", MAX("GeneralLedgerInvoiceInvoiceLineID") AS MaxGLIILId
                    FROM "GeneralLedgerInvoiceInvoiceLine"
                    GROUP BY "InvoiceLineId"
                ) AS LatestGLIIL ON GLIIL."InvoiceLineId" = LatestGLIIL."InvoiceLineId" 
                                 AND GLIIL."GeneralLedgerInvoiceInvoiceLineID" = LatestGLIIL.MaxGLIILId
                WHERE GLIIL."InvoiceLineId" = "InvoiceLine"."InvoiceLineID"
                AND GLIIL."ReversedGeneralLedgerInvoiceInvoiceLineId" IS NOT NULL
            )
            """;

          decimal totalAmount = await con.QuerySingleOrDefaultAsync<decimal>(
              totalAmountQuery,
              new { InvoiceId = invoiceId, OrganizationId = organizationId }
          );

          string receivedAmountQuery = """
            SELECT COALESCE(SUM(iilp."Amount"), 0)
            FROM "InvoiceInvoiceLinePayment" iilp
            JOIN "Payment" p ON iilp."PaymentId" = p."PaymentID"
            WHERE iilp."InvoiceId" = @InvoiceId 
            AND iilp."OrganizationId" = @OrganizationId
            AND p."VoidReason" IS NULL;
            """;

          decimal receivedAmount = await con.QuerySingleOrDefaultAsync<decimal>(
              receivedAmountQuery,
              new { InvoiceId = invoiceId, OrganizationId = organizationId }
          );

          string updateInvoiceQuery = """
            UPDATE "Invoice"
            SET "TotalAmount" = @TotalAmount, "ReceivedAmount" = @ReceivedAmount
            WHERE "InvoiceID" = @InvoiceId AND "OrganizationId" = @OrganizationId
            """;

          affectedRows = await con.ExecuteAsync(
              updateInvoiceQuery,
              new { TotalAmount = totalAmount, ReceivedAmount = receivedAmount, InvoiceId = invoiceId, OrganizationId = organizationId }
          );
        }

        return affectedRows;
      }

      public Invoice Create(Invoice entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Invoice> CreateAsync(Invoice entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@BusinessEntityId", entity.BusinessEntityId);
        p.Add("@BillingAddressJSON", entity.BillingAddressJSON);
        p.Add("@ShippingAddressJSON", entity.ShippingAddressJSON ?? "{}");
        p.Add("@DueDate", entity.DueDate);
        p.Add("@Status", InvoiceStatusConstants.Unpaid);
        p.Add("@PaymentInstructions", entity.PaymentInstructions);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@TotalAmount", entity.TotalAmount);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invoice>("""
            INSERT INTO "Invoice" (
                "InvoiceNumber", 
                "BusinessEntityId", 
                "BillingAddressJSON", 
                "ShippingAddressJSON",
                "DueDate", 
                "Status", 
                "PaymentInstructions",
                "CreatedById", 
                "OrganizationId", 
                "Created",  
                "LastUpdated", 
                "TotalAmount") 
            VALUES (
                nextval('"InvoiceNumberSeq"'), 
                @BusinessEntityId, 
                @BillingAddressJSON, 
                @ShippingAddressJSON,
                @DueDate, 
                @Status, 
                @PaymentInstructions,
                @CreatedById, 
                @OrganizationId, 
                CURRENT_TIMESTAMP,
                CURRENT_TIMESTAMP,
                @TotalAmount)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<bool> ExistsAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", invoiceId);
        p.Add("@OrganizationId", organizationId);

        bool result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.ExecuteScalarAsync<bool>("""
            SELECT COUNT(*) > 0
            FROM "Invoice" 
            WHERE "InvoiceID" = @InvoiceID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }

      public Invoice Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Invoice> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<(List<Invoice> Invoices, int? NextPageNumber)> GetAllAsync(
        int page,
        int pageSize,
        string[] Statuses,
        int organizationId,
        bool includeVoidInvoices)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@Statuses", Statuses);
        p.Add("@OrganizationId", organizationId);

        string voidCondition = includeVoidInvoices ? "" : "AND (\"VoidReason\" IS NULL)";

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invoice>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "InvoiceID" DESC) AS RowNumber
                FROM "Invoice"
                WHERE "Status" = ANY(@Statuses)
                AND "OrganizationId" = @OrganizationId
                {voidCondition}
            ) AS NumberedInvoices
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        return (resultList, nextPageNumber);
      }

      public async Task<List<Invoice>> GetAllAsync(int organizationId, string[] inStatus)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Statuses", inStatus);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invoice>("""
            SELECT * FROM "Invoice" 
            WHERE "Status" = ANY(@Statuses) 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<Invoice> GetAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", id);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invoice>("""
            SELECT * 
            FROM "Invoice" 
            WHERE "InvoiceID" = @InvoiceID 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public async Task<List<Invoice>> GetAsync(string invoiceIdsCsv, int organizationId)
      {
        IEnumerable<Invoice> result;
        var invoiceIds = invoiceIdsCsv.Split(',').Select(id => int.Parse(id.Trim())).ToArray();

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invoice>("""
            SELECT * FROM "Invoice" 
            WHERE "InvoiceID" = ANY(@InvoiceIds) 
            AND "OrganizationId" = @OrganizationId
            """, new { InvoiceIds = invoiceIds, OrganizationId = organizationId });
        }

        return result.ToList();
      }

      public async Task<DateTime> GetLastUpdatedAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", invoiceId);
        p.Add("@OrganizationId", organizationId);

        DateTime result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.ExecuteScalarAsync<DateTime>("""
            SELECT "LastUpdated" 
            FROM "Invoice" 
            WHERE "InvoiceID" = @InvoiceID AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }

      public async Task<(decimal unpaid, decimal paid)> GetUnpaidAndPaidBalance(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        decimal unpaid = 0.0M;
        decimal paid = 0.0M;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          await con.OpenAsync();

          string unpaidQuery = """
            SELECT COALESCE(SUM(TotalAmount - ReceivedAmount), 0)
            FROM "Invoice"
            WHERE "OrganizationId" = @OrganizationId
            AND "Status" != @PaidStatus
            """;

          unpaid = await con.QuerySingleOrDefaultAsync<decimal>(unpaidQuery, p);

          string paidQuery = """
            SELECT COALESCE(SUM(ReceivedAmount), 0)
            FROM "Invoice"
            WHERE "OrganizationId" = @OrganizationId
            AND "Status" = @PaidStatus
            """;

          paid = await con.QuerySingleOrDefaultAsync<decimal>(paidQuery, p);
        }

        return (unpaid, paid);
      }

      public async Task<bool> IsVoidAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", invoiceId);
        p.Add("@OrganizationId", organizationId);

        string? voidReason;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          voidReason = await con.ExecuteScalarAsync<string>("""
            SELECT "VoidReason" 
            FROM "Invoice" 
            WHERE "InvoiceID" = @InvoiceID 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return !string.IsNullOrEmpty(voidReason);
      }
      public async Task<List<Invoice>> SearchInvoicesAsync(
        string[] inStatus,
        string invoiceNumbersSpaceSeparated,
        string company,
        int organizationId)
      {
        var invoiceNumbers = invoiceNumbersSpaceSeparated?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(n => int.TryParse(n, out var num) ? num : (int?)null)
                                .Where(n => n.HasValue)
                                .Select(n => n.Value) ?? Enumerable.Empty<int>();

        var companyParts = company?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        using (var con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          var sql = """
                SELECT i.* 
                FROM "Invoice" i
                JOIN "BusinessEntity" be ON i."BusinessEntityId" = be."BusinessEntityID"
                WHERE i."OrganizationId" = @OrganizationId
                """;

          if (inStatus != null && inStatus.Any())
          {
            sql += " AND i.\"Status\" = ANY(@Status)";
          }

          if (invoiceNumbers.Any())
          {
            sql += " AND i.\"InvoiceNumber\" = ANY(@InvoiceNumbers)";
          }

          if (companyParts.Any())
          {
            var companyFilters = companyParts.Select((_, idx) => $"(be.\"CompanyName\" ILIKE @CompanyPart{idx} OR be.\"FirstName\" ILIKE @CompanyPart{idx} OR be.\"LastName\" ILIKE @CompanyPart{idx})");
            sql += " AND (" + string.Join(" OR ", companyFilters) + ")";
          }

          var parameters = new DynamicParameters();
          parameters.Add("OrganizationId", organizationId);

          if (inStatus != null)
          {
            parameters.Add("Status", inStatus);
          }

          if (invoiceNumbers.Any())
          {
            parameters.Add("InvoiceNumbers", invoiceNumbers.ToArray());
          }

          foreach (var (part, idx) in companyParts.Select((part, idx) => (part, idx)))
          {
            parameters.Add($"CompanyPart{idx}", $"%{part}%");
          }

          return (await con.QueryAsync<Invoice>(sql, parameters)).ToList();
        }
      }

      public int Update(Invoice entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateLastUpdated(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", invoiceId);
        p.Add("@OrganizationId", organizationId);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "Invoice" 
            SET "LastUpdated" = CURRENT_TIMESTAMP 
            WHERE "InvoiceID" = @InvoiceID 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsModified;
      }

      public async Task<int> UpdatePaymentInstructions(int invoiceId, string? paymentInstructions, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@PaymentInstructions", paymentInstructions);
        p.Add("@OrganizationId", organizationId);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "Invoice"
            SET "PaymentInstructions" = @PaymentInstructions 
            WHERE "InvoiceID" = @InvoiceId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsModified;
      }

      public async Task<int> UpdateStatusAsync(int invoiceId, string status)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", invoiceId);
        p.Add("@Status", status);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "Invoice"
            SET "Status" = @Status 
            WHERE "InvoiceID" = @InvoiceID
            """, p);
        }

        return rowsModified;
      }

      public async Task<int> VoidAsync(int invoiceId, string? voidReason, int organizationId)
      {
        int rowsAffected = 0;
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceID", invoiceId);
        p.Add("@VoidReason", voidReason);
        p.Add("@Status", InvoiceStatusConstants.Void);
        p.Add("@OrganizationId", organizationId);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Invoice"
            SET 
            "VoidReason" = @VoidReason,
            "Status" = @Status
            WHERE "InvoiceID" = @InvoiceID AND "VoidReason" IS NULL
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }
    }

    public IInvoiceInvoiceLinePaymentManager GetInvoiceInvoiceLinePaymentManager()
    {
      return new InvoiceInvoiceLinePaymentManager();
    }

    public class InvoiceInvoiceLinePaymentManager : IInvoiceInvoiceLinePaymentManager
    {
      public InvoiceInvoiceLinePayment Create(InvoiceInvoiceLinePayment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<InvoiceInvoiceLinePayment> CreateAsync(InvoiceInvoiceLinePayment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", entity.InvoiceId);
        p.Add("@InvoiceLineId", entity.InvoiceLineId);
        p.Add("@PaymentId", entity.PaymentId);
        p.Add("@Amount", entity.Amount);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<InvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceInvoiceLinePayment>("""
            INSERT INTO "InvoiceInvoiceLinePayment" 
            ("InvoiceId", "InvoiceLineId", "PaymentId", "Amount", "CreatedById", "OrganizationId") 
            VALUES 
            (@InvoiceId, @InvoiceLineId, @PaymentId, @Amount, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public InvoiceInvoiceLinePayment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<InvoiceInvoiceLinePayment> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<(List<InvoiceInvoiceLinePayment> InvoicePayments, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId, List<string> typesToLoad = null)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceInvoiceLinePayment>("""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "InvoicePaymentID" DESC) AS RowNumber
                FROM "InvoicePayment"
                WHERE "OrganizationId" = @OrganizationId    
            ) AS NumberedInvoicePayments
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page
            """, p);
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          foreach (var invoicePayment in resultList)
          {
            if (typesToLoad?.Contains(TypesToLoadConstants.Invoice) == true)
            {
              var invoice = await con.QuerySingleOrDefaultAsync<Invoice>("""
                    SELECT * FROM "Invoice" 
                    WHERE "InvoiceID" = @InvoiceId
                    AND "OrganizationId" = @OrganizationId
                    """, new { invoicePayment.InvoiceId, OrganizationId = organizationId });
              invoicePayment.Invoice = invoice;
            }

            if (typesToLoad?.Contains(TypesToLoadConstants.Payment) == true)
            {
              var payment = await con.QuerySingleOrDefaultAsync<Payment>("""
                    SELECT * FROM "Payment" 
                    WHERE "PaymentID" = @PaymentId
                    AND "OrganizationId" = @OrganizationId
                    """, new { invoicePayment.PaymentId, OrganizationId = organizationId });
              invoicePayment.Payment = payment;
            }
          }
        }

        return (resultList, nextPageNumber);
      }

      public async Task<List<Invoice>> GetAllInvoicesByPaymentIdAsync(int paymentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentId", paymentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Invoice>("""
            SELECT i.* 
            FROM "Invoice" i
            JOIN "InvoiceInvoiceLinePayment" iilp ON i."InvoiceID" = iilp."InvoiceId"
            WHERE iilp."PaymentId" = @PaymentId
            AND i."OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<Payment>> GetAllPaymentsByInvoiceIdAsync(int invoiceId, int organizationId, bool includeVoid = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@IncludeVoid", includeVoid);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Payment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Payment>("""
            SELECT DISTINCT p.* 
            FROM "Payment" p
            JOIN "InvoiceInvoiceLinePayment" iilp ON p."PaymentID" = iilp."PaymentId"
            WHERE iilp."InvoiceId" = @InvoiceId
            AND p."OrganizationId" = @OrganizationId
            AND (@IncludeVoid = TRUE OR p."VoidReason" IS NULL)
            """, new { InvoiceId = invoiceId, OrganizationId = organizationId, IncludeVoid = includeVoid });
        }

        return result.ToList();
      }

      public async Task<InvoiceInvoiceLinePayment> GetInvoicePaymentAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoicePaymentID", id);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceInvoiceLinePayment>("""
            SELECT * 
            FROM "InvoicePayment" 
            WHERE "InvoicePaymentID" = @InvoicePaymentID AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<decimal> GetTotalReceivedAsync(int invoiceLineId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceLineId", invoiceLineId);
        p.Add("@OrganizationId", organizationId);

        decimal result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QuerySingleOrDefaultAsync<decimal>("""
            SELECT COALESCE(SUM(iilp."Amount"), 0) 
            FROM "InvoiceInvoiceLinePayment" iilp
            JOIN "Payment" p ON iilp."PaymentId" = p."PaymentID"
            WHERE iilp."InvoiceLineId" = @InvoiceLineId
            AND iilp."OrganizationId" = @OrganizationId
            AND p."VoidReason" IS NULL 
            """, p);
        }

        return result;
      }

      public async Task<List<InvoiceInvoiceLinePayment>> GetValidInvoicePaymentsAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InvoiceInvoiceLinePayment>("""
            SELECT * 
            FROM "InvoiceInvoiceLinePayment"
            WHERE "InvoiceId" = @InvoiceId
            AND "OrganizationId" = @OrganizationId
            AND "VoidReason" IS NULL
            """, p);
        }

        return result.ToList();
      }

      public async Task<(List<InvoiceInvoiceLinePayment> InvoicePayments, int? NextPageNumber)> SearchInvoicePaymentsAsync(
  int page,
  int pageSize,
  string customerSearchQuery,
  List<string> typesToLoad,
  int organizationId)
      {
        QueryBuilder builder = new QueryBuilder();

        builder.AddParameter("@Page", page);
        builder.AddParameter("@PageSize", pageSize);
        builder.AddParameter("@OrganizationId", organizationId);

        var conditions = new List<string>();
        if (!string.IsNullOrEmpty(customerSearchQuery))
        {
          var customerKeywords = customerSearchQuery.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToList();
          var columns = new[] { "\"be\".\"FirstName\"", "\"be\".\"LastName\"", "\"be\".\"CompanyName\"" };
          foreach (var keyword in customerKeywords)
          {
            foreach (var column in columns)
            {
              builder.AddSearchCondition(column, keyword);
            }
          }
        }

        string searchLogic = builder.BuildSearchLogic();

        IEnumerable<InvoiceInvoiceLinePayment> result;
        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string query = $"""
            SELECT * FROM (
                SELECT ip.*,
                    ROW_NUMBER() OVER (ORDER BY ip."InvoicePaymentID" DESC) AS RowNumber
                FROM "InvoicePayment" ip
                JOIN "Invoice" i ON ip."InvoiceId" = i."InvoiceID"
                JOIN "BusinessEntity" be ON i."BusinessEntityId" = be."BusinessEntityID"
                WHERE ip."OrganizationId" = @OrganizationId
                {searchLogic}
            ) AS NumberedInvoicePayments
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page
            """;

          result = await con.QueryAsync<InvoiceInvoiceLinePayment>(query, builder.Parameters);
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          foreach (var invoicePayment in resultList)
          {
            if (typesToLoad?.Contains(TypesToLoadConstants.Invoice) == true)
            {
              var invoice = await con.QuerySingleOrDefaultAsync<Invoice>("""
                    SELECT * FROM "Invoice" 
                    WHERE "InvoiceID" = @InvoiceId
                    AND "OrganizationId" = @OrganizationId
                    """, new { invoicePayment.InvoiceId, OrganizationId = organizationId });
              invoicePayment.Invoice = invoice;
            }

            if (typesToLoad?.Contains(TypesToLoadConstants.Payment) == true)
            {
              var payment = await con.QuerySingleOrDefaultAsync<Payment>("""
                    SELECT * FROM "Payment" 
                    WHERE "PaymentID" = @PaymentId
                    AND "OrganizationId" = @OrganizationId
                    """, new { invoicePayment.PaymentId, OrganizationId = organizationId });
              invoicePayment.Payment = payment;
            }
          }
        }

        return (resultList, nextPageNumber);
      }

      public int Update(InvoiceInvoiceLinePayment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> VoidAsync(int id, string voidReason, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoicePaymentID", id);
        p.Add("@VoidReason", voidReason);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "InvoicePayment"
            SET 
                "VoidReason" = @VoidReason,
                "Amount" = 0
            WHERE "InvoicePaymentID" = @InvoicePaymentID
            AND "VoidReason" IS NULL
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }
    }

    public IItemManager GetItemManager()
    {
      return new ItemManager();
    }

    public class ItemManager : IItemManager
    {
      public Item Create(Item entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Item> CreateAsync(Item entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", entity.Name);
        p.Add("@Description", entity.Description);
        p.Add("@InventoryMethod", entity.InventoryMethod);  
        p.Add("@ItemType", entity.ItemType);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@RevenueChartOfAccountId", entity.RevenueChartOfAccountId);
        p.Add("@AssetsChartOfAccountId", entity.AssetsChartOfAccountId);
        p.Add("@ParentItemId", entity.ParentItemId);  

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Item>("""
            INSERT INTO "Item" ("Name", "Description", "InventoryMethod", "ItemType", "RevenueChartOfAccountId", "AssetsChartOfAccountId", "CreatedById", "OrganizationId", "ParentItemId")
            VALUES (@Name, @Description, @InventoryMethod, @ItemType, @RevenueChartOfAccountId, @AssetsChartOfAccountId, @CreatedById, @OrganizationId, @ParentItemId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Item Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Item> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Item>> GetAllAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Item>("""
            SELECT * 
            FROM "Item" 
            WHERE "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<(List<Item> Items, int? NextPageNumber)> GetAllAsync(int page, int pageSize, bool loadChildren, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Item>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "ItemID" DESC) AS RowNumber
                FROM "Item"
                WHERE "OrganizationId" = @OrganizationId
            ) AS NumberedItems
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);

          if (loadChildren)
          {
            foreach (var item in result)
            {
              DynamicParameters p2 = new DynamicParameters();
              p2.Add("@ItemId", item.ItemID);
              p2.Add("@OrganizationId", organizationId);

              var children = await con.QueryAsync<Item>("""
                SELECT * 
                FROM "Item" 
                WHERE "ParentItemId" = @ItemId 
                AND "OrganizationId" = @OrganizationId
                """, p2);

              item.Children = children.ToList();
            }
          }
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        return (resultList, nextPageNumber);
      }

      public async Task<Item?> GetAsync(int itemId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemId", itemId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Item>("""
            SELECT * 
            FROM "Item" 
            WHERE "ItemID" = @ItemId 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.FirstOrDefault();
      }

      public async Task<List<Item>?> GetChildrenAsync(int itemId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemId", itemId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Item>("""
            SELECT * 
            FROM "Item" 
            WHERE "ParentItemId" = @ItemId 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public int Update(Item entity)
      {
        throw new NotImplementedException();
      }
    }

    public IOrganizationManager GetOrganizationManager()
    {
      return new OrganizationManager();
    }

    public class OrganizationManager : IOrganizationManager
    {
      public Organization Create(Organization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Organization> CreateAsync(Organization entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Organization Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Organization> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<Organization> GetAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Organization>? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Organization>("""
            SELECT * 
            FROM "Organization" 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public async Task<string?> GetPaymentInstructions(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        string? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.ExecuteScalarAsync<string>("""
            SELECT "PaymentInstructions" 
            FROM "Organization" 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return result;
      }

      public async Task<int> UpdateNameAsync(int organizationId, string name)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Name", name);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "Name" = @Name 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdatePaymentInstructions(int organizationId, string? paymentInstructions)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@PaymentInstructions", paymentInstructions);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "PaymentInstructions" = @PaymentInstructions 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public int Update(Organization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAddressAsync(int organizationId, string address)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Address", address);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "Address" = @Address 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateAccountsPayableEmailAsync(int organizationId, string accountsPayableEmail)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@AccountsPayableEmail", accountsPayableEmail);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          return await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "AccountsPayableEmail" = @AccountsPayableEmail 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }
      }

      public async Task<int> UpdateAccountsPayablePhoneAsync(int organizationId, string accountsPayablePhone)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@AccountsPayablePhone", accountsPayablePhone);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          return await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "AccountsPayablePhone" = @AccountsPayablePhone 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }
      }

      public async Task<int> UpdateAccountsReceivableEmailAsync(int organizationId, string accountsReceivableEmail)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@AccountsReceivableEmail", accountsReceivableEmail);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          return await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "AccountsReceivableEmail" = @AccountsReceivableEmail 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }
      }

      public async Task<int> UpdateAccountsReceivablePhoneAsync(int organizationId, string accountsReceivablePhone)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@AccountsReceivablePhone", accountsReceivablePhone);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          return await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "AccountsReceivablePhone" = @AccountsReceivablePhone 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }
      }

      public async Task<int> UpdateWebsiteAsync(int organizationId, string website)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Website", website);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          return await con.ExecuteAsync($"""
            UPDATE "Organization" 
            SET "Website" = @Website 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }
      }
    }

    public IPaymentInstructionManager GetPaymentInstructionManager()
    {
      return new PaymentInstructionManager();
    }

    public class PaymentInstructionManager : IPaymentInstructionManager
    {
      public PaymentInstruction Create(PaymentInstruction entity)
      {
        throw new NotImplementedException();
      }

      public async Task<PaymentInstruction> CreateAsync(PaymentInstruction entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("Title", entity.Title);
        p.Add("Content", entity.Content);
        p.Add("CreatedById", entity.CreatedById);
        p.Add("OrganizationId", entity.OrganizationId);

        PaymentInstruction? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryFirstOrDefaultAsync<PaymentInstruction>(@"
            INSERT INTO PaymentInstruction (Title, Content, CreatedById, OrganizationId) 
            VALUES (@Title, @Content, @CreatedById, @OrganizationId)
            RETURNING *", p);
        }

        return result!;
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public PaymentInstruction Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<PaymentInstruction> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<PaymentInstruction>> GetAllAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<PaymentInstruction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<PaymentInstruction>("""
            SELECT * FROM "PaymentInstruction"
            WHERE "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public int Update(PaymentInstruction entity)
      {
        throw new NotImplementedException();
      }
    }

    public IPaymentManager GetPaymentManager()
    {
      return new PaymentManager();
    }

    public class PaymentManager : IPaymentManager
    {
      public Payment Create(Payment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Payment> CreateAsync(Payment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReferenceNumber", entity.ReferenceNumber);
        p.Add("@Amount", entity.Amount);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Payment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Payment>("""
            INSERT INTO "Payment" ("ReferenceNumber", "Amount", "CreatedById", "OrganizationId")
            VALUES (@ReferenceNumber, @Amount, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Payment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Payment> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Payment>> GetAllByInvoiceIdAsync(int invoiceId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Payment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Payment>("""
            SELECT DISTINCT p.* 
            FROM "Payment" p
            JOIN "InvoiceInvoiceLinePayment" iilp ON p."PaymentID" = iilp."PaymentId"
            WHERE iilp."InvoiceId" = @InvoiceId
            AND iilp."OrganizationId" = @OrganizationId;
            """, p);
        }

        return result.ToList();
      }

      public async Task<Payment> GetAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentID", id);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Payment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Payment>("""
            SELECT * FROM "Payment" 
            WHERE "PaymentID" = @PaymentID 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(Payment entity)
      {
        throw new NotImplementedException();
      }

      public async Task UpdateVoidReasonAsync(int paymentId, string? voidReason, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentID", paymentId);
        p.Add("@VoidReason", voidReason);
        p.Add("@OrganizationId", organizationId);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          await con.ExecuteAsync("""
            UPDATE "Payment"
            SET 
                "VoidReason" = @VoidReason
            WHERE "PaymentID" = @PaymentID
            AND "VoidReason" IS NULL
            AND "OrganizationId" = @OrganizationId
            """, p);
        }
      }
    }

    public IPaymentTermManager GetPaymentTermManager()
    {
      return new PaymentTermManager();
    }

    public class PaymentTermManager : IPaymentTermManager
    {
      public PaymentTerm Create(PaymentTerm entity)
      {
        throw new NotImplementedException();
      }

      public async Task<PaymentTerm> CreateAsync(PaymentTerm entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Description", entity.Description);
        p.Add("@DaysUntilDue", entity.DaysUntilDue);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<PaymentTerm> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<PaymentTerm>("""
            INSERT INTO "PaymentTerm" ("Description", "DaysUntilDue", "CreatedById", "OrganizationId") 
            VALUES (@Description, @DaysUntilDue, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public PaymentTerm Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<PaymentTerm> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<PaymentTerm>> GetAllAsync()
      {
        IEnumerable<PaymentTerm> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<PaymentTerm>("""
            SELECT * FROM "PaymentTerm"
            """);
        }

        return result.ToList();
      }


      public async Task<PaymentTerm?> GetAsync(int paymentTermId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentTermID", paymentTermId);

        IEnumerable<PaymentTerm> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<PaymentTerm>("""
            SELECT * 
            FROM "PaymentTerm" 
            WHERE "PaymentTermID" = @PaymentTermID
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(PaymentTerm entity)
      {
        throw new NotImplementedException();
      }
    }

    public IReconciliationManager GetReconciliationManager()
    {
      return new ReconciliationManager();
    }

    public class ReconciliationManager : IReconciliationManager
    {
      public Reconciliation Create(Reconciliation entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Reconciliation> CreateAsync(Reconciliation entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Status", entity.Status);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Reconciliation> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Reconciliation>("""
            INSERT INTO "Reconciliation" 
            ("Status", "CreatedById", "OrganizationId") 
            VALUES 
            (@Status, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Reconciliation Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Reconciliation> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Reconciliation>> GetAllDescendingAsync(int top, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Reconciliation> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Reconciliation, ReconciliationAttachment, Reconciliation>($"""
            SELECT r.*, ra.*
            FROM "Reconciliation" r
            INNER JOIN "ReconciliationAttachment" ra ON r."ReconciliationID" = ra."ReconciliationId"
            WHERE r."OrganizationId" = @OrganizationId
            ORDER BY r."ReconciliationID" DESC
            LIMIT {top}
            """, (r, ra) =>
          {
            r.ReconciliationAttachment = ra;
            return r;
          }, p, splitOn: "ReconciliationAttachmentID");
        }

        return result.ToList();
      }

      public async Task<Reconciliation> GetByIdAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationID", id);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Reconciliation> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Reconciliation>("""
            SELECT * 
            FROM "Reconciliation" 
            WHERE "ReconciliationID" = @ReconciliationID 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.Single();
      }

      public int Update(Reconciliation entity)
      {
        throw new NotImplementedException();
      }
    }

    public IReconciliationTransactionManager GetReconciliationTransactionManager()
    {
      return new ReconciliationTransactionManager();
    }

    public class ReconciliationTransactionManager : IReconciliationTransactionManager
    {
      public ReconciliationTransaction Create(ReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }

      public Task<ReconciliationTransaction> CreateAsync(ReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public ReconciliationTransaction Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<ReconciliationTransaction> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<ReconciliationTransaction>> GetAllByIdAsync(int reconciliationId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationId", reconciliationId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ReconciliationTransaction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationTransaction>(
            """
            SELECT * 
            FROM "ReconciliationTransaction" 
            WHERE "ReconciliationId" = @ReconciliationId 
            AND "OrganizationId" = @OrganizationId;
            """, p);
        }

        return result.ToList();
      }

      public async Task<ReconciliationTransaction> GetAsync(int reconciliationTransactionID)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);

        IEnumerable<ReconciliationTransaction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationTransaction>(
            """
            SELECT * 
            FROM "ReconciliationTransaction" 
            WHERE "ReconciliationTransactionID" = @ReconciliationTransactionID;
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public async Task<(List<ReconciliationTransaction> ReconciliationTransactions, int? NextPageNumber)> GetReconciliationTransactionAsync(int reconciliationId, int page, int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationId", reconciliationId);
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);

        IEnumerable<ReconciliationTransaction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationTransaction>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "ReconciliationTransactionID" DESC) AS RowNumber
                FROM "ReconciliationTransaction"
                WHERE "ReconciliationId" = @ReconciliationId
            ) AS NumberedReconciliationTransactions
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        return (resultList, nextPageNumber);
      }

      public async Task<int> ImportAsync(List<ReconciliationTransaction> reconciliationTransactions)
      {
        int rowsAffected = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          foreach (var reconciliationTransaction in reconciliationTransactions)
          {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ReconciliationId", reconciliationTransaction.ReconciliationId);
            p.Add("@RawData", reconciliationTransaction.RawData);
            p.Add("@TransactionDate", reconciliationTransaction.TransactionDate);
            p.Add("@PostedDate", reconciliationTransaction.PostedDate);
            p.Add("@Description", reconciliationTransaction.Description);
            p.Add("@Amount", reconciliationTransaction.Amount);
            p.Add("@Category", reconciliationTransaction.Category);
            p.Add("@CreatedById", reconciliationTransaction.CreatedById);
            p.Add("@OrganizationId", reconciliationTransaction.OrganizationId);

            rowsAffected += await con.ExecuteAsync(
                """
                INSERT INTO "ReconciliationTransaction" 
                (
                    "ReconciliationId",    
                    "RawData", 
                    "TransactionDate", 
                    "PostedDate", 
                    "Description", 
                    "Amount", 
                    "Category", 
                    "CreatedById", 
                    "OrganizationId"
                ) 
                VALUES 
                (
                    @ReconciliationId,
                    @RawData, 
                    @TransactionDate, 
                    @PostedDate, 
                    @Description, 
                    @Amount, 
                    @Category, 
                    @CreatedById, 
                    @OrganizationId
                );
                """, p);
          }
        }

        return rowsAffected;
      }

      public async Task<int> UpdateReconciliationTransactionInstructionAsync(int reconciliationTransactionID, string reconciliationInstruction)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);
        p.Add("@ReconciliationInstruction", reconciliationInstruction);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync(
            """
            UPDATE "ReconciliationTransaction" 
            SET "ReconciliationInstruction" = @ReconciliationInstruction
            WHERE "ReconciliationTransactionID" = @ReconciliationTransactionID;
            """, p);
        }

        return rowsAffected;
      }

      public int Update(ReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAssetOrLiabilityChartOfAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationLiabilitiesAndAssetsAccountId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);
        p.Add("@AssetOrLiabilityChartOfAccountId", selectedReconciliationLiabilitiesAndAssetsAccountId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "ReconciliationTransaction" 
            SET "AssetOrLiabilityChartOfAccountId" = @AssetOrLiabilityChartOfAccountId
            WHERE "ReconciliationTransactionID" = @ReconciliationTransactionID;
            """, p);

          return rowsAffected;
        }
      }

      public async Task<int> UpdateExpenseChartOfAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationExpenseAccountId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);
        p.Add("@ExpenseChartOfAccountId", selectedReconciliationExpenseAccountId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "ReconciliationTransaction" 
            SET "ExpenseChartOfAccountId" = @ExpenseChartOfAccountId
            WHERE "ReconciliationTransactionID" = @ReconciliationTransactionID;
            """, p);

          return rowsAffected;
        }
      }
    }

    public IReconiliationAttachmentManager GetReconiliationAttachmentManager()
    {
      return new ReconiliationAttachmentManager();
    }

    public class ReconiliationAttachmentManager : IReconiliationAttachmentManager
    {
      public ReconciliationAttachment Create(ReconciliationAttachment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<ReconciliationAttachment> CreateAsync(
        ReconciliationAttachment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationId", entity.ReconciliationId);
        p.Add("@OriginalFileName", entity.OriginalFileName);
        p.Add("@FilePath", entity.FilePath);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<ReconciliationAttachment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationAttachment>(
            """
            INSERT INTO "ReconciliationAttachment" ("ReconciliationId", "OriginalFileName", "FilePath", "CreatedById", "OrganizationId") 
            VALUES (@ReconciliationId, @OriginalFileName, @FilePath, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public ReconciliationAttachment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<ReconciliationAttachment> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<ReconciliationAttachment> GetAsync(int reconciliationAttachmentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationAttachmentId", reconciliationAttachmentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ReconciliationAttachment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationAttachment>(
              """
            SELECT * 
            FROM "ReconciliationAttachment" 
            WHERE "ReconciliationAttachmentID" = @ReconciliationAttachmentId 
            AND "OrganizationId" = @OrganizationId;
            """, p);
        }

        return result.SingleOrDefault();
      }


      public async Task<ReconciliationAttachment> GetByReconciliationIdAsync(int reconciliationId, int organizationId)
      {
        ReconciliationAttachment? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          var p = new DynamicParameters();
          p.Add("@ReconciliationId", reconciliationId);
          p.Add("@OrganizationId", organizationId);

          result = await con.QueryFirstOrDefaultAsync<ReconciliationAttachment>("""
            SELECT * 
            FROM "ReconciliationAttachment" 
            WHERE "ReconciliationId" = @ReconciliationId 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result!;
      }

      public int Update(ReconciliationAttachment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateFilePathAsync(int id, string destinationPath, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationAttachmentID", id);
        p.Add("@FilePath", destinationPath);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync(
            """
            UPDATE "ReconciliationAttachment" 
            SET "FilePath" = @FilePath 
            WHERE "ReconciliationAttachmentID" = @ReconciliationAttachmentID 
            AND "OrganizationId" = @OrganizationId;
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateReconciliationIdAsync(int id, int reconciliationId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationAttachmentID", id);
        p.Add("@ReconciliationId", reconciliationId);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync(
            """
            UPDATE "ReconciliationAttachment" 
            SET "ReconciliationId" = @ReconciliationId 
            WHERE "ReconciliationAttachmentID" = @ReconciliationAttachmentID 
            AND "OrganizationId" = @OrganizationId;
            """, p);
        }

        return rowsAffected;
      }
    }

    public ITagManager GetTagManager()
    {
      return new TagManager();
    }

    public class TagManager : ITagManager
    {
      public Tag Create(Tag entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Tag> CreateAsync(Tag entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", entity.Name);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Tag> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Tag>("""
            INSERT INTO "Tag" ("Name", "CreatedById", "OrganizationId")
            VALUES (@Name, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Tag Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Tag> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Tag>> GetAllAsync()
      {
        IEnumerable<Tag> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Tag>("""
            SELECT * FROM "Tag"
            """);
        }

        return result.ToList();
      }

      public async Task<Tag> GetByNameAsync(string name)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", name);

        IEnumerable<Tag> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Tag>("""
            SELECT * 
            FROM "Tag" 
            WHERE "Name" = @Name
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(Tag entity)
      {
        throw new NotImplementedException();
      }
    }

    public IToDoManager GetTaskManager()
    {
      return new ToDoManager();
    }

    public class ToDoManager : IToDoManager
    {
      public ToDo Create(ToDo entity)
      {
        throw new NotImplementedException();
      }

      public async Task<ToDo> CreateAsync(ToDo entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Title", entity.Title);
        p.Add("@Content", entity.Content);
        p.Add("@ParentToDoId", entity.ParentToDoId);
        p.Add("@Status", entity.Status);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<ToDo> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ToDo>("""
            INSERT INTO "ToDo" 
            ("Title", "Content", "ParentToDoId", "Status", "CreatedById", "OrganizationId") 
            VALUES 
            (@Title, @Content, @ParentToDoId, @Status, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public ToDo Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<ToDo> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<ToDo>> GetAllAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        List<ToDo> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = (await con.QueryAsync<ToDo>("""
            SELECT "ToDoID", "Title", "Content", "ParentToDoId", "Status", "Created", "CreatedById", "OrganizationId"
            FROM "ToDo"
            WHERE "OrganizationId" = @OrganizationId
            """, p)).ToList();
        }
        return result;
      }

      public async Task<ToDo> GetAsync(int id, int organizationId)
      {
        ToDo? result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          var p = new DynamicParameters();
          p.Add("@ToDoID", id);
          p.Add("@OrganizationId", organizationId);

          result = await con.QuerySingleOrDefaultAsync<ToDo>("""
            SELECT * 
            FROM "ToDo"
            WHERE "ToDoID" = @ToDoID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result!;
      }

      public async Task<List<ToDo>> GetChildrenAsync(int parentId, int organizationId)
      {
        IEnumerable<ToDo> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          var p = new DynamicParameters();
          p.Add("@ParentId", parentId);
          p.Add("@OrganizationId", organizationId);

          result = await con.QueryAsync<ToDo>("""
            SELECT * FROM "ToDo"
            WHERE "ParentToDoId" = @ParentId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<ToDo>> GetDescendantsAsync(int id, int organizationId)
      {
        IEnumerable<ToDo> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          DynamicParameters p = new DynamicParameters();
          p.Add("@ToDoID", id);
          p.Add("@OrganizationId", organizationId);

          string sql = """
            WITH RECURSIVE Descendants AS (
              SELECT 
                "ToDoID",
                "Title", 
                "Content", 
                "ParentToDoId",
                "Status",
                "OrganizationId"
              FROM "ToDo"
              WHERE "ParentToDoId" = @ToDoID
              UNION ALL
              SELECT 
                t."ToDoID",
                t."Title", 
                t."Content", 
                t."ParentToDoId",
                t."Status",
                t."OrganizationId"
              FROM "ToDo" t
              INNER JOIN Descendants d ON t."ParentToDoId" = d."ToDoID"
            )
            SELECT * FROM Descendants
            WHERE "OrganizationId" = @OrganizationId
            """;

          result = await con.QueryAsync<ToDo>(sql, p);
        }

        return result.ToList();
      }

      public async Task<List<ToDo>> GetToDoChildrenAsync(int id, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ToDoID", id);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ToDo> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ToDo>("""
            SELECT * 
            FROM "ToDo" 
            WHERE "ParentToDoId" = @ToDoID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public int Update(ToDo entity)
      {
        throw new NotImplementedException();
      }

      public async Task<ToDo> UpdateContentAsync(int toDoId, string content, int organizationId)
      {
        ToDo result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          DynamicParameters p = new DynamicParameters();
          p.Add("@ToDoID", toDoId);
          p.Add("@Content", content);
          p.Add("@OrganizationId", organizationId);

          IEnumerable<ToDo> updatedToDos = await con.QueryAsync<ToDo>("""
            UPDATE "ToDo"
            SET "Content" = @Content
            WHERE "ToDoID" = @ToDoID
            AND "OrganizationId" = @OrganizationId
            RETURNING *;
            """, p);

          result = updatedToDos.Single();
        }

        return result;
      }

      public async Task<int> UpdateParentToDoIdAsync(int toDoId, int? newParentId, int organizationId)
      {
        // Ensure a ToDo cannot be made a parent of itself
        if (toDoId == newParentId)
        {
          throw new InvalidOperationException("A ToDo cannot be made a parent of itself.");
        }

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          DynamicParameters p = new DynamicParameters();
          p.Add("@ToDoID", toDoId, DbType.Int32);
          p.Add("@NewParentId", newParentId, DbType.Int32);
          p.Add("@OrganizationId", organizationId);

          // Check if new parent is a descendant of the ToDo
          string checkSql = """
            WITH RECURSIVE ToDoTree AS (
              SELECT "ToDoID", "ParentToDoId" 
              FROM "ToDo" 
              WHERE "ToDoID" = @ToDoID
              AND "OrganizationId" = @OrganizationId
              UNION ALL
              SELECT t."ToDoID", t."ParentToDoId"
              FROM "ToDo" t
              JOIN ToDoTree tt ON t."ParentToDoId" = tt."ToDoID"
              WHERE t."OrganizationId" = @OrganizationId
            )
            SELECT 1
            FROM ToDoTree 
            WHERE "ToDoID" = @NewParentId
            """;

          var circularCheck = await con.QueryFirstOrDefaultAsync<int?>(checkSql, p);

          if (circularCheck != null)
          {
            throw new InvalidOperationException("Changing the parent ToDo would result in a circular reference.");
          }

          // If no circular reference detected, proceed with the update
          string sql = """
            UPDATE "ToDo"
            SET "ParentToDoId" = @NewParentId
            WHERE "ToDoID" = @ToDoID AND "OrganizationId" = @OrganizationId
            """;

          int affectedRows = await con.ExecuteAsync(sql, p);
          return affectedRows;
        }
      }

      public async Task<int> UpdateTaskStatusIdAsync(int toDoId, string status, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ToDoId", toDoId);
        p.Add("@Status", status);
        p.Add("@OrganizationId", organizationId);

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string sql = """
            UPDATE "ToDo"
            SET "Status" = @Status
            WHERE "ToDoID" = @ToDoId
            AND "OrganizationId" = @OrganizationId
            """;

          return await con.ExecuteAsync(sql, p);
        }
      }
    }

    public IToDoTagManager GetToDoTagManager()
    {
      throw new NotImplementedException();
    }

    public IUserManager GetUserManager()
    {
      return new UserManager();
    }

    public class UserManager : IUserManager
    {
      public User Create(User entity)
      {
        throw new NotImplementedException();
      }

      public async Task<User> CreateAsync(User entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", entity.Email);
        p.Add("@FirstName", entity.FirstName);
        p.Add("@LastName", entity.LastName);
        p.Add("@CreatedById", entity.CreatedById);

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<User>("""
            INSERT INTO "User" ("Email", "FirstName", "LastName", "CreatedById") 
            VALUES (@Email, @FirstName, @LastName, @CreatedById)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public User Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<User> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<User>> GetAllAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<User>("""
            SELECT u.* 
            FROM "User" u
            INNER JOIN "UserOrganization" uo ON u."UserID" = uo."UserId"
            WHERE uo."OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<User> GetAsync(int userId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", userId);

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<User>("""
            SELECT * 
            FROM "User" 
            WHERE "UserID" = @UserId
            """
            , p);
        }

        return result.SingleOrDefault()!;
      }

      public async Task<User> GetByEmailAsync(string email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<User>("""
            SELECT * 
            FROM "User" 
            WHERE "Email" = @Email
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<int> UpdatePasswordAsync(int userId, string passwordHash)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserID", userId);
        p.Add("@Password", passwordHash);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "User" 
            SET "Password" = @Password
            WHERE "UserID" = @UserID
            """, p);
        }

        return rowsModified;
      }

      public int Update(User entity)
      {
        throw new NotImplementedException();
      }
    }

    public IUserOrganizationManager GetUserOrganizationManager()
    {
      return new UserOrganizationManager();
    }

    public class UserOrganizationManager : IUserOrganizationManager
    {
      public UserOrganization Create(UserOrganization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<UserOrganization> CreateAsync(UserOrganization entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("UserId", entity.UserId);
        p.Add("OrganizationId", entity.OrganizationId);

        IEnumerable<UserOrganization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<UserOrganization>($"""
            INSERT INTO "UserOrganization" ("UserId", "OrganizationId") 
            VALUES (@UserId, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public UserOrganization Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<UserOrganization> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<UserOrganization> GetAsync(int userId, int organizationId)
      {
        var p = new DynamicParameters();
        p.Add("UserID", userId);
        p.Add("OrganizationId", organizationId);

        IEnumerable<UserOrganization> result;

        using (IDbConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        { 
          result = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>(
            """
            SELECT uo.*, u.*, o.* 
            FROM "UserOrganization" uo
            INNER JOIN "User" u ON uo."UserId" = u."UserID"
            INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
            WHERE uo."UserId" = @UserID AND uo."OrganizationId" = @OrganizationId
            """,
            (uo, u, o) =>
            {
              uo.User = u;
              uo.Organization = o;
              return uo;
            }, p,
            splitOn: "UserID,OrganizationID");
        }

        return result.SingleOrDefault()!;
      }

      public async Task<List<Organization>> GetByUserIdAsync(int userId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", userId);

        IEnumerable<Organization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Organization>("""
            select o.* from "Organization" o
            inner join "UserOrganization" uo on uo."OrganizationId" = o."OrganizationID"
            where uo."UserId" = @UserId
            """
            , p);
        }

        return result.ToList();
      }

      public int Update(UserOrganization entity)
      {
        throw new NotImplementedException();
      }
    }

    public IReconciliationExpenseManager GetExpenseManager()
    {
      return new ReconciliationExpenseManager();
    }

    public class ReconciliationExpenseManager : IReconciliationExpenseManager
    {
      public ReconciliationExpense Create(ReconciliationExpense entity)
      {
        throw new NotImplementedException();
      }

      public async Task<ReconciliationExpense> CreateAsync(ReconciliationExpense entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Amount", entity.Amount);
        p.Add("@ReconciliationTransactionId", entity.ReconciliationTransactionId);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<ReconciliationExpense> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationExpense>("""
            INSERT INTO "ReconciliationExpense" ("Amount", "ReconciliationTransactionId", "CreatedById", "OrganizationId") 
            VALUES (@Amount, @ReconciliationTransactionId, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public ReconciliationExpense Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<ReconciliationExpense> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<ReconciliationExpense> GetAsync(int reconciliationTransactionID)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);

        IEnumerable<ReconciliationExpense> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationExpense>("""
            SELECT * 
            FROM "ReconciliationExpense" 
            WHERE "ReconciliationTransactionId" = @ReconciliationTransactionID;
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public int Update(ReconciliationExpense entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAmountAsync(int reconciliationExpenseId, decimal? amount)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationExpenseID", reconciliationExpenseId);
        p.Add("@Amount", amount);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "ReconciliationExpense" 
            SET "Amount" = @Amount
            WHERE "ReconciliationExpenseID" = @ReconciliationExpenseID;
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateAsync(ReconciliationExpense expense)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationExpenseID", expense.ReconciliationExpenseID);
        p.Add("@Amount", expense.Amount);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "ReconciliationExpense" 
            SET 
            "Amount" = @Amount
            WHERE "ReconciliationExpenseID" = @ReconciliationExpenseID;
            """, p);
        }

        return rowsAffected;
      }
    }

    public IGeneralLedgerReconciliationTransactionManager GetGeneralLedgerReconciliationExpenseManager()
    {
      return new GeneralLedgerReconciliationTransactionManager();
    }

    public class GeneralLedgerReconciliationTransactionManager : IGeneralLedgerReconciliationTransactionManager
    {
      public GeneralLedgerReconciliationTransaction Create(GeneralLedgerReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }

      public async Task<GeneralLedgerReconciliationTransaction> CreateAsync(GeneralLedgerReconciliationTransaction entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@GeneralLedgerId", entity.GeneralLedgerId);
        p.Add("@ReconciliationTransactionId", entity.ReconciliationTransactionId);
        p.Add("@ReversedGeneralLedgerReconciliationTransactionId", entity.ReversedGeneralLedgerReconciliationTransactionId);
        p.Add("@TransactionGuid", entity.TransactionGuid);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<GeneralLedgerReconciliationTransaction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<GeneralLedgerReconciliationTransaction>("""
            INSERT INTO "GeneralLedgerReconciliationTransaction" 
            ("GeneralLedgerId", "ReversedGeneralLedgerReconciliationTransactionId", "ReconciliationTransactionId", "TransactionGuid", "CreatedById", "OrganizationId") 
            VALUES 
            (@GeneralLedgerId, @ReversedGeneralLedgerReconciliationTransactionId, @ReconciliationTransactionId, @TransactionGuid, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public GeneralLedgerReconciliationTransaction Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<GeneralLedgerReconciliationTransaction> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<GeneralLedgerReconciliationTransaction>> GetLastTransactionAsync(
        int reconciliationTransactionId,
        int organizationId,
        bool loadChildren = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionId", reconciliationTransactionId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<GeneralLedgerReconciliationTransaction> result;

        if (loadChildren)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            result = await con.QueryAsync<GeneralLedgerReconciliationTransaction, GeneralLedger, GeneralLedgerReconciliationTransaction>("""
              SELECT glrt.*, gl.*
              FROM "GeneralLedgerReconciliationTransaction" glrt
              JOIN "GeneralLedger" gl ON glrt."GeneralLedgerId" = gl."GeneralLedgerID"
              WHERE glrt."TransactionGuid" IN (
                  SELECT "TransactionGuid"
                  FROM "GeneralLedgerReconciliationTransaction"
                  WHERE "ReconciliationTransactionId" = @ReconciliationTransactionId
                  AND "OrganizationId" = @OrganizationId
                  ORDER BY "GeneralLedgerReconciliationTransactionID" DESC
                  LIMIT 1
              );
              """, (glrt, gl) =>
            {
              glrt.GeneralLedger = gl;
              return glrt;
            }, p, splitOn: "GeneralLedgerID");
          }
        }
        else
        {
          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            result = await con.QueryAsync<GeneralLedgerReconciliationTransaction>("""
              SELECT *
              FROM "GeneralLedgerReconciliationTransaction"
              WHERE "TransactionGuid" IN (
                  SELECT "TransactionGuid"
                  FROM "GeneralLedgerReconciliationTransaction"
                  WHERE "ReconciliationTransactionId" = @ReconciliationTransactionId
                  AND "OrganizationId" = @OrganizationId
                  ORDER BY "GeneralLedgerReconciliationTransactionID" DESC
                  LIMIT 1
              );
              """, p);
          }
        }

        return result.ToList();
      }

      public int Update(GeneralLedgerReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }
    }

    public IReconciliationExpenseCategoryManager GetReconciliationExpenseCategoryManager()
    {
      return new ReconciliationExpenseCategoryManager();
    }

    public class ReconciliationExpenseCategoryManager : IReconciliationExpenseCategoryManager
    {
      public ReconciliationExpenseCategory Create(ReconciliationExpenseCategory entity)
      {
        throw new NotImplementedException();
      }

      public Task<ReconciliationExpenseCategory> CreateAsync(ReconciliationExpenseCategory entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public ReconciliationExpenseCategory Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<ReconciliationExpenseCategory> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<ReconciliationExpenseCategory>> GetAllAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<ReconciliationExpenseCategory> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationExpenseCategory>("""
            SELECT * 
            FROM "ReconciliationExpenseCategory" 
            WHERE "OrganizationId" = @OrganizationId;
            """, p);
        }

        return result.ToList();
      }

      public async Task<ReconciliationExpenseCategory> GetAsync(int reconciliationExpenseCategoryId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationExpenseCategoryID", reconciliationExpenseCategoryId);

        IEnumerable<ReconciliationExpenseCategory> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ReconciliationExpenseCategory>("""
            SELECT * 
            FROM "ReconciliationExpenseCategory" 
            WHERE "ReconciliationExpenseCategoryID" = @ReconciliationExpenseCategoryID;
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public int Update(ReconciliationExpenseCategory entity)
      {
        throw new NotImplementedException();
      }
    }

    public IDatabaseManager GetDatabaseManager()
    {
      return new DatabaseManager();
    }

    public class DatabaseManager : IDatabaseManager
    {
      public Business.DatabaseThing Create(Business.DatabaseThing entity)
      {
        throw new NotImplementedException();
      }

      public Task<Business.DatabaseThing> CreateAsync(Business.DatabaseThing entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Business.DatabaseThing Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Business.DatabaseThing> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task ResetDatabaseAsync()
      {
        // Combine the base directory with the script name to get the full path
        string resetCreateDbScriptPath = Path.Combine(AppContext.BaseDirectory, "reset-and-create-database.sql");
        string createSchemaScriptPath = Path.Combine(AppContext.BaseDirectory, "create-db-script-psql.sql");

        // Read the content of the scripts
        string resetCreateDbScript = System.IO.File.ReadAllText(resetCreateDbScriptPath);
        string createSchemaScript = System.IO.File.ReadAllText(createSchemaScriptPath);

        // Execute the scripts using Npgsql and Dapper
        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.AdminPsql))
        {
          await con.ExecuteAsync(resetCreateDbScript);
        }

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          await con.ExecuteAsync(createSchemaScript);
        }
      }

      public int Update(Business.DatabaseThing entity)
      {
        throw new NotImplementedException();
      }
    }

    public IGeneralLedgerInvoiceInvoiceLineManager GetGeneralLedgerInvoiceInvoiceLineManager()
    {
      return new GeneralLedgerInvoiceInvoiceLineManager();
    }

    public class GeneralLedgerInvoiceInvoiceLineManager : IGeneralLedgerInvoiceInvoiceLineManager
    {
      public GeneralLedgerInvoiceInvoiceLine Create(GeneralLedgerInvoiceInvoiceLine entity)
      {
        throw new NotImplementedException();
      }

      public async Task<GeneralLedgerInvoiceInvoiceLine> CreateAsync(GeneralLedgerInvoiceInvoiceLine entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@GeneralLedgerId", entity.GeneralLedgerId);
        p.Add("@InvoiceId", entity.InvoiceId);
        p.Add("@InvoiceLineId", entity.InvoiceLineId);
        p.Add("@ReverseGeneralLedgerInvoiceInvoiceLineId", entity.ReversedGeneralLedgerInvoiceInvoiceLineId);
        p.Add("@TransactionGuid", entity.TransactionGuid);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<GeneralLedgerInvoiceInvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLine>("""
            INSERT INTO "GeneralLedgerInvoiceInvoiceLine" 
            ("GeneralLedgerId", "InvoiceId", "InvoiceLineId", "ReversedGeneralLedgerInvoiceInvoiceLineId", "TransactionGuid", "CreatedById", "OrganizationId")
            VALUES 
            (@GeneralLedgerId, @InvoiceId, @InvoiceLineId, @ReverseGeneralLedgerInvoiceInvoiceLineId, @TransactionGuid, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public GeneralLedgerInvoiceInvoiceLine Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<GeneralLedgerInvoiceInvoiceLine> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<GeneralLedgerInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);
        p.Add("@IncludeRemoved", includeRemoved);

        IEnumerable<GeneralLedgerInvoiceInvoiceLine> result;

        throw new NotImplementedException();

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLine>("""

            """, p);
        }
      }

      public async Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId, int organizationId, bool onlyCurrent = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          string query;
          if (onlyCurrent)
          {
            query = """
                SELECT il.*
                FROM "InvoiceLine" il
                WHERE il."InvoiceId" = @InvoiceId
                AND il."OrganizationId" = @OrganizationId
                AND EXISTS (
                    SELECT 1
                    FROM "GeneralLedgerInvoiceInvoiceLine" gliil
                    INNER JOIN (
                        SELECT "InvoiceLineId", MAX("GeneralLedgerInvoiceInvoiceLineID") AS MaxGLIILID
                        FROM "GeneralLedgerInvoiceInvoiceLine"
                        GROUP BY "InvoiceLineId"
                    ) AS LastTransactions ON gliil."InvoiceLineId" = LastTransactions."InvoiceLineId" AND gliil."GeneralLedgerInvoiceInvoiceLineID" = LastTransactions.MaxGLIILID
                    WHERE gliil."InvoiceLineId" = il."InvoiceLineID"
                    AND gliil."ReversedGeneralLedgerInvoiceInvoiceLineId" IS NULL
                )
                """;
          }
          else
          {
            query = """
                SELECT *
                FROM "InvoiceLine"
                WHERE "InvoiceId" = @InvoiceId
                AND "OrganizationId" = @OrganizationId
                """;
          }

          result = await con.QueryAsync<InvoiceLine>(query, p);
        }

        return result.ToList();
      }

      public async Task<List<GeneralLedgerInvoiceInvoiceLine>> GetLastTransactionAsync(
        int invoiceLineId,
        int organizationId,
        bool loadChildren)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceLineId", invoiceLineId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<GeneralLedgerInvoiceInvoiceLine> result;

        if (loadChildren)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            string query = """
              SELECT
                  gliil.*,
                  gl.*
              FROM "GeneralLedgerInvoiceInvoiceLine" gliil
              INNER JOIN "GeneralLedger" gl ON gliil."GeneralLedgerId" = gl."GeneralLedgerID"
              WHERE 
                gliil."TransactionGuid" = (
                  SELECT "TransactionGuid"
                  FROM "GeneralLedgerInvoiceInvoiceLine"
                  WHERE "InvoiceLineId" = @InvoiceLineId
                  ORDER BY "GeneralLedgerInvoiceInvoiceLineID" DESC
                  LIMIT 1
                )
                AND "InvoiceLineId" = @InvoiceLineId
                AND gliil."ReversedGeneralLedgerInvoiceInvoiceLineId" IS NULL
                AND gliil."OrganizationId" = @OrganizationId
              """;

            result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLine, GeneralLedger, GeneralLedgerInvoiceInvoiceLine>(query, (gliil, gl) =>
            {
              gliil.GeneralLedger = gl;
              return gliil;
            }, p, splitOn: "GeneralLedgerID");
          }
        }
        else
        {
          using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
          {
            string query = """
              SELECT *
              FROM "GeneralLedgerInvoiceInvoiceLine"
              WHERE "TransactionGuid" = (
                SELECT "TransactionGuid"
                FROM "GeneralLedgerInvoiceInvoiceLine"
                WHERE "InvoiceLineId" = @InvoiceLineId
                ORDER BY "Created" DESC
                LIMIT 1
              )
              AND "ReversedGeneralLedgerInvoiceInvoiceLineId" IS NULL
              AND "OrganizationId" = @OrganizationId
              """;

            result = await con.QueryAsync<GeneralLedgerInvoiceInvoiceLine>(query, p);
          }
        }

        return result.ToList();
      }

      public int Update(GeneralLedgerInvoiceInvoiceLine entity)
      {
        throw new NotImplementedException();
      }
    }

    public IUserToDoManager GetUserToDoManager()
    {
      return new UserTaskManager();
    }

    public IInventoryManager GetInventoryManager()
    {
      return new InventoryManager();
    }

    public class InventoryManager : IInventoryManager
    {
      public Inventory Create(Inventory entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Inventory> CreateAsync(Inventory entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemId", entity.ItemId);
        p.Add("@LocationId", entity.LocationId);
        p.Add("@Quantity", entity.Quantity);
        p.Add("@SalePrice", entity.SalePrice);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Inventory> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Inventory>("""
            INSERT INTO "Inventory" ("ItemId", "LocationId", "Quantity", "SalePrice", "CreatedById", "OrganizationId") 
            VALUES (@ItemId, @LocationId, @Quantity, @SalePrice, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Inventory Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Inventory> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Inventory>?> GetAllAsync(
        int page, 
        int pageSize, 
        int organizationId)
      {
        throw new NotImplementedException();
      }

      public async Task<List<Inventory>?> GetAllAsync(int[] itemIds, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemIds", itemIds);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Inventory> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Inventory>("""
            SELECT * 
            FROM "Inventory" 
            WHERE "ItemId" = ANY(@ItemIds)
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public int Update(Inventory entity)
      {
        throw new NotImplementedException();
      }
    }

    public IRequestLogManager GetRequestLogManager()
    {
      return new RequestLogManager();
    }

    public class RequestLogManager : IRequestLogManager
    {
      public RequestLog Create(RequestLog entity)
      {
        throw new NotImplementedException();
      }

      public async Task<RequestLog> CreateAsync(RequestLog entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@RemoteIp", entity.RemoteIp);
        p.Add("@CountryCode", entity.CountryCode);
        p.Add("@Referer", entity.Referer);
        p.Add("@UserAgent", entity.UserAgent);
        p.Add("@Path", entity.Path);
        p.Add("@ResponseLengthBytes", entity.ResponseLengthBytes);
        p.Add("@StatusCode", entity.StatusCode);

        IEnumerable<RequestLog> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<RequestLog>("""
            INSERT INTO "RequestLog" 
            ("RemoteIp", "CountryCode", "Referer", "UserAgent", "Path", "ResponseLengthBytes", "StatusCode") 
              VALUES 
            (@RemoteIp, @CountryCode, @Referer, @UserAgent, @Path, @ResponseLengthBytes, @StatusCode)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public RequestLog Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<RequestLog> GetAll()
      {
        throw new NotImplementedException();
      }

      public int Update(RequestLog entity)
      {
        throw new NotImplementedException();
      }
    }

    public IInventoryAdjustmentManager GetInventoryAdjustmentManager()
    {
      return new InventoryAdjustmentManager();
    }

    public class InventoryAdjustmentManager : IInventoryAdjustmentManager
    {
      public InventoryAdjustment Create(InventoryAdjustment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<InventoryAdjustment> CreateAsync(InventoryAdjustment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemId", entity.ItemId);
        p.Add("@ToLocationId", entity.ToLocationId);
        p.Add("@FromLocationId", entity.FromLocationId);
        p.Add("@Quantity", entity.Quantity);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<InventoryAdjustment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<InventoryAdjustment>("""
            INSERT INTO "InventoryAdjustment" 
            ("ItemId", "ToLocationId", "FromLocationId", "Quantity", "CreatedById", "OrganizationId") 
            VALUES 
            (@ItemId, @ToLocationId, @FromLocationId, @Quantity, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public InventoryAdjustment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<InventoryAdjustment> GetAll()
      {
        throw new NotImplementedException();
      }

      public int Update(InventoryAdjustment entity)
      {
        throw new NotImplementedException();
      }
    }

    public IZIPCodeManager GetZIPCodeManager()
    {
      return new ZIPCodeManager();
    }

    public class ZIPCodeManager : IZIPCodeManager
    {
      public ZipCode Create(ZipCode entity)
      {
        throw new NotImplementedException();
      }

      public Task<ZipCode> CreateAsync(ZipCode entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public ZipCode Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<ZipCode> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<ZipCode>> GetAllAsync(bool locationIsNull)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@LocationIsNull", locationIsNull);

        IEnumerable<ZipCode> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<ZipCode>("""
            SELECT "ID", "Zip5", "Latitude", "Longitude", "City", "State2"
            FROM "ZipCode"
            WHERE "Location" IS NULL AND "Latitude" IS NOT NULL AND "Longitude" IS NOT NULL
            """, p);
        }

        return result.ToList();
      }

      public int Update(ZipCode entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateLocationAsync(List<ZipCode> zipCodes)
      {
        int rowsAffected = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          foreach (var zipCode in zipCodes)
          {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ID", zipCode.ID);
            p.Add("@Latitude", zipCode.Latitude);
            p.Add("@Longitude", zipCode.Longitude);

            string query = """
              UPDATE "ZipCode"
              SET "Location" = ST_SetSRID(ST_MakePoint(@Longitude, @Latitude), 4326)
              WHERE "ID" = @ID;
              """;

            rowsAffected += await con.ExecuteAsync(query, p);
          }
        }

        return rowsAffected;
      }
    }

    public ITenantManager GetTenantManager()
    {
      throw new NotImplementedException();
    }

    public class TenantManager : ITenantManager
    {
//      CREATE TABLE "Tenant"
//(
//	"TenantID" SERIAL PRIMARY KEY NOT NULL,
//	"Name" VARCHAR(100) NOT NULL,
//	"Email" VARCHAR(100) NOT NULL,
//	"Ipv4" VARCHAR(15) NOT NULL,
//	"VmHostname" VARCHAR(255) NOT NULL,
//	"SSHPublic" TEXT NOT NULL, 
//	"CreatedById" INT NOT NULL,
//	"Created" TIMESTAMPTZ NOT NULL DEFAULT(CURRENT_TIMESTAMP AT TIME ZONE 'UTC')
//);

      public Tenant Create(Tenant entity)
      {
        throw new NotImplementedException();
      }

      public Task<Tenant> CreateAsync(Tenant entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<bool> ExistsAsync(string email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<Tenant> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "Email" = @Email
            """, p);
        }

        return result.Any();
      }

      public Tenant Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Tenant> GetAll()
      {
        throw new NotImplementedException();
      }

      public int Update(Tenant entity)
      {
        throw new NotImplementedException();
      }
    }

    public ILocationManager GetLocationManager()
    {
      return new LocationManager();
    }

    public class LocationManager : ILocationManager
    {
      public Location Create(Location entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Location> CreateAsync(Location entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", entity.Name);
        p.Add("@Description", entity.Description);
        p.Add("@ParentLocationId", entity.ParentLocationId);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Location> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Location>("""
            INSERT INTO "Location" ("Name", "Description", "ParentLocationId", "CreatedById", "OrganizationId") 
            VALUES (@Name, @Description, @ParentLocationId, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Location Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Location> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<(List<Location> Locations, int? NextPageNumber)> GetAllAsync(
        int page,
        int pageSize,
        int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Location> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Location>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "LocationID" DESC) AS RowNumber
                FROM "Location"
                WHERE "OrganizationId" = @OrganizationId
            ) AS NumberedLocations
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var resultList = result.ToList();
        int? nextPageNumber = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPageNumber = page + 1;
        }

        return (resultList, nextPageNumber);
      }

      public async Task<List<Location>> GetAllAsync(int organizationId)
      {
        IEnumerable<Location> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Location>("""
            SELECT * 
            FROM "Location" 
            WHERE "OrganizationId" = @OrganizationId
            """, new { OrganizationId = organizationId });
        }

        return result.ToList();
      }

      public async Task<Location?> GetAsync(int locationId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@LocationID", locationId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Location> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<Location>("""
            SELECT * 
            FROM "Location" 
            WHERE "LocationID" = @LocationID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(Location entity)
      {
        throw new NotImplementedException();
      }
    }

    public class UserTaskManager : IUserToDoManager
    {
      public UserToDo Create(UserToDo entity)
      {
        throw new NotImplementedException();
      }

      public async Task<UserToDo> CreateAsync(UserToDo entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", entity.UserId);
        p.Add("@ToDoId", entity.ToDoId);
        p.Add("@Completed", entity.Completed);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@CreatedById", entity.CreatedById);

        IEnumerable<UserToDo> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<UserToDo>("""
            INSERT INTO "UserToDo" ("UserId", "ToDoId", "Completed", "OrganizationId", "CreatedById") 
            VALUES (@UserId, @TodoId, @Completed, @OrganizationId, @CreatedById)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public UserToDo Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<UserToDo> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<User>> GetUsersAsync(int toDoId, int organizationId)
      {
        var p = new DynamicParameters();
        p.Add("@ToDoId", toDoId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(ConfigurationSingleton.Instance.ConnectionStringPsql))
        {
          result = await con.QueryAsync<User>("""
            SELECT u.* 
            FROM "User" u
            INNER JOIN "UserToDo" utt ON u."UserID" = utt."UserId"
            WHERE utt."ToDoId" = @ToDoId
            AND utt."OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public int Update(UserToDo entity)
      {
        throw new NotImplementedException();
      }
    }
  }
}