using Dapper;
using Accounting.Business;
using Accounting.Common;
using Accounting.Database.Interfaces;
using Npgsql;
using System.Data;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;
using Renci.SshNet.Security;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accounting.Database
{
  public class DapperPsqlFactory : IDatabaseFactoryDefinition
  {
    private readonly string _connectionString;

    public DapperPsqlFactory(string databaseName, string databasePassword)
    {
      NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
      builder.Host = "localhost";
      builder.Username = "postgres";
      builder.Database = databaseName;
      builder.Password = databasePassword;
      _connectionString = builder.ConnectionString;
    }

    public IAddressManager GetAddressManager()
    {
      return new AddressManager(_connectionString);
    }

    public class AddressManager : IAddressManager
    {
      private readonly string _connectionString;

      public AddressManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Address>("""
            SELECT * 
            FROM "Address" 
            WHERE "BusinessEntityId" = @BusinessEntityId
            """, p);
        }

        return result.ToList();
      }

      public async Task<Address?> GetAsync(int addressId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ID", addressId);

        IEnumerable<Address> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new BusinessEntityManager(_connectionString);
    }

    public class BusinessEntityManager : IBusinessEntityManager
    {
      private readonly string _connectionString;

      public BusinessEntityManager(string connectionSring)
      {
        _connectionString = connectionSring;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<List<BusinessEntity>> GetAllAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<BusinessEntity> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<BusinessEntity>("""
            SELECT * FROM "BusinessEntity"
            WHERE "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<(List<BusinessEntity> businessEntities, int? nextPage)> GetAllAsync(int page, int pageSize, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<BusinessEntity> result;
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<BusinessEntity>("""
            SELECT * FROM (
                SELECT *, ROW_NUMBER() OVER (ORDER BY "BusinessEntityID" DESC) AS RowNumber
                FROM "BusinessEntity"
                WHERE "OrganizationId" = @OrganizationId    
            ) AS NumberedBusinessEntities
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var resultList = result.ToList();
        int? nextPage = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPage = page + 1;
        }

        return (resultList, nextPage);
      }

      public async Task<BusinessEntity> GetByIdAsync(int businessEntityId, int organizationId)
      {
        var p = new DynamicParameters();
        p.Add("@BusinessEntityID", businessEntityId);
        p.Add("@OrganizationId", organizationId);

        BusinessEntity? result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<int> UpdateAsync(int id, string? firstName, string? lastName, string? companyName, string? selectedCustomerType, string? businessEntityTypesCsv, int? selectedPaymentTermId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@BusinessEntityID", id);
        p.Add("@FirstName", firstName);
        p.Add("@LastName", lastName);
        p.Add("@CompanyName", companyName);
        p.Add("@CustomerType", selectedCustomerType);
        p.Add("@BusinessEntityTypesCsv", businessEntityTypesCsv);
        p.Add("@PaymentTermId", selectedPaymentTermId);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "BusinessEntity" SET 
              "FirstName" = @FirstName,
              "LastName" = @LastName,
              "CompanyName" = @CompanyName,
              "CustomerType" = @CustomerType,
              "BusinessEntityTypesCsv" = @BusinessEntityTypesCsv,
              "PaymentTermId" = @PaymentTermId
            WHERE "BusinessEntityID" = @BusinessEntityID
            """, p);
        }

        return rowsModified;
      }
    }


    public IAccountManager GetAccountManager()
    {
      return new AccountManager(_connectionString);
    }

    public class AccountManager : IAccountManager
    {
      private readonly string _connectionString;

      public AccountManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Account Create(Account entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Account> CreateAsync(Account entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", entity.Name);
        p.Add("@Type", entity.Type);
        p.Add("@InvoiceCreationForCredit", entity.InvoiceCreationForCredit);
        p.Add("@InvoiceCreationForDebit", entity.InvoiceCreationForDebit);
        p.Add("@ReceiptOfPaymentForCredit", entity.ReceiptOfPaymentForCredit);
        p.Add("@ReceiptOfPaymentForDebit", entity.ReceiptOfPaymentForDebit);
        p.Add("@ParentAccountId", entity.ParentAccountId);
        p.Add("@ReconciliationExpense", entity.ReconciliationExpense);
        p.Add("@ReconciliationLiabilitiesAndAssets", entity.ReconciliationLiabilitiesAndAssets);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            INSERT INTO "Account" 
            ("Name", "Type", "InvoiceCreationForCredit", "InvoiceCreationForDebit", "ReceiptOfPaymentForCredit", "ReceiptOfPaymentForDebit", "ReconciliationExpense", "ReconciliationLiabilitiesAndAssets", "ParentAccountId", "CreatedById", "OrganizationId") 
            VALUES 
            (@Name, @Type, @InvoiceCreationForCredit, @InvoiceCreationForDebit, @ReceiptOfPaymentForCredit, @ReceiptOfPaymentForDebit, @ReconciliationExpense, @ReconciliationLiabilitiesAndAssets, @ParentAccountId, @CreatedById, @OrganizationId)
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
        p.Add("@AccountID", id);
        p.Add("@OrganizationId", organizationId);

        bool result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.ExecuteScalarAsync<bool>("""
            SELECT CASE WHEN COUNT(*) > 0 THEN true ELSE false END
            FROM "Account" 
            WHERE "AccountID" = @AccountID AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }

      public Account Get(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<List<Account>> GetAccountBalanceReport(int organizationId)
      {
        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          string query = $"""
            SELECT 
                a."AccountID", 
                a."Name", 
                a."Type", 
                CASE 
                    WHEN a."Type" IN ('assets', 'expense') THEN SUM(COALESCE(gl."Debit", 0)) - SUM(COALESCE(gl."Credit", 0))
                    ELSE SUM(COALESCE(gl."Credit", 0)) - SUM(COALESCE(gl."Debit", 0))
                END AS "CurrentBalance"
            FROM "Account" a
            LEFT JOIN "Journal" gl ON a."AccountID" = gl."AccountId" AND a."OrganizationId" = gl."OrganizationId"
            WHERE a."OrganizationId" = @OrganizationId
            GROUP BY a."AccountID", a."Name", a."Type"
            """;

          result = await con.QueryAsync<Account>(query, new { OrganizationId = organizationId });
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAccountOptionsForInvoiceCreationCredit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "InvoiceCreationForCredit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAccountOptionsForInvoiceCreationDebit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "InvoiceCreationForDebit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAccountOptionsForPaymentReceptionCredit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "ReceiptOfPaymentForCredit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAccountOptionsForPaymentReceptionDebit(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "ReceiptOfPaymentForDebit" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public IEnumerable<Account> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Account>> GetAllAsync(int organizationId, bool includeCountJournalEntries)
      {
        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          if (includeCountJournalEntries)
          {
            result = await con.QueryAsync<Account>("""
                SELECT a.*, COUNT(gl."JournalID") AS "JournalEntryCount"
                FROM "Account" a
                LEFT JOIN "Journal" gl ON a."AccountID" = gl."AccountId" AND a."OrganizationId" = gl."OrganizationId"
                WHERE a."OrganizationId" = @OrganizationId
                GROUP BY a."AccountID"
                """, new { OrganizationId = organizationId });
          }
          else
          {
            result = await con.QueryAsync<Account>("""
                SELECT * 
                FROM "Account"
                WHERE "OrganizationId" = @OrganizationId
                """, new { OrganizationId = organizationId });
          }
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAllAsync(string accountType, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Type", accountType);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "Type" = @Type
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<(List<Account> accounts, int? nextPage)> GetAllAsync(
        int page,
        int pageSize,
        int organizationId,
        bool includeJournalEntriesCount,
        bool includeDescendants)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize + 1);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>($"""
            SELECT *, ROW_NUMBER() OVER (ORDER BY "AccountID") AS "RowNumber"
            FROM "Account"
            WHERE "OrganizationId" = @OrganizationId AND "ParentAccountId" IS NULL
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset
            """, new { PageSize = pageSize + 1, Offset = pageSize * (page - 1), OrganizationId = organizationId });
        }

        var hasMoreRecords = result.Count() > pageSize;

        if (hasMoreRecords)
        {
          result = result.Take(pageSize);
        }

        int? nextPage = hasMoreRecords ? page + 1 : null;

        if (includeDescendants)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            var allAccounts = await con.QueryAsync<Account>($"""
                SELECT * 
                FROM "Account"
                WHERE "OrganizationId" = @OrganizationId
                """, p);

            foreach (var account in result)
            {
              account.Children = allAccounts
                  .Where(x => x.ParentAccountId == account.AccountID)
                  .OrderBy(x => x.Name)
                  .ToList();
              if (account.Children.Any())
              {
                PopulateChildrenRecursively(account.Children, allAccounts);
              }
            }
          }
        }

        List<int> allIds = new List<int>();

        void GetAllAccountIds(Account account, List<int> ids)
        {
          ids.Add(account.AccountID);
          if (account.Children != null)
          {
            foreach (var child in account.Children)
            {
              GetAllAccountIds(child, ids);
            }
          }
        }

        foreach (var account in result)
        {
          GetAllAccountIds(account, allIds);
        }

        if (includeJournalEntriesCount)
        {
          var accountIds = allIds;

          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            var journalEntryCounts = await con.QueryAsync<(int AccountID, int JournalEntryCount)>($"""
                SELECT "AccountId", COUNT(*) AS "JournalEntryCount"
                FROM "Journal"
                WHERE "AccountId" = ANY(@AccountIds)
                GROUP BY "AccountId"
                """, new { AccountIds = accountIds.ToArray() });

            var journalEntryCountDict = journalEntryCounts.ToDictionary(x => x.AccountID, x => x.JournalEntryCount);

            void UpdateJournalEntryCounts(Account account)
            {
              if (journalEntryCountDict.TryGetValue(account.AccountID, out var count))
              {
                account.JournalEntryCount = count;
              }
              if (account.Children != null)
              {
                foreach (var child in account.Children)
                {
                  UpdateJournalEntryCounts(child);
                }
              }
            }

            foreach (var account in result)
            {
              UpdateJournalEntryCounts(account);
            }
          }
        }

        return (result.ToList(), nextPage);
      }

      private void PopulateChildrenRecursively(List<Account> children, IEnumerable<Account> allAccounts)
      {
        foreach (var child in children)
        {
          child.Children = allAccounts.Where(x => x.ParentAccountId == child.AccountID).OrderBy(x => x.Name).ToList();

          if (child.Children.Any())
          {
            PopulateChildrenRecursively(child.Children, allAccounts);
          }
        }
      }

      public async Task<List<Account>> GetAllReconciliationExpenseAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "ReconciliationExpense" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "ReconciliationLiabilitiesAndAssets" = true
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<Account>> GetAsync(string[] accountNames, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountNames", accountNames);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "Name" = ANY(@AccountNames)
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<Account> GetAsync(int accountId, int organizationId, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountID", accountId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        string connectionString = builder.ConnectionString;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "AccountID" = @AccountID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.Single();
      }

      public async Task<Account> GetAsync(string accountName, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", accountName);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "Name" = @Name
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.Single();
      }

      public async Task<Account> GetAsync(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<Account> GetByAccountNameAsync(string accountName, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", accountName);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "Name" = @Name
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public async Task<string> GetTypeAsync(int accountId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountID", accountId);

        string? result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.ExecuteScalarAsync<string>("""
            SELECT "Type" 
            FROM "Account" 
            WHERE "AccountID" = @AccountID
            """, p);
        }

        return result!;
      }

      public int Update(Account entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAsync(Account account)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountID", account.AccountID);
        p.Add("@Name", account.Name);
        p.Add("@Type", account.Type);
        p.Add("@InvoiceCreationForCredit", account.InvoiceCreationForCredit);
        p.Add("@InvoiceCreationForDebit", account.InvoiceCreationForDebit);
        p.Add("@ReceiptOfPaymentForCredit", account.ReceiptOfPaymentForCredit);
        p.Add("@ReceiptOfPaymentForDebit", account.ReceiptOfPaymentForDebit);
        p.Add("@ReconciliationExpense", account.ReconciliationExpense);
        p.Add("@ReconciliationLiabilitiesAndAssets", account.ReconciliationLiabilitiesAndAssets);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "Account" SET 
            "Name" = @Name,
            "Type" = @Type,
            "InvoiceCreationForCredit" = @InvoiceCreationForCredit,
            "InvoiceCreationForDebit" = @InvoiceCreationForDebit,
            "ReceiptOfPaymentForCredit" = @ReceiptOfPaymentForCredit,
            "ReceiptOfPaymentForDebit" = @ReceiptOfPaymentForDebit,
            "ReconciliationExpense" = @ReconciliationExpense,
            "ReconciliationLiabilitiesAndAssets" = @ReconciliationLiabilitiesAndAssets
            WHERE "AccountID" = @AccountID
            """, p);
        }

        return rowsModified;
      }

      public async Task<Account> GetAsync(int accountId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountID", accountId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "AccountID" = @AccountID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.Single();
      }

      public async Task<List<Account>> GetAssetAccounts(int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Type", Account.AccountTypeConstants.Assets);

        IEnumerable<Account> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Account>("""
            SELECT * 
            FROM "Account" 
            WHERE "Type" = @Type
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<int> DeleteAsync(int accountID, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountID", accountID);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "Account" 
            WHERE "AccountID" = @AccountID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }
    }

    public IJournalInvoiceInvoiceLinePaymentManager GetJournalInvoiceInvoiceLinePaymentManager()
    {
      return new JournalInvoiceInvoiceLinePaymentManager(_connectionString);
    }

    public class JournalInvoiceInvoiceLinePaymentManager : IJournalInvoiceInvoiceLinePaymentManager
    {
      private readonly string _connectionString;

      public JournalInvoiceInvoiceLinePaymentManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public JournalInvoiceInvoiceLinePayment Create(JournalInvoiceInvoiceLinePayment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<JournalInvoiceInvoiceLinePayment> CreateAsync(JournalInvoiceInvoiceLinePayment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@JournalId", entity.JournalId);
        p.Add("@InvoiceInvoiceLinePaymentId", entity.InvoiceInvoiceLinePaymentId);
        p.Add("@ReversedJournalInvoiceInvoiceLinePaymentId", entity.ReversedJournalInvoiceInvoiceLinePaymentId);
        p.Add("@TransactionGuid", entity.TransactionGuid);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@CreatedById", entity.CreatedById);

        IEnumerable<JournalInvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          string insertQuery = """
            INSERT INTO "JournalInvoiceInvoiceLinePayment" ("JournalId", "InvoiceInvoiceLinePaymentId", "ReversedJournalInvoiceInvoiceLinePaymentId", "TransactionGuid", "CreatedById", "OrganizationId") 
            VALUES (@JournalId, @InvoiceInvoiceLinePaymentId, @ReversedJournalInvoiceInvoiceLinePaymentId, @TransactionGuid, @CreatedById, @OrganizationId)
            RETURNING *;
            """;

          result = await con.QueryAsync<JournalInvoiceInvoiceLinePayment>(insertQuery, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public JournalInvoiceInvoiceLinePayment Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<JournalInvoiceInvoiceLinePayment> GetAll()
      {
        throw new NotImplementedException();
      }

      public Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries)
      {
        throw new NotImplementedException();
      }

      public async Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoicePaymentId", invoicePaymentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<JournalInvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<JournalInvoiceInvoiceLinePayment>($"""
            SELECT * 
            FROM "JournalInvoicePayment" 
            WHERE "InvoicePaymentId" = @InvoicePaymentId 
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<List<JournalInvoiceInvoiceLinePayment>?> GetAllByInvoiceIdAsync(int invoiceId, int organizationId, bool includeReversedEntries = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<JournalInvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          string query = $"""
            SELECT * 
            FROM "JournalInvoiceInvoiceLinePayment" 
            WHERE "InvoiceInvoiceLinePaymentId" = @InvoiceId 
            AND "OrganizationId" = @OrganizationId
            """;

          if (!includeReversedEntries)
          {
            query += " AND \"ReversedJournalInvoiceInvoiceLinePaymentId\" IS NULL";
          }

          result = await con.QueryAsync<JournalInvoiceInvoiceLinePayment>(query, p);
        }

        return result.ToList();
      }

      public async Task<List<JournalInvoiceInvoiceLinePayment>> GetAllByPaymentIdAsync(int paymentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentId", paymentId);
        p.Add("@OrganizationId", organizationId);

        List<JournalInvoiceInvoiceLinePayment> result = new List<JournalInvoiceInvoiceLinePayment>();

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          var query = $"""
            SELECT glilp.*, gl.*
            FROM "JournalInvoiceInvoiceLinePayment" glilp
            JOIN "InvoiceInvoiceLinePayment" ilp ON glilp."InvoiceInvoiceLinePaymentId" = ilp."InvoiceInvoiceLinePaymentID"
            JOIN "Journal" gl ON glilp."JournalId" = gl."JournalID"
            WHERE glilp."TransactionGuid" = (
                SELECT "TransactionGuid" 
                FROM "JournalInvoiceInvoiceLinePayment" 
                WHERE "PaymentId" = @PaymentId
                AND "OrganizationId" = @OrganizationId
                ORDER BY "JournalInvoiceInvoiceLinePaymentID" DESC
                LIMIT 1
            ) AND glilp."ReversedJournalInvoiceInvoiceLinePaymentId" IS NULL
            """;

          var invoicePayments = await con.QueryAsync<JournalInvoiceInvoiceLinePayment, Journal, JournalInvoiceInvoiceLinePayment>(
              query,
              (glilp, gl) =>
              {
                glilp.Journal = gl;
                return glilp;
              },
              splitOn: "JournalID",
              param: p
          );

          result = invoicePayments.ToList();
        }

        return result;
      }

      public async Task<List<JournalInvoiceInvoiceLinePayment>> GetLastTransactionsAsync(int paymentId, int organizationId, bool loadChildren)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentId", paymentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<JournalInvoiceInvoiceLinePayment> result;

        if (loadChildren)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            string query = """
                SELECT giiilp.*, gl.*
                FROM "JournalInvoiceInvoiceLinePayment" giiilp
                JOIN "Journal" gl ON giiilp."JournalId" = gl."JournalID"
                WHERE giiilp."TransactionGuid" = (
                  SELECT "TransactionGuid"
                  FROM "JournalInvoiceInvoiceLinePayment"
                  WHERE "InvoiceInvoiceLinePaymentId" IN (
                    SELECT "InvoiceInvoiceLinePaymentID"
                    FROM "InvoiceInvoiceLinePayment"
                    WHERE "PaymentId" = @PaymentId
                  )
                  ORDER BY "JournalInvoiceInvoiceLinePaymentID" DESC
                  LIMIT 1
                )
                AND giiilp."ReversedJournalInvoiceInvoiceLinePaymentId" IS NULL
                AND giiilp."OrganizationId" = @OrganizationId
                """;

            result = await con.QueryAsync<JournalInvoiceInvoiceLinePayment, Journal, JournalInvoiceInvoiceLinePayment>(query, (giiilp, gl) =>
            {
              giiilp.Journal = gl;
              return giiilp;
            }, p, splitOn: "JournalID");
          }
        }
        else
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            string query = """
                SELECT *
                FROM "JournalInvoiceInvoiceLinePayment"
                WHERE "TransactionGuid" = (
                  SELECT "TransactionGuid"
                  FROM "JournalInvoiceInvoiceLinePayment"
                  WHERE "InvoiceInvoiceLinePaymentId" IN (
                    SELECT "InvoiceInvoiceLinePaymentID"
                    FROM "InvoiceInvoiceLinePayment"
                    WHERE "PaymentId" = @PaymentId
                  )
                  ORDER BY "JournalInvoiceInvoiceLinePaymentID" DESC
                  LIMIT 1
                )
                AND "ReversedJournalInvoiceInvoiceLinePaymentId" IS NULL
                AND "OrganizationId" = @OrganizationId
                """;

            result = await con.QueryAsync<JournalInvoiceInvoiceLinePayment>(query, p);
          }
        }

        return result.ToList();
      }

      public int Update(JournalInvoiceInvoiceLinePayment entity)
      {
        throw new NotImplementedException();
      }
    }

    public IJournalManager GetJournalManager()
    {
      return new JournalManager(_connectionString);
    }

    public class JournalManager : IJournalManager
    {
      private readonly string _connectionString;

      public JournalManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Journal Create(Journal entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Journal> CreateAsync(Journal journal)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountId", journal.AccountId);
        p.Add("@Debit", journal.Debit);
        p.Add("@Credit", journal.Credit);
        p.Add("@Memo", journal.Memo);
        p.Add("@CreatedById", journal.CreatedById);
        p.Add("@OrganizationId", journal.OrganizationId);

        IEnumerable<Journal> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Journal>("""
            INSERT INTO "Journal" 
            ("AccountId", "Debit", "Credit", "Memo", "CreatedById", "OrganizationId") 
            VALUES 
            (@AccountId, @Debit, @Credit, @Memo, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Journal Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Journal> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<Journal> GetAsync(int journalId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@JournalId", journalId);
        p.Add("@OrganizationId", organizationId);

        Journal? result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QuerySingleOrDefaultAsync<Journal>("""
            SELECT * 
            FROM "Journal" 
            WHERE "JournalID" = @JournalId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result;
      }


      public async Task<List<Journal>> GetLedgerEntriesAsync(int[] journalIds, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@JournalIds", journalIds);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Journal> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Journal>("""
            SELECT * 
            FROM "Journal" 
            WHERE "JournalID" = ANY(@JournalIds)
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return result.ToList();
      }

      public async Task<bool> HasEntriesAsync(int accountId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@AccountId", accountId);
        p.Add("@OrganizationId", organizationId);

        int count;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          count = await con.ExecuteScalarAsync<int>("""
            SELECT COUNT(*) 
            FROM "Journal" 
            WHERE "AccountId" = @AccountId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return count > 0;
      }

      public int Update(Journal entity)
      {
        throw new NotImplementedException();
      }
    }

    public IInvoiceAttachmentManager GetInvoiceAttachmentManager()
    {
      return new InvoiceAttachmentManager(_connectionString);
    }

    public class InvoiceAttachmentManager : IInvoiceAttachmentManager
    {
      private readonly string _connectionString;

      public InvoiceAttachmentManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public InvoiceAttachment Create(InvoiceAttachment entity)
      {
        throw new NotImplementedException();
      }

      public async Task<InvoiceAttachment> CreateAsync(InvoiceAttachment entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", entity.InvoiceId);
        p.Add("@OriginalFileName", entity.OriginalFileName);
        p.Add("@FilePath", entity.FilePath);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<InvoiceAttachment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<InvoiceAttachment>("""
            INSERT INTO "InvoiceAttachment" 
            ("InvoiceId", "OriginalFileName", "FilePath", "CreatedById", "OrganizationId") 
            VALUES 
            (@InvoiceId, @OriginalFileName, @FilePath, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }


      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(int invoiceAttachmentID, int invoiceID, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceAttachmentID", invoiceAttachmentID);
        p.Add("@InvoiceId", invoiceID);
        p.Add("@OrganizationId", organizationId);
        
        int rowsAffected;
        
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "InvoiceAttachment" 
            WHERE "InvoiceAttachmentID" = @InvoiceAttachmentID
            AND "InvoiceId" = @InvoiceId
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<int> UpdateFilePathAsync(int invoiceAttachmentId, string newPath, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceAttachmentID", invoiceAttachmentId);
        p.Add("@FilePath", newPath);
        p.Add("@OrganizationId", organizationId);

        int rowsModified;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsModified = await con.ExecuteAsync("""
            UPDATE "InvoiceAttachment" 
            SET "FilePath" = @FilePath 
            WHERE "InvoiceAttachmentID" = @InvoiceAttachmentID 
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new InvoiceLineManager(_connectionString);
    }

    public class InvoiceLineManager : IInvoiceLineManager
    {
      private readonly string _connectionString;

      public InvoiceLineManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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
        p.Add("@RevenueAccountId", entity.RevenueAccountId);
        p.Add("@AssetsAccountId", entity.AssetsAccountId);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<InvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<InvoiceLine>("""
            INSERT INTO "InvoiceLine" 
            ("Title", "Description", "Quantity", "Price", "RevenueAccountId", "AssetsAccountId", "CreatedById", "OrganizationId", "InvoiceId")
            VALUES 
            (@Title, @Description, @Quantity, @Price, @RevenueAccountId, @AssetsAccountId, @CreatedById, @OrganizationId, @InvoiceId)
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new InvoiceManager(_connectionString);
    }

    public class InvoiceManager : IInvoiceManager
    {
      private readonly string _connectionString;

      public InvoiceManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public async Task<string> CalculateInvoiceStatusAsync(int invoiceId, int organizationId)
      {
        decimal invoiceTotal = 0;
        decimal receivedAmount = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
                FROM "JournalInvoiceInvoiceLine" AS GLIIL
                JOIN (
                    SELECT "InvoiceLineId", MAX("JournalInvoiceInvoiceLineID") AS MaxGLIILId
                    FROM "JournalInvoiceInvoiceLine"
                    GROUP BY "InvoiceLineId"
                ) AS LatestGLIIL ON GLIIL."InvoiceLineId" = LatestGLIIL."InvoiceLineId" 
                                 AND GLIIL."JournalInvoiceInvoiceLineID" = LatestGLIIL.MaxGLIILId
                WHERE GLIIL."InvoiceLineId" = "InvoiceLine"."InvoiceLineID"
                AND GLIIL."ReversedJournalInvoiceInvoiceLineId" IS NOT NULL
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
          return Invoice.InvoiceStatusConstants.Unpaid;
        }
        else if (receivedAmount < invoiceTotal)
        {
          return Invoice.InvoiceStatusConstants.PartiallyPaid;
        }
        else
        {
          return Invoice.InvoiceStatusConstants.Paid;
        }
      }

      public async Task<int> ComputeAndUpdateTotalAmountAndReceivedAmount(int invoiceId, int organizationId)
      {
        int affectedRows = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          string totalAmountQuery = """
            SELECT SUM("Quantity" * "Price")
            FROM "InvoiceLine"
            WHERE "InvoiceId" = @InvoiceId 
            AND "OrganizationId" = @OrganizationId
            AND NOT EXISTS (
                SELECT 1
                FROM "JournalInvoiceInvoiceLine" AS GLIIL
                JOIN (
                    SELECT "InvoiceLineId", MAX("JournalInvoiceInvoiceLineID") AS MaxGLIILId
                    FROM "JournalInvoiceInvoiceLine"
                    GROUP BY "InvoiceLineId"
                ) AS LatestGLIIL ON GLIIL."InvoiceLineId" = LatestGLIIL."InvoiceLineId" 
                                 AND GLIIL."JournalInvoiceInvoiceLineID" = LatestGLIIL.MaxGLIILId
                WHERE GLIIL."InvoiceLineId" = "InvoiceLine"."InvoiceLineID"
                AND GLIIL."ReversedJournalInvoiceInvoiceLineId" IS NOT NULL
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
        p.Add("@Status", Invoice.InvoiceStatusConstants.Unpaid);
        p.Add("@PaymentInstructions", entity.PaymentInstructions);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@TotalAmount", entity.TotalAmount);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<(List<Invoice> invoices, int? nextPage)> GetAllAsync(
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

        IEnumerable<Invoice> paginatedResult;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          paginatedResult = await con.QueryAsync<Invoice>($"""
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

        var result = paginatedResult.ToList();
        int? nextPage = null;

        if (result.Count > pageSize)
        {
          result.RemoveAt(result.Count - 1);
          nextPage = page + 1;
        }

        return (result, nextPage);
      }

      public async Task<List<Invoice>> GetAllAsync(int organizationId, string[] inStatus)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Statuses", inStatus);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (var con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
        p.Add("@Status", Invoice.InvoiceStatusConstants.Void);
        p.Add("@OrganizationId", organizationId);

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new InvoiceInvoiceLinePaymentManager(_connectionString);
    }

    public class InvoiceInvoiceLinePaymentManager : IInvoiceInvoiceLinePaymentManager
    {
      private readonly string _connectionString;

      public InvoiceInvoiceLinePaymentManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<(List<InvoiceInvoiceLinePayment> invoicePayments, int? nextPage)> GetAllAsync(int page, int pageSize, int organizationId, List<string> typesToLoad = null)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceInvoiceLinePayment> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
        int? nextPage = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPage = page + 1;
        }

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        return (resultList, nextPage);
      }

      public async Task<List<Invoice>> GetAllInvoicesByPaymentIdAsync(int paymentId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PaymentId", paymentId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Invoice> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<(List<InvoiceInvoiceLinePayment> invoicePayments, int? nextPage)> SearchInvoicePaymentsAsync(
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
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
        int? nextPage = null;

        if (resultList.Count > pageSize)
        {
          resultList.RemoveAt(resultList.Count - 1);
          nextPage = page + 1;
        }

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        return (resultList, nextPage);
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new ItemManager(_connectionString);
    }

    public class ItemManager : IItemManager
    {
      private readonly string _connectionString;

      public ItemManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Item Create(Item entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Item> CreateAsync(Item entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", entity.Name);
        p.Add("@Description", entity.Description);
        p.Add("@Quantity", entity.Quantity);
        p.Add("@AssemblyQuantity", entity.AssemblyQuantity);
        p.Add("@SellFor", entity.SellFor);
        p.Add("@InventoryMethod", entity.InventoryMethod);
        p.Add("@ItemType", entity.ItemType);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);
        p.Add("@RevenueAccountId", entity.RevenueAccountId);
        p.Add("@AssetsAccountId", entity.AssetsAccountId);
        p.Add("@ParentItemId", entity.ParentItemId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Item>("""
        INSERT INTO "Item" ("Name", "Description", "Quantity", "AssemblyQuantity", "SellFor", "InventoryMethod", "ItemType", "RevenueAccountId", "AssetsAccountId", "CreatedById", "OrganizationId", "ParentItemId")
        VALUES (@Name, @Description, @Quantity, @AssemblyQuantity, @SellFor, @InventoryMethod, @ItemType, @RevenueAccountId, @AssetsAccountId, @CreatedById, @OrganizationId, @ParentItemId)
        RETURNING *;
        """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(int itemId, bool deleteChildren)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemId", itemId);

        try
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            await con.OpenAsync();
            if (deleteChildren)
            {
              // Use a recursive CTE to find all descendants
              await con.ExecuteAsync("""
                  WITH RECURSIVE Descendants AS (
                    SELECT "ItemID" FROM "Item" WHERE "ParentItemId" = @ItemId
                    UNION
                    SELECT i."ItemID" FROM "Item" i
                    INNER JOIN Descendants d ON i."ParentItemId" = d."ItemID"
                  )
                  DELETE FROM "Item"
                  WHERE "ItemID" IN (SELECT "ItemID" FROM Descendants)
                  """, p);
            }

            // Delete the item itself
            int rowsAffected = await con.ExecuteAsync("""
                DELETE FROM "Item" 
                WHERE "ItemID" = @ItemId
                """, p);

            return rowsAffected;
          }
        }
        catch (PostgresException ex) when (ex.SqlState == "23503")
        {
          throw new InvalidOperationException("The item cannot be deleted because it is being used elsewhere. 23503.", ex);
        }
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Item>("""
        SELECT * 
        FROM "Item" 
        WHERE "OrganizationId" = @OrganizationId
        """, p);
        }

        return result.ToList();
      }

      public async Task<(List<Item> items, int? nextPage)> GetAllAsync(
        int page,
        int pageSize,
        int organizationId,
        bool includeDescendants,
        bool includeInventories)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize + 1);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Item>($"""
        SELECT *, ROW_NUMBER() OVER (ORDER BY "ItemID") AS "RowNumber"
        FROM "Item"
        WHERE "OrganizationId" = @OrganizationId AND "ParentItemId" IS NULL
        ORDER BY "Name"
        LIMIT @PageSize OFFSET @Offset
        """, new { PageSize = pageSize + 1, Offset = pageSize * (page - 1), OrganizationId = organizationId });
        }

        var hasMoreRecords = result.Count() > pageSize;

        if (hasMoreRecords)
        {
          result = result.Take(pageSize);
        }

        int? nextPage = hasMoreRecords ? page + 1 : null;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          if (includeDescendants)
          {
            var allItems = await con.QueryAsync<Item>($"""
          SELECT * 
          FROM "Item"
          WHERE "OrganizationId" = @OrganizationId
          """, p);

            void PopulateChildrenRecursively(List<Item> children)
            {
              foreach (var child in children)
              {
                child.Children = allItems.Where(x => x.ParentItemId == child.ItemID).OrderBy(x => x.Name).ToList();
                if (child.Children.Any())
                {
                  PopulateChildrenRecursively(child.Children);
                }
              }
            }

            foreach (var item in result)
            {
              item.Children = allItems.Where(x => x.ParentItemId == item.ItemID).OrderBy(x => x.Name).ToList();
              if (item.Children.Any())
              {
                PopulateChildrenRecursively(item.Children);
              }
            }
          }

          async Task LoadInventoriesAsync(NpgsqlConnection con, Item item, int organizationId)
          {
            var inventories = await con.QueryAsync<Inventory, Item, Location, Inventory>($"""
          SELECT i.*, it.*, l.*
          FROM "Inventory" i
          INNER JOIN "Item" it ON i."ItemId" = it."ItemID"
          INNER JOIN "Location" l ON i."LocationId" = l."LocationID"
          WHERE i."ItemId" = @ItemId
          AND i."OrganizationId" = @OrganizationId
          """,
                (inventory, inventoryItem, location) =>
                {
                  inventory.Location = location;
                  return inventory;
                },
                new { ItemId = item.ItemID, OrganizationId = organizationId },
                splitOn: "ItemID,LocationID");

            item.Inventories = inventories.ToList();

            // Recursively load inventories for children
            foreach (var child in item.Children)
            {
              await LoadInventoriesAsync(con, child, organizationId);
            }
          }

          if (includeInventories)
          {
            foreach (var item in result)
            {
              await LoadInventoriesAsync(con, item, organizationId);
            }
          }
        }

        return (result.ToList(), nextPage);
      }

      public async Task<Item?> GetAsync(int itemId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ItemId", itemId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Item> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new OrganizationManager(_connectionString);
    }

    public class OrganizationManager : IOrganizationManager
    {
      private readonly string _connectionString;

      public OrganizationManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Organization Create(Organization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Organization> CreateAsync(Organization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Organization> CreateAsync(string organizationName, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", organizationName);

        string sql = """
          INSERT INTO "Organization" ("Name") 
          VALUES (@Name) 
          RETURNING *;
          """;

        IEnumerable<Organization> result;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        string connectionString = builder.ConnectionString;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
        {
          result = await con.QueryAsync<Organization>(sql, p);
        }

        return result.Single();
      }

      public async Task<Organization> CreateAsync(string organizationName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", organizationName);

        IEnumerable<Organization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Organization>("""
            INSERT INTO "Organization" ("Name") 
            VALUES (@Name) 
            RETURNING *;
            """, p);
        }

        return result.Single();
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

      public async Task<Organization> GetAsync(int organizationId, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Organization>? result;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        string connectionString = builder.ConnectionString;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          return await con.ExecuteAsync($"""
            UPDATE "Organization" 
            SET "Website" = @Website 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }
      }

      public async Task<Organization> GetAsync(string name, bool searchTenants)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Name", name);

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          var organization = (await con.QueryAsync<Organization>("""
            SELECT * 
            FROM "Organization" 
            WHERE "Name" = @Name
            """, p)).FirstOrDefault();

          if (organization != null)
            return organization;
        }

        if (searchTenants)
        {
          TenantManager tenantManager = new TenantManager(_connectionString);
          var tenants = await tenantManager.GetAllAsync();

          var builder = new NpgsqlConnectionStringBuilder(_connectionString);

          foreach (var tenant in tenants)
          {
            if (string.IsNullOrEmpty(tenant.DatabaseName))
              continue;

            builder.Database = tenant.DatabaseName;
            using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
            {
              var organization = (await con.QueryAsync<Organization>("""
                    SELECT * 
                    FROM "Organization"
                    WHERE "Name" = @Name
                    """, p)).FirstOrDefault();

              if (organization != null)
                return organization;
            }
          }
        }

        return null!;
      }

      public async Task<Organization> GetAsync(string name)
      {
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          DynamicParameters p = new DynamicParameters();
          p.Add("@Name", name);

          var organization = (await con.QueryAsync<Organization>("""
            SELECT * 
            FROM "Organization" 
            WHERE "Name" = @Name
            """, p)).FirstOrDefault();

          return organization;
        }
      }

      public async Task<List<Organization>> GetAllAsync()
      {
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          var organizations = await con.QueryAsync<Organization>("""
            SELECT * 
            FROM "Organization"
            """);

          return organizations.ToList();
        }
      }

      public async Task<int> UpdateAsync(int organizationId, string name, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);
        p.Add("@Name", name);

        int rowsAffected;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        string connectionString = builder.ConnectionString;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Organization" 
            SET "Name" = @Name 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> DeleteAsync(int organizationId, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        string connectionString = builder.ConnectionString;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "Organization" 
            WHERE "OrganizationID" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task InsertSampleOrganizationDataAsync(string sampleSqlDataToInsert)
      {
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          await con.ExecuteAsync(sampleSqlDataToInsert);
        }
      }
    }

    public IPaymentInstructionManager GetPaymentInstructionManager()
    {
      return new PaymentInstructionManager(_connectionString);
    }

    public class PaymentInstructionManager : IPaymentInstructionManager
    {
      private readonly string _connectionString;

      public PaymentInstructionManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection())
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new PaymentManager(_connectionString);
    }

    public class PaymentManager : IPaymentManager
    {
      private readonly string _connectionString;

      public PaymentManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new PaymentTermManager(_connectionString);
    }

    public class PaymentTermManager : IPaymentTermManager
    {
      private readonly string _connectionString;

      public PaymentTermManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new ReconciliationManager(_connectionString);
    }

    public class ReconciliationManager : IReconciliationManager
    {
      private readonly string _connectionString;

      public ReconciliationManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new ReconciliationTransactionManager(_connectionString);
    }

    public class ReconciliationTransactionManager : IReconciliationTransactionManager
    {
      private readonly string _connectionString;

      public ReconciliationTransactionManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<(List<ReconciliationTransaction> reconciliationTransactions, int? nextPage)> GetReconciliationTransactionAsync(int reconciliationId, int page, int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationId", reconciliationId);
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);

        IEnumerable<ReconciliationTransaction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<int> UpdateAssetOrLiabilityAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationLiabilitiesAndAssetsAccountId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);
        p.Add("@AssetOrLiabilityAccountId", selectedReconciliationLiabilitiesAndAssetsAccountId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "ReconciliationTransaction" 
            SET "AssetOrLiabilityAccountId" = @AssetOrLiabilityAccountId
            WHERE "ReconciliationTransactionID" = @ReconciliationTransactionID;
            """, p);

          return rowsAffected;
        }
      }

      public async Task<int> UpdateExpenseAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationExpenseAccountId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionID", reconciliationTransactionID);
        p.Add("@ExpenseAccountId", selectedReconciliationExpenseAccountId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "ReconciliationTransaction" 
            SET "ExpenseAccountId" = @ExpenseAccountId
            WHERE "ReconciliationTransactionID" = @ReconciliationTransactionID;
            """, p);

          return rowsAffected;
        }
      }
    }

    public IReconiliationAttachmentManager GetReconiliationAttachmentManager()
    {
      return new ReconiliationAttachmentManager(_connectionString);
    }

    public class ReconiliationAttachmentManager : IReconiliationAttachmentManager
    {
      private readonly string _connectionString;

      public ReconiliationAttachmentManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new TagManager(_connectionString);
    }

    public class TagManager : ITagManager
    {
      private readonly string _connectionString;

      public TagManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new ToDoManager(_connectionString);
    }

    public class ToDoManager : IToDoManager
    {
      private readonly string _connectionString;

      public ToDoManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new UserManager(_connectionString);
    }

    public class UserManager : IUserManager
    {
      private readonly string _connectionString;

      public UserManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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
        p.Add("@Password", entity.Password);

        string sql = """
          INSERT INTO "User" ("Email", "FirstName", "LastName", "Password")
          VALUES (@Email, @FirstName, @LastName, @Password)
          RETURNING *;
          """;

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<User>(sql, p);
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<User>("""
              SELECT * 
              FROM "User" 
              WHERE "UserID" = @UserId
              """, p);
        }

        return result.SingleOrDefault()!;
      }

      public async Task<(User, Tenant)> GetFirstOfAnyTenantAsync(string email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = DatabaseThing.DatabaseConstants.DatabaseName;

        TenantManager tenantManager = new TenantManager(builder.ConnectionString);
        var tenants = await tenantManager.GetAllAsync();

        foreach (var tenant in tenants)
        {
          if (string.IsNullOrEmpty(tenant.DatabaseName))
            continue;

          builder.Database = tenant.DatabaseName;
          builder.Password = tenant.DatabasePassword;

          using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
          {
            var userOrganizations = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>("""
                    SELECT uo.*, u.*, o.*
                    FROM "User" u
                    LEFT JOIN "UserOrganization" uo ON u."UserID" = uo."UserId"
                    LEFT JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
                    WHERE u."Email" = @Email
                    """,
                (userOrg, user, org) =>
                {
                  userOrg.User = user;
                  userOrg.Organization = org;
                  return userOrg;
                },
                p,
                splitOn: "UserID,OrganizationID"
            );

            var user = userOrganizations.FirstOrDefault()?.User;
            if (user != null)
              return (user, tenant);
          }
        }

        return (null!, null!);
      }

      public int Update(User entity)
      {
        throw new NotImplementedException();
      }

      public async Task<User> GetAsync(string email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<User> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<User>("""
              SELECT * 
              FROM "User" 
              WHERE "Email" = @Email
              """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<int> UpdatePasswordAllTenantsAsync(string email, string password)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);
        p.Add("@Password", password);

        int rowsModified = 0;

        TenantManager tenantManager = new TenantManager(_connectionString);
        var tenants = await tenantManager.GetAllAsync();

        foreach (var tenant in tenants)
        {
          if (string.IsNullOrEmpty(tenant.DatabaseName))
            continue;

          var builder = new NpgsqlConnectionStringBuilder(_connectionString)
          {
            Database = tenant.DatabaseName,
            Password = tenant.DatabasePassword
          };

          using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
          {
            rowsModified += await con.ExecuteAsync("""
                  UPDATE "User" 
                  SET "Password" = @Password
                  WHERE "Email" = @Email
                  """, p);
          }
        }

        return rowsModified;
      }

      public async Task<int> DeleteAsync(int userId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserID", userId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
              DELETE FROM "User" 
              WHERE "UserID" = @UserID
              """, p);
        }

        return rowsAffected;
      }

      public async Task<(List<User> users, int? nextPageNumber)> GetAllAsync(int page, int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);

        IEnumerable<User> paginatedResult;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          paginatedResult = await con.QueryAsync<User>($"""
          SELECT * FROM (
              SELECT *,
                     ROW_NUMBER() OVER (ORDER BY "UserID" DESC) AS RowNumber
              FROM "User"
          ) AS NumberedUsers
          WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
          """, p);
        }

        var result = paginatedResult.ToList();
        int? nextPageNumber = null;

        if (result.Count > pageSize)
        {
          result.RemoveAt(result.Count - 1);
          nextPageNumber = page + 1;
        }

        return (result, nextPageNumber);
      }

      public async Task<List<User>> GetFilteredAsync(string search)
      {
        if (string.IsNullOrWhiteSpace(search))
        {
          return new List<User>();
        }

        var searchParts = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (!searchParts.Any())
        {
          return new List<User>();
        }

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        for (int i = 0; i < searchParts.Length; i++)
        {
          string paramName = $"@Search{i}";
          parameters.Add(paramName, $"%{searchParts[i]}%");
          conditions.Add($@"""Email"" ILIKE {paramName} OR ""FirstName"" ILIKE {paramName} OR ""LastName"" ILIKE {paramName}");
        }

        string query = $"""
          SELECT *
          FROM "User"
          WHERE {string.Join(" OR ", conditions)}
          LIMIT 100
          """;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          var result = await con.QueryAsync<User>(query, parameters);
          return result.ToList();
        }
      }

      public async Task<IEnumerable<UserReferenceInfo>> GetUserReferencesAsync(int userId)
      {
        using var con = new NpgsqlConnection(_connectionString);
        DynamicParameters p = new();
        p.Add("@UserID", userId);

        var foreignKeyQuery = """
          SELECT n.nspname AS schema_name,
                 c.relname AS table_name,
                 a.attname AS column_name
          FROM pg_constraint AS con
          JOIN pg_class AS c ON con.conrelid = c.oid
          JOIN pg_namespace AS n ON c.relnamespace = n.oid
          JOIN pg_attribute AS a ON a.attrelid = c.oid AND a.attnum = ANY(con.conkey)
          WHERE con.confrelid = '"User"'::regclass;
          """;

        var references = await con.QueryAsync<(string Schema, string Table, string Column)>(foreignKeyQuery);
        var results = new List<UserReferenceInfo>();

        foreach (var (schema, table, column) in references)
        {
          var countQuery = $"""
            SELECT COUNT(*)
            FROM "{schema}"."{table}"
            WHERE "{column}" = @UserID
            """;

          int count = await con.ExecuteScalarAsync<int>(countQuery, p);
          if (count > 0)
          {
            results.Add(new UserReferenceInfo
            {
              Schema = schema,
              Table = table,
              Column = column,
              ReferenceCount = count
            });
          }
        }

        return results;
      }
    }

    public IUserOrganizationManager GetUserOrganizationManager()
    {
      return new UserOrganizationManager(_connectionString);
    }

    public class UserOrganizationManager : IUserOrganizationManager
    {
      private readonly string _connectionString;

      public UserOrganizationManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public UserOrganization Create(UserOrganization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<UserOrganization> CreateAsync(UserOrganization entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", entity.UserId);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<UserOrganization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<UserOrganization>($"""
            INSERT INTO "UserOrganization" ("UserId", "OrganizationId") 
            VALUES (@UserId, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public async Task<UserOrganization> CreateAsync(
        UserOrganization userOrganization,
        string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", userOrganization.UserId);
        p.Add("@OrganizationId", userOrganization.OrganizationId);

        IEnumerable<UserOrganization> result;


        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<UserOrganization>($"""
            INSERT INTO "UserOrganization" ("UserId", "OrganizationId") 
            VALUES (@UserId, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public async Task<UserOrganization> CreateAsync(int userID, int organizationId, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", userID);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<UserOrganization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<int> DeleteByOrganizationIdAsync(int organizationId, string databaseName, string databasePassword)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        builder.Password = databasePassword;
        string connectionString = builder.ConnectionString;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "UserOrganization" 
            WHERE "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> DeleteUserAsync(int userId, string databasePassword, string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", userId);

        int rowsAffected;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        builder.Password = databasePassword;

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "UserOrganization" 
            WHERE "UserId" = @UserId
            """, p);
        }

        return rowsAffected;
      }

      public UserOrganization Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<UserOrganization> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<UserOrganization>> GetAllAsync(int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantId", tenantId);

        IEnumerable<Tenant> tenants;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          tenants = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "TenantID" = @TenantId
            """, p);
        }

        Tenant tenant = tenants.Single();

        string databaseName = tenant.DatabaseName;

        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        builder.Password = tenant.DatabasePassword;
        string connectionString = builder.ConnectionString;

        IEnumerable<UserOrganization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
        {
          result = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>("""
            SELECT uo.*, u.*, o.*
            FROM "UserOrganization" uo
            INNER JOIN "User" u ON uo."UserId" = u."UserID"
            INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
            """,
            (uo, u, o)
              =>
            {
              uo.User = u;
              uo.Organization = o;
              return uo;
            }, splitOn: "UserID,OrganizationID");
        }

        return result.ToList();
      }

      public async Task<UserOrganization> GetAsync(int userId, int organizationId)
      {
        var p = new DynamicParameters();
        p.Add("UserID", userId);
        p.Add("OrganizationId", organizationId);

        IEnumerable<UserOrganization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>("""
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

      public async Task<List<(Organization Organization, Tenant? Tenant)>> GetByEmailAsync(string email, bool searchTenants)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        List<(Organization Organization, Tenant? Tenant)> organizationsWithTenants = new List<(Organization, Tenant?)>();

        if (!searchTenants)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            var userOrganizations = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>("""
                SELECT uo.*, u.*, o.*
                FROM "UserOrganization" uo
                INNER JOIN "User" u ON uo."UserId" = u."UserID"
                INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
                WHERE u."Email" = @Email
                """,
                (userOrg, user, org) =>
                {
                  userOrg.User = user;
                  userOrg.Organization = org;
                  return userOrg;
                }, p, splitOn: "UserID,OrganizationID"
            );

            foreach (var userOrg in userOrganizations)
            {
              var org = userOrg.Organization;
              Tenant? tenant = null;

              if (org.TenantId.HasValue)
              {
                tenant = await con.QuerySingleOrDefaultAsync<Tenant>("""
                        SELECT *
                        FROM "Tenant"
                        WHERE "TenantID" = @TenantId
                        """, new { TenantId = org.TenantId.Value });
              }

              organizationsWithTenants.Add((org, tenant));
            }
          }
        }

        if (searchTenants)
        {
          var builder = new NpgsqlConnectionStringBuilder(_connectionString);
          builder.Database = DatabaseThing.DatabaseConstants.DatabaseName;

          TenantManager tenantManager = new TenantManager(builder.ConnectionString);
          var tenants = await tenantManager.GetAllAsync();

          foreach (var tenant in tenants)
          {
            if (string.IsNullOrEmpty(tenant.DatabaseName))
              continue;

            builder.Database = tenant.DatabaseName;
            builder.Password = tenant.DatabasePassword;
            using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
            {
              var userOrganizations = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>("""
                    SELECT uo.*, u.*, o.*
                    FROM "UserOrganization" uo
                    INNER JOIN "User" u ON uo."UserId" = u."UserID"
                    INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
                    WHERE u."Email" = @Email
                    """,
                  (userOrg, user, org) =>
                  {
                    userOrg.User = user;
                    userOrg.Organization = org;
                    return userOrg;
                  }, p, splitOn: "UserID,OrganizationID"
              );

              foreach (var userOrg in userOrganizations)
              {
                organizationsWithTenants.Add((userOrg.Organization, tenant));
              }
            }
          }
        }

        return organizationsWithTenants;
      }

      public async Task<UserOrganization?> GetByEmailAsync(
        string email,
        int? selectedOrganizationId,
        int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<UserOrganization?> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          if (tenantId > 0)
          {
            TenantManager tenantManager = new TenantManager(_connectionString);
            Tenant tenant = await tenantManager.GetAsync(tenantId);

            if (tenant != null && !string.IsNullOrEmpty(tenant.DatabaseName))
            {
              NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString);
              builder.Database = tenant.DatabaseName;
              builder.Password = tenant.DatabasePassword;

              using (NpgsqlConnection tenantCon = new NpgsqlConnection(builder.ConnectionString))
              {
                result = await tenantCon.QueryAsync<UserOrganization, User, Organization, UserOrganization>(
                  """
                  SELECT uo.*, u.*, o.*
                  FROM "UserOrganization" uo
                  INNER JOIN "User" u ON uo."UserId" = u."UserID"
                  INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
                  WHERE u."Email" = @Email
                    AND o."OrganizationID" = @OrganizationId
                  """,
                    (uo, u, o) =>
                  {
                    uo.User = u;
                    uo.Organization = o;
                    return uo;
                  },
                  new { Email = email, OrganizationId = selectedOrganizationId },
                  splitOn: "UserID,OrganizationID"
                );
              }
            }
            else
            {
              throw new System.Exception("Tenant not found or invalid tenant configuration.");
            }
          }
          else
          {
            result = await con.QueryAsync<UserOrganization, User, Organization, UserOrganization>("""
              SELECT uo.*, u.*, o.*
              FROM "UserOrganization" uo
              INNER JOIN "User" u ON uo."UserId" = u."UserID"
              INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
              WHERE u."Email" = @Email
              """, (uo, u, o) =>
            {
              uo.User = u;
              uo.Organization = o;
              return uo;
            }, p);
          }
        }

        return result.Single();
      }

      public async Task<List<Organization>> GetByUserIdAsync(int userId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@UserId", userId);

        IEnumerable<Organization> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Organization>("""
          select o.* from "Organization" o
          inner join "UserOrganization" uo on uo."OrganizationId" = o."OrganizationID"
          where uo."UserId" = @UserId
          """, p);
        }

        return result.ToList();
      }

      public async Task<List<User>> GetUsersWithOrganizationsAsync(string databaseName)
      {
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          // Step 1: Fetch all users
          var users = (await con.QueryAsync<User>(
              """
            SELECT "UserID", "Email", "FirstName", "LastName", "Created", "CreatedById"
            FROM "User"
            """
          )).ToList();

          if (users.Count == 0)
            return users;

          // Step 2: Fetch all organizations for the users
          var userIds = users.Select(u => u.UserID).ToArray();
          var organizations = await con.QueryAsync<User, Organization, User>(
              """
            SELECT uo."UserId", o."OrganizationID", o."Name", o."Address", o."BaseCurrency", o."Website"
            FROM "UserOrganization" uo
            INNER JOIN "Organization" o ON uo."OrganizationId" = o."OrganizationID"
            WHERE uo."UserId" = ANY(@UserIds)
            """,
              (user, organization) =>
              {
                return new User
                {
                  UserID = user.UserID,
                  Organizations = new List<Organization> { organization }
                };
              },
              new { UserIds = userIds },
              splitOn: "OrganizationID"
          );

          // Step 3: Map organizations to their respective users
          var organizationsByUserId = organizations
              .GroupBy(o => o.UserID)
              .ToDictionary(g => g.Key, g => g.SelectMany(u => u.Organizations).ToList());

          foreach (var user in users)
          {
            if (organizationsByUserId.TryGetValue(user.UserID, out var orgList))
            {
              user.Organizations = orgList;
            }
          }

          return users;
        }
      }

      public int Update(UserOrganization entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateUserOrganizationsAsync(int userId, List<int> selectedOrganizationIds, string databasePassword, string databaseName)
      {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = databaseName;
        builder.Password = databasePassword;

        using var con = new NpgsqlConnection(builder.ConnectionString);

        var rowsAffected = 0;

        if (!selectedOrganizationIds.Any())
        {
          rowsAffected += await con.ExecuteAsync("""
            DELETE FROM "UserOrganization"
            WHERE "UserId" = @UserId;
            """, new { UserId = userId });
        }
        else
        {
          var p = new DynamicParameters();
          p.Add("UserId", userId);
          p.Add("SelectedOrganizationIds", selectedOrganizationIds.ToArray());

          rowsAffected += await con.ExecuteAsync("""
            DELETE FROM "UserOrganization"
            WHERE "UserId" = @UserId
            AND "OrganizationId" NOT IN (SELECT UNNEST(@SelectedOrganizationIds));
            """, p);

          rowsAffected += await con.ExecuteAsync("""
            INSERT INTO "UserOrganization" ("UserId", "OrganizationId")
            SELECT @UserId, OrganizationId
            FROM UNNEST(@SelectedOrganizationIds) AS OrganizationId
            WHERE NOT EXISTS (
                SELECT 1 
                FROM "UserOrganization" 
                WHERE "UserId" = @UserId AND "OrganizationId" = OrganizationId
            );
            """, p);
        }

        return rowsAffected;
      }
    }

    public IReconciliationExpenseManager GetExpenseManager()
    {
      return new ReconciliationExpenseManager(_connectionString);
    }

    public class ReconciliationExpenseManager : IReconciliationExpenseManager
    {
      private readonly string _connectionString;

      public ReconciliationExpenseManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

    public IJournalReconciliationTransactionManager GetJournalReconciliationExpenseManager()
    {
      return new JournalReconciliationTransactionManager(_connectionString);
    }

    public class JournalReconciliationTransactionManager : IJournalReconciliationTransactionManager
    {
      private readonly string _connectionString;

      public JournalReconciliationTransactionManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public JournalReconciliationTransaction Create(JournalReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }

      public async Task<JournalReconciliationTransaction> CreateAsync(JournalReconciliationTransaction entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@JournalId", entity.JournalId);
        p.Add("@ReconciliationTransactionId", entity.ReconciliationTransactionId);
        p.Add("@ReversedJournalReconciliationTransactionId", entity.ReversedJournalReconciliationTransactionId);
        p.Add("@TransactionGuid", entity.TransactionGuid);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<JournalReconciliationTransaction> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<JournalReconciliationTransaction>("""
            INSERT INTO "JournalReconciliationTransaction" 
            ("JournalId", "ReversedJournalReconciliationTransactionId", "ReconciliationTransactionId", "TransactionGuid", "CreatedById", "OrganizationId") 
            VALUES 
            (@JournalId, @ReversedJournalReconciliationTransactionId, @ReconciliationTransactionId, @TransactionGuid, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public JournalReconciliationTransaction Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<JournalReconciliationTransaction> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<JournalReconciliationTransaction>> GetLastTransactionAsync(
        int reconciliationTransactionId,
        int organizationId,
        bool loadChildren = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ReconciliationTransactionId", reconciliationTransactionId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<JournalReconciliationTransaction> result;

        if (loadChildren)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            result = await con.QueryAsync<JournalReconciliationTransaction, Journal, JournalReconciliationTransaction>("""
              SELECT glrt.*, gl.*
              FROM "JournalReconciliationTransaction" glrt
              JOIN "Journal" gl ON glrt."JournalId" = gl."JournalID"
              WHERE glrt."TransactionGuid" IN (
                  SELECT "TransactionGuid"
                  FROM "JournalReconciliationTransaction"
                  WHERE "ReconciliationTransactionId" = @ReconciliationTransactionId
                  AND "OrganizationId" = @OrganizationId
                  ORDER BY "JournalReconciliationTransactionID" DESC
                  LIMIT 1
              );
              """, (glrt, gl) =>
            {
              glrt.Journal = gl;
              return glrt;
            }, p, splitOn: "JournalID");
          }
        }
        else
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            result = await con.QueryAsync<JournalReconciliationTransaction>("""
              SELECT *
              FROM "JournalReconciliationTransaction"
              WHERE "TransactionGuid" IN (
                  SELECT "TransactionGuid"
                  FROM "JournalReconciliationTransaction"
                  WHERE "ReconciliationTransactionId" = @ReconciliationTransactionId
                  AND "OrganizationId" = @OrganizationId
                  ORDER BY "JournalReconciliationTransactionID" DESC
                  LIMIT 1
              );
              """, p);
          }
        }

        return result.ToList();
      }

      public int Update(JournalReconciliationTransaction entity)
      {
        throw new NotImplementedException();
      }
    }

    public IReconciliationExpenseCategoryManager GetReconciliationExpenseCategoryManager()
    {
      return new ReconciliationExpenseCategoryManager(_connectionString);
    }

    public class ReconciliationExpenseCategoryManager : IReconciliationExpenseCategoryManager
    {
      private readonly string _connectionString;

      public ReconciliationExpenseCategoryManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new DatabaseManager(_connectionString);
    }

    public class DatabaseManager : IDatabaseManager
    {
      private readonly string _connectionString;

      public DatabaseManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public DatabaseThing Create(DatabaseThing entity)
      {
        throw new NotImplementedException();
      }

      public Task<DatabaseThing> CreateAsync(DatabaseThing entity)
      {
        throw new NotImplementedException();
      }

      public async Task<DatabaseThing> CreateDatabase(string tenantId)
      {
        string databaseName = $"{PrefixConstants.TenantDatabasePrefix}{tenantId}";

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = DatabaseThing.DatabaseConstants.DatabaseNameAdmin;
        //builder.Password = ConfigurationSingleton.Instance.DatabasePassword;

        using (var con = new NpgsqlConnection(builder.ConnectionString))
        {
          await con.ExecuteAsync($$"""
            CREATE DATABASE {{databaseName}}
            WITH
                OWNER = postgres
                TEMPLATE = template0
                ENCODING = 'UTF8'
                LC_COLLATE = 'en_US.utf8'
                LC_CTYPE = 'en_US.utf8'
                CONNECTION LIMIT = -1;
            """);

          string getDatabaseInfoCommand = $$"""
            SELECT
                datname AS Name,
                datdba::regrole::text AS Owner,
                pg_encoding_to_char(encoding) AS Encoding,
                datcollate AS Collation,
                datctype AS Ctype,
                datconnlimit AS ConnectionLimit
            FROM
                pg_database
            WHERE
                datname = '{{databaseName}}';
            """;

          DatabaseThing databaseThing = await con.QuerySingleAsync<DatabaseThing>(getDatabaseInfoCommand);

          return databaseThing;
        }
      }

      public int Delete(string databaseName)
      {
        throw new NotImplementedException();
      }

      public async Task DeleteAsync(string databaseName)
      {
        string sanitizedDbName = Regex.Replace(databaseName, @"[^a-zA-Z0-9_]", "");

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = DatabaseThing.DatabaseConstants.DatabaseNameAdmin;
        builder.Password = ConfigurationSingleton.Instance.DatabasePassword;

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          await con.ExecuteAsync($"""
            DROP DATABASE IF EXISTS "{sanitizedDbName}" WITH (FORCE);
            """);
        }
      }

      public DatabaseThing Get(string databaseName)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<DatabaseThing> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task ResetDatabaseAsync()
      {
        string resetCreateDbScriptPath = Path.Combine(AppContext.BaseDirectory, "reset-and-create-database.sql");
        string createSchemaScriptPath = Path.Combine(AppContext.BaseDirectory, "create-db-script-psql.sql");

        string resetCreateDbScript = System.IO.File.ReadAllText(resetCreateDbScriptPath);
        string createSchemaScript = System.IO.File.ReadAllText(createSchemaScriptPath);

        //modify connection string to use admin database
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        builder.Database = DatabaseThing.DatabaseConstants.DatabaseNameAdmin;
        //builder.Password = ConfigurationSingleton.Instance.DatabasePassword;

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          await con.ExecuteAsync(resetCreateDbScript);
        }

        builder.Database = DatabaseThing.DatabaseConstants.DatabaseName;

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          await con.ExecuteAsync(createSchemaScript);
        }
      }

      public async Task RunSQLScript(string script, string databaseName)
      {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);

        builder.Database = databaseName;

        var updatedConnectionString = builder.ToString();

        using (NpgsqlConnection con = new NpgsqlConnection(updatedConnectionString))
        {
          await con.ExecuteAsync(script);
        }
      }

      public int Update(DatabaseThing entity)
      {
        throw new NotImplementedException();
      }
    }

    public IJournalInvoiceInvoiceLineManager GetJournalInvoiceInvoiceLineManager()
    {
      return new JournalInvoiceInvoiceLineManager(_connectionString);
    }

    public class JournalInvoiceInvoiceLineManager : IJournalInvoiceInvoiceLineManager
    {
      private readonly string _connectionString;

      public JournalInvoiceInvoiceLineManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public JournalInvoiceInvoiceLine Create(JournalInvoiceInvoiceLine entity)
      {
        throw new NotImplementedException();
      }

      public async Task<JournalInvoiceInvoiceLine> CreateAsync(JournalInvoiceInvoiceLine entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@JournalId", entity.JournalId);
        p.Add("@InvoiceId", entity.InvoiceId);
        p.Add("@InvoiceLineId", entity.InvoiceLineId);
        p.Add("@ReverseJournalInvoiceInvoiceLineId", entity.ReversedJournalInvoiceInvoiceLineId);
        p.Add("@TransactionGuid", entity.TransactionGuid);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<JournalInvoiceInvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<JournalInvoiceInvoiceLine>("""
            INSERT INTO "JournalInvoiceInvoiceLine" 
            ("JournalId", "InvoiceId", "InvoiceLineId", "ReversedJournalInvoiceInvoiceLineId", "TransactionGuid", "CreatedById", "OrganizationId")
            VALUES 
            (@JournalId, @InvoiceId, @InvoiceLineId, @ReverseJournalInvoiceInvoiceLineId, @TransactionGuid, @CreatedById, @OrganizationId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public JournalInvoiceInvoiceLine Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<JournalInvoiceInvoiceLine> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<JournalInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);
        p.Add("@IncludeRemoved", includeRemoved);

        IEnumerable<JournalInvoiceInvoiceLine> result;

        throw new NotImplementedException();

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<JournalInvoiceInvoiceLine>("""

            """, p);
        }
      }

      public async Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId, int organizationId, bool onlyCurrent = false)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceId", invoiceId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<InvoiceLine> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
                    FROM "JournalInvoiceInvoiceLine" gliil
                    INNER JOIN (
                        SELECT "InvoiceLineId", MAX("JournalInvoiceInvoiceLineID") AS MaxGLIILID
                        FROM "JournalInvoiceInvoiceLine"
                        GROUP BY "InvoiceLineId"
                    ) AS LastTransactions ON gliil."InvoiceLineId" = LastTransactions."InvoiceLineId" AND gliil."JournalInvoiceInvoiceLineID" = LastTransactions.MaxGLIILID
                    WHERE gliil."InvoiceLineId" = il."InvoiceLineID"
                    AND gliil."ReversedJournalInvoiceInvoiceLineId" IS NULL
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

      public async Task<List<JournalInvoiceInvoiceLine>> GetLastTransactionAsync(
        int invoiceLineId,
        int organizationId,
        bool loadChildren)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@InvoiceLineId", invoiceLineId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<JournalInvoiceInvoiceLine> result;

        if (loadChildren)
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            string query = """
              SELECT
                  gliil.*,
                  gl.*
              FROM "JournalInvoiceInvoiceLine" gliil
              INNER JOIN "Journal" gl ON gliil."JournalId" = gl."JournalID"
              WHERE 
                gliil."TransactionGuid" = (
                  SELECT "TransactionGuid"
                  FROM "JournalInvoiceInvoiceLine"
                  WHERE "InvoiceLineId" = @InvoiceLineId
                  ORDER BY "JournalInvoiceInvoiceLineID" DESC
                  LIMIT 1
                )
                AND "InvoiceLineId" = @InvoiceLineId
                AND gliil."ReversedJournalInvoiceInvoiceLineId" IS NULL
                AND gliil."OrganizationId" = @OrganizationId
              """;

            result = await con.QueryAsync<JournalInvoiceInvoiceLine, Journal, JournalInvoiceInvoiceLine>(query, (gliil, gl) =>
            {
              gliil.Journal = gl;
              return gliil;
            }, p, splitOn: "JournalID");
          }
        }
        else
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            string query = """
              SELECT *
              FROM "JournalInvoiceInvoiceLine"
              WHERE "TransactionGuid" = (
                SELECT "TransactionGuid"
                FROM "JournalInvoiceInvoiceLine"
                WHERE "InvoiceLineId" = @InvoiceLineId
                ORDER BY "Created" DESC
                LIMIT 1
              )
              AND "ReversedJournalInvoiceInvoiceLineId" IS NULL
              AND "OrganizationId" = @OrganizationId
              """;

            result = await con.QueryAsync<JournalInvoiceInvoiceLine>(query, p);
          }
        }

        return result.ToList();
      }

      public int Update(JournalInvoiceInvoiceLine entity)
      {
        throw new NotImplementedException();
      }
    }



    public IInventoryManager GetInventoryManager()
    {
      return new InventoryManager(_connectionString);
    }

    public class InventoryManager : IInventoryManager
    {
      private readonly string _connectionString;

      public InventoryManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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
        p.Add("@SellFor", entity.SellFor);
        p.Add("@CreatedById", entity.CreatedById);
        p.Add("@OrganizationId", entity.OrganizationId);

        IEnumerable<Inventory> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Inventory>("""
            INSERT INTO "Inventory" ("ItemId", "LocationId", "Quantity", "SellFor", "CreatedById", "OrganizationId") 
            VALUES (@ItemId, @LocationId, @Quantity, @SellFor, @CreatedById, @OrganizationId)
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new RequestLogManager(_connectionString);
    }

    public class RequestLogManager : IRequestLogManager
    {
      private readonly string _connectionString;

      public RequestLogManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<(IEnumerable<RequestLog> requestLogs, int? nextPage)> GetAllAsync(int page, int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);

        IEnumerable<RequestLog> paginatedResult;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          paginatedResult = await con.QueryAsync<RequestLog>($"""
            SELECT * FROM (
              SELECT *,
                     ROW_NUMBER() OVER (ORDER BY "RequestLogID" DESC) AS RowNumber
              FROM "RequestLog"
            ) AS NumberedLogs
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var result = paginatedResult.ToList();
        int? nextPage = null;

        if (result.Count > pageSize)
        {
          result.RemoveAt(result.Count - 1);
          nextPage = page + 1;
        }

        return (result, nextPage);
      }

      public async Task<RequestLog> GetByIdAsync(int requestLogId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@RequestLogId", requestLogId);
        
        IEnumerable<RequestLog> result;
        
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<RequestLog>("""
            SELECT * 
            FROM "RequestLog" 
            WHERE "RequestLogID" = @RequestLogId;
            """, p);
        }

        return result.SingleOrDefault()!;
      }

      public int Update(RequestLog entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateResponseAsync(int requestLogID, string statusCode, long responseLength)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@RequestLogID", requestLogID);
        p.Add("@StatusCode", statusCode);
        p.Add("@ResponseLength", responseLength);
        
        int rowsAffected;
        
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "RequestLog" 
            SET "StatusCode" = @StatusCode, "ResponseLengthBytes" = @ResponseLength
            WHERE "RequestLogID" = @RequestLogID;
            """, p);
        }

        return rowsAffected;
      }
    }

    public IInventoryAdjustmentManager GetInventoryAdjustmentManager()
    {
      return new InventoryAdjustmentManager(_connectionString);
    }

    public class InventoryAdjustmentManager : IInventoryAdjustmentManager
    {
      private readonly string _connectionString;

      public InventoryAdjustmentManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new ZIPCodeManager(_connectionString);
    }

    public class ZIPCodeManager : IZIPCodeManager
    {
      private readonly string _connectionString;

      public ZIPCodeManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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
      return new TenantManager(_connectionString);
    }

    public class TenantManager : ITenantManager
    {
      private readonly string _connectionString;

      public TenantManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Tenant Create(Tenant entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateUserAsync(string email, string firstName, string lastName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@FirstName", firstName);
        p.Add("@LastName", lastName);
        p.Add("@Email", email);

        int rowsAffected = 0;

        List<Tenant> tenants = await GetAllAsync();

        foreach (var tenant in tenants.Where(t => !t.DropletId.HasValue))
        {
          NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
          {
            Database = tenant.DatabaseName
          };
          using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
          {
            rowsAffected = await con.ExecuteAsync("""
              UPDATE "User"
              SET "FirstName" = @FirstName, "LastName" = @LastName
              WHERE "Email" = @Email
              """, p);
          }
        }

        return rowsAffected;
      }

      public async Task<Tenant> CreateAsync(Tenant entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@FullyQualifiedDomainName", entity.FullyQualifiedDomainName);
        p.Add("@Email", entity.Email);
        p.Add("@DatabasePassword", entity.DatabasePassword);
        p.Add("@DropletId", entity.DropletId);
        p.Add("@Ipv4", entity.Ipv4);
        p.Add("@SshPublic", entity.SshPublic);
        IEnumerable<Tenant> result;

        // change db name to default db
        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            INSERT INTO "Tenant" ("PublicId", "FullyQualifiedDomainName", "Email", "DatabasePassword", "DropletId", "Ipv4", "SshPublic")
            VALUES (
              substr(md5(random()::text), 1, 10), -- Random alphanumeric string
              @FullyQualifiedDomainName, 
              @Email, 
              @DatabasePassword,
              @DropletId, 
              @Ipv4, 
              @SshPublic
            )
            RETURNING *;
            """, p);
        }
        return result.Single();
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(
        int page,
        int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);

        IEnumerable<Tenant> paginatedResult;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          paginatedResult = await con.QueryAsync<Tenant>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "TenantID" DESC) AS RowNumber
                FROM "Tenant"
            ) AS NumberedTenants
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var result = paginatedResult.ToList();
        int? nextPage = null;

        if (result.Count > pageSize)
        {
          result.RemoveAt(result.Count - 1);
          nextPage = page + 1;
        }

        return (result, nextPage);
      }

      public int Update(Tenant entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateDatabaseName(int tenantID, string? databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantID);
        p.Add("@DatabaseName", databaseName);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "DatabaseName" = @DatabaseName
            WHERE "TenantID" = @TenantID
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateDropletIdAsync(int tenantId, long dropletId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantId);
        p.Add("@DropletId", dropletId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "DropletId" = @DropletId
            WHERE "TenantID" = @TenantID
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateSshPrivateAsync(int tenantId, string sshPrivate)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantId);
        p.Add("@SshPrivate", sshPrivate);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "SshPrivate" = @SshPrivate
            WHERE "TenantID" = @TenantID
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateSshPublicAsync(int tenantId, string sshPublic, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantId);
        p.Add("@SshPublic", sshPublic);
        p.Add("@OrganizationId", organizationId);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "SshPublic" = @SshPublic
            WHERE "TenantID" = @TenantID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<List<Tenant>> GetAllAsync()
      {
        IEnumerable<Tenant> result;

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant"
            """);
        }

        return result.ToList();
      }

      public async Task<int> UpdateSshPublicAsync(int tenantId, string sshPublic)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantId);
        p.Add("@SshPublic", sshPublic);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "SshPublic" = @SshPublic
            WHERE "TenantID" = @TenantID
            """, p);
        }

        return rowsAffected;
      }

      public async Task<Tenant> GetAsync(int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantId);

        IEnumerable<Tenant> result;

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "TenantID" = @TenantID
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<int> DeleteAsync(int tenantID)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantID", tenantID);

        int rowsModified;

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName,
          Password = ConfigurationSingleton.Instance.DatabasePassword
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          rowsModified = await con.ExecuteAsync("""
            DELETE FROM "Tenant" 
            WHERE "TenantID" = @TenantID
            """, p);
        }

        return rowsModified;
      }

      public async Task<Tenant> GetAsync(string tenantPublicId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantPublicId", tenantPublicId);

        IEnumerable<Tenant> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "PublicId" = @TenantPublicId
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<int> UpdateEmailAsync(int tenantId, string email)
      {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@TenantID", tenantId);
        parameters.Add("@Email", email);

        int rowsAffected;

        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await connection.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "Email" = @Email
            WHERE "TenantID" = @TenantID
            """, parameters);
        }

        return rowsAffected;
      }

      public async Task<int> UpdateIpv4Async(int tenantId, string ipAddress)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantId", tenantId);
        p.Add("@IpAddress", ipAddress);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "Ipv4" = @IpAddress
            WHERE "TenantID" = @TenantId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<bool> TenantExistsAsync(string? databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@DatabaseName", databaseName);

        IEnumerable<Tenant> result;

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "DatabaseName" = @DatabaseName
            """, p);
        }
        return result.Any();
      }

      public async Task<Tenant> GetByEmailAsync(string? email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<Tenant> result;

        // change db name to default db
        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "Email" = @Email
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<Tenant> GetByDatabaseNameAsync(string databaseName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@DatabaseName", databaseName);

        IEnumerable<Tenant> result;

        // change db name to default db

        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "DatabaseName" = @DatabaseName
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<int> UpdateHomepageMessageAsync(int tenantId, string? homepageMessage)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantId", tenantId);
        p.Add("@HomepageMessage", homepageMessage);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Tenant" 
            SET "HomepageMessage" = @HomepageMessage
            WHERE "TenantID" = @TenantId
            """, p);
        }

        return rowsAffected;
      }

      public async Task<int> GetCurrentDropletCountAsync()
      {
        int numberOfTenantsWithDropletId = 0;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          numberOfTenantsWithDropletId = await con.ExecuteScalarAsync<int>("""
            SELECT COUNT(*)
            FROM "Tenant"
            WHERE "DropletId" IS NOT NULL
            """);
        }

        return numberOfTenantsWithDropletId;
      }

      public async Task<int> UpdateUserEmailAsync(string oldEmail, string newEmail)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@OldEmail", oldEmail);
        p.Add("@NewEmail", newEmail);

        int rowsAffected = 0;

        List<Tenant> tenants = await GetAllAsync();
        foreach (var tenant in tenants.Where(t => !t.DropletId.HasValue))
        {
          NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
          {
            Database = tenant.DatabaseName
          };
          using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
          {
            rowsAffected += await con.ExecuteAsync("""
              UPDATE "User"
              SET "Email" = @NewEmail
              WHERE "Email" = @OldEmail
              """, p);
          }
        }

        return rowsAffected;
      }

      public async Task<Tenant?> GetByDomainAsync(string fullyQualifiedDomainName)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@FullyQualifiedDomainName", fullyQualifiedDomainName);
        
        IEnumerable<Tenant> result;
        
        // change db name to default db
        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
          Database = DatabaseThing.DatabaseConstants.DatabaseName
        };

        using (NpgsqlConnection con = new NpgsqlConnection(builder.ConnectionString))
        {
          result = await con.QueryAsync<Tenant>("""
            SELECT * 
            FROM "Tenant" 
            WHERE "FullyQualifiedDomainName" = @FullyQualifiedDomainName
            """, p);
        }

        return result.SingleOrDefault();
      }
    }

    public ISecretManager GetSecretManager()
    {
      return new SecretManager(_connectionString);
    }

    public class SecretManager : ISecretManager
    {
      private string _connectionString;

      public SecretManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Secret Create(Secret entity)
      {
        throw new NotImplementedException();
      }

      public Task<Secret> CreateAsync(Secret entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Secret> CreateAsync(bool master, string? value, string? type, string? purpose, int organizationId, int createdById, int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Master", master);
        p.Add("@Value", value);
        p.Add("@Type", type);
        p.Add("@Purpose", purpose);
        p.Add("@OrganizationId", organizationId);
        p.Add("@CreatedById", createdById);
        p.Add("@TenantId", tenantId);

        IEnumerable<Secret> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Secret>("""
            INSERT INTO "Secret" ("Master", "Value", "Type", "Purpose", "OrganizationId", "CreatedById", "TenantId")
            VALUES (@Master, @Value, @Type, @Purpose, @OrganizationId, @CreatedById, @TenantId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(int id)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ID", id);

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          return await con.ExecuteAsync("""
          DELETE FROM "Secret"
          WHERE "SecretID" = @ID
          """, p);
        }
      }

      public async Task<int> DeleteMasterAsync(int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantId", tenantId);

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          return await con.ExecuteAsync("""
          DELETE FROM "Secret"
          WHERE "TenantId" = @TenantId
          AND "Master" = TRUE
          """, p);
        }
      }

      public Secret Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Secret> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Secret>> GetAllAsync(int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantId", tenantId);

        IEnumerable<Secret> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Secret>("""
                SELECT * 
                FROM "Secret" 
                WHERE "TenantId" = @TenantId
                ORDER BY "SecretID" DESC
                """, p);
        }

        return result.ToList();
      }

      public async Task<Secret> GetAsync(int secretId, int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@ID", secretId);
        p.Add("@TenantId", tenantId);

        IEnumerable<Secret> result;

        using (NpgsqlConnection con
          = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Secret>("""
            SELECT * 
            FROM "Secret" 
            WHERE "SecretID" = @ID
            AND "TenantId" = @TenantId
            """, p);
        }

        return result.Single();
      }

      public async Task<Secret?> GetAsync(string type, int tenantId, int? organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Type", type);
        p.Add("@TenantId", tenantId);
        p.Add("@OrganizationId", organizationId, DbType.Int32); // Always add it, even if null

        IEnumerable<Secret> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          
          result = await con.QueryAsync<Secret>("""
            SELECT *
            FROM "Secret"
            WHERE "Type" = @Type
              AND "TenantId" = @TenantId
              AND (@OrganizationId IS NULL OR "OrganizationId" = @OrganizationId)
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<Secret?> GetAsync(string type)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Type", type);

        IEnumerable<Secret> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Secret>("""
            SELECT * 
            FROM "Secret" 
            WHERE "Type" = @Type
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<Secret?> GetMasterAsync(int tenantId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@TenantId", tenantId);

        IEnumerable<Secret> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Secret>("""
            SELECT * 
            FROM "Secret" 
            WHERE "TenantId" = @TenantId
            AND "Master" = TRUE
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(Secret entity)
      {
        throw new NotImplementedException();
      }
    }

    public ILoginWithoutPasswordManager GetLoginWithoutPasswordManager()
    {
      return new LoginWithoutPasswordManager(_connectionString);
    }

    public class LoginWithoutPasswordManager : ILoginWithoutPasswordManager
    {
      private readonly string _connectionString;

      public LoginWithoutPasswordManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public LoginWithoutPassword Create(LoginWithoutPassword entity)
      {
        throw new NotImplementedException();
      }

      public Task<LoginWithoutPassword> CreateAsync(LoginWithoutPassword entity)
      {
        throw new NotImplementedException();
      }

      public async Task<LoginWithoutPassword> CreateAsync(string email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<LoginWithoutPassword> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<LoginWithoutPassword>("""
            INSERT INTO "LoginWithoutPassword" ("Code", "Email", "Expires") 
            VALUES (substr(md5(random()::text), 1, 10), @Email, CURRENT_TIMESTAMP AT TIME ZONE 'UTC' + INTERVAL '5 minute')
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(LoginWithoutPassword loginWithoutPassword)
      {
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          return await con.ExecuteAsync("""
                DELETE FROM "LoginWithoutPassword"
                WHERE "LoginWithoutPasswordID" = @LoginWithoutPasswordID;
                """, loginWithoutPassword);
        }
      }

      public async Task<int> DeleteAsync(string? email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          return await con.ExecuteAsync("""
                DELETE FROM "LoginWithoutPassword"
                WHERE "Email" = @Email;
                """, p);
        }
      }

      public LoginWithoutPassword Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<LoginWithoutPassword> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<LoginWithoutPassword> GetAsync(string email)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Email", email);

        IEnumerable<LoginWithoutPassword> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<LoginWithoutPassword>("""
            SELECT * 
            FROM "LoginWithoutPassword" 
            WHERE "Email" = @Email
            AND "Expires" > CURRENT_TIMESTAMP AT TIME ZONE 'UTC'
            """, p);
        }

        return result.SingleOrDefault();
      }

      public int Update(LoginWithoutPassword entity)
      {
        throw new NotImplementedException();
      }
    }

    public ILocationManager GetLocationManager()
    {
      return new LocationManager(_connectionString);
    }

    public class LocationManager : ILocationManager
    {
      private readonly string _connectionString;

      public LocationManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<int> DeleteAsync(int locationID, bool deleteChildren)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@LocationID", locationID);

        try
        {
          using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
          {
            if (deleteChildren)
            {
              // Use a recursive CTE to find all descendants
              await con.ExecuteAsync("""
                  WITH RECURSIVE Descendants AS (
                    SELECT "LocationID" FROM "Location" WHERE "ParentLocationId" = @LocationID
                    UNION
                    SELECT l."LocationID" FROM "Location" l
                    INNER JOIN Descendants d ON l."ParentLocationId" = d."LocationID"
                  )
                  DELETE FROM "Location"
                  WHERE "LocationID" IN (SELECT "LocationID" FROM Descendants)
                  """, p);
            }
            // Delete the item itself
            int rowsAffected = await con.ExecuteAsync("""
                DELETE FROM "Location" 
                WHERE "LocationID" = @LocationID
                """, p);

            return rowsAffected;
          }
        }
        catch (PostgresException ex) when (ex.SqlState == "23503")
        {
          throw new InvalidOperationException("The location cannot be deleted because it is being used elsewhere. 23503.", ex);
        }
      }

      public Location Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Location> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<List<Location>> GetAllAsync(int organizationId)
      {
        IEnumerable<Location> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Location>("""
            SELECT * 
            FROM "Location" 
            WHERE "OrganizationId" = @OrganizationId
            """, new { OrganizationId = organizationId });
        }

        return result.ToList();
      }

      public async Task<(List<Location> locations, int? nextPage)> GetAllAsync(
        int page,
        int pageSize,
        int organizationId,
        bool includeDescendants,
        bool includeInventories)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize + 1);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Location> locations;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          locations = await con.QueryAsync<Location>($"""
            SELECT *, ROW_NUMBER() OVER (ORDER BY "LocationID") AS "RowNumber"
            FROM "Location"
            WHERE "OrganizationId" = @OrganizationId AND "ParentLocationId" IS NULL
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset
            """, new { PageSize = pageSize + 1, Offset = pageSize * (page - 1), OrganizationId = organizationId });
        }

        var hasMoreRecords = locations.Count() > pageSize;

        if (hasMoreRecords)
        {
          locations = locations.Take(pageSize);
        }

        int? nextPage = hasMoreRecords ? page + 1 : null;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          if (includeDescendants)
          {
            // Query to get all locations for the organization
            var allLocations = await con.QueryAsync<Location>($"""
              SELECT *
              FROM "Location"
              WHERE "OrganizationId" = @OrganizationId
              """, p);

            void PopulateChildrenRecursively(List<Location> children)
            {
              foreach (var child in children)
              {
                child.Children = allLocations.Where(x => x.ParentLocationId == child.LocationID).OrderBy(x => x.Name).ToList();
                if (child.Children.Any())
                {
                  PopulateChildrenRecursively(child.Children);
                }
              }
            }

            // Populate children for each root location
            foreach (var location in locations)
            {
              location.Children = allLocations.Where(x => x.ParentLocationId == location.LocationID).OrderBy(x => x.Name).ToList();
              if (location.Children.Any())
              {
                PopulateChildrenRecursively(location.Children);
              }
            }
          }

          if (includeInventories)
          {
            foreach (var location in locations)
            {
              var inventories = await con.QueryAsync<Inventory, Item, Location, Inventory>($"""
                SELECT i.*, it.*, l.*
                FROM "Inventory" i
                INNER JOIN "Item" it ON i."ItemId" = it."ItemID"
                INNER JOIN "Location" l ON i."LocationId" = l."LocationID"
                WHERE i."LocationId" = @LocationId
                AND i."OrganizationId" = @OrganizationId
                """,
                (inventory, inventoryItem, loc) =>
                {
                  inventory.Item = inventoryItem;
                  return inventory;
                },
                new { LocationId = location.LocationID, OrganizationId = organizationId },
                splitOn: "ItemID,LocationID");

              location.Inventories = inventories.ToList();
            }
          }
        }

        return (locations.ToList(), nextPage);
      }

      public async Task<Location?> GetAsync(int locationId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@LocationID", locationId);
        p.Add("@OrganizationId", organizationId);

        IEnumerable<Location> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

      public async Task<List<Location>?> GetChildrenAsync(int locationId, int organizationId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@LocationID", locationId);
        p.Add("@OrganizationId", organizationId);
        IEnumerable<Location> result;
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Location>("""
            SELECT * 
            FROM "Location" 
            WHERE "ParentLocationId" = @LocationID
            AND "OrganizationId" = @OrganizationId
            """, p);
        }
        return result.ToList();
      }

      public async Task<bool> IsLocationInUseAsync(int locationId, int organizationId)
      {
        using var con = new NpgsqlConnection(_connectionString);
        DynamicParameters p = new();
        p.Add("@LocationID", locationId);
        p.Add("@OrganizationId", organizationId);

        // Query to find foreign key references to the Location table.
        var foreignKeyQuery = """
          SELECT n.nspname AS schema_name,
                 c.relname AS table_name,
                 a.attname AS column_name
          FROM pg_constraint AS con
          JOIN pg_class AS c ON con.conrelid = c.oid
          JOIN pg_namespace AS n ON c.relnamespace = n.oid
          JOIN pg_attribute AS a ON a.attrelid = c.oid AND a.attnum = ANY(con.conkey)
          WHERE con.confrelid = '"Location"'::regclass;
          """;

        var references = await con.QueryAsync<(string Schema, string Table, string Column)>(foreignKeyQuery);

        foreach (var (schema, table, column) in references)
        {
          var existsQuery = $"""
          SELECT EXISTS (
            SELECT 1
            FROM "{schema}"."{table}"
            WHERE "{column}" = @LocationID
            AND "OrganizationId" = @OrganizationId
          )
          """;

          bool isInUse = await con.ExecuteScalarAsync<bool>(existsQuery, p);
          if (isInUse)
            return true;
        }

        return false;
      }

      public int Update(Location entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAsync(int locationId, string? name)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@LocationID", locationId);
        p.Add("@Name", name);

        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            UPDATE "Location" 
            SET "Name" = @Name
            WHERE "LocationID" = @LocationID
            """, p);
        }

        return rowsAffected;
      }
    }

    public IBlogManager GetBlogManager()
    {
      return new BlogManager(_connectionString);
    }

    public class BlogManager : IBlogManager
    {
      private readonly string _connectionString;

      public BlogManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Blog Create(Blog entity)
      {
        throw new NotImplementedException();
      }

      public async Task<Blog> CreateAsync(Blog entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Title", entity.Title);
        p.Add("@Content", entity.Content);
        p.Add("@CreatedById", entity.CreatedById);
        if (!string.IsNullOrEmpty(entity.PublicId)) p.Add("@PublicId", entity.PublicId);

        string columns = "\"Title\", \"Content\", \"CreatedById\"";
        string values = "@Title, @Content, @CreatedById";

        if (!string.IsNullOrEmpty(entity.PublicId))
        {
          columns += ", \"PublicId\"";
          values += ", @PublicId";
        }

        string query = $"INSERT INTO \"Blog\" ({columns}) VALUES ({values}) RETURNING *;";

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          var result = await con.QueryAsync<Blog>(query, p);
          return result.Single();
        }
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public async Task<int> DeleteAsync(int blogId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@BlogID", blogId);
        
        int rowsAffected;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          rowsAffected = await con.ExecuteAsync("""
            DELETE FROM "Blog"
            WHERE "BlogID" = @BlogID
            """, p);
        }

        return rowsAffected;
      }

      public Blog Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Blog> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<(List<Blog> blogs, int? nextPage)> GetAllAsync(int page, int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);

        IEnumerable<Blog> paginatedResult;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          paginatedResult = await con.QueryAsync<Blog>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "BlogID" DESC) AS RowNumber
                FROM "Blog"
            ) AS NumberedBlogs
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }

        var result = paginatedResult.ToList();
        int? nextPage = null;

        if (result.Count > pageSize)
        {
          result.RemoveAt(result.Count - 1);
          nextPage = page + 1;
        }

        return (result, nextPage);
      }

      public async Task<Blog> GetAsync(int blogId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@BlogID", blogId);

        IEnumerable<Blog> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Blog>("""
            SELECT * 
            FROM "Blog" 
            WHERE "BlogID" = @BlogID
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<Blog> GetByPublicIdAsync(string publicId)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@PublicId", publicId);

        IEnumerable<Blog> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Blog>("""
            SELECT * 
            FROM "Blog" 
            WHERE "PublicId" = @PublicId
            """, p);
        }

        return result.SingleOrDefault();
      }

      public async Task<Blog> GetFirstPublicAsync()
      {
        IEnumerable<Blog> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Blog>("""
            SELECT * 
            FROM "Blog" 
            WHERE "PublicId" IS NOT NULL
            ORDER BY "BlogID" DESC
            LIMIT 1
            """);
        }

        return result.SingleOrDefault();
      }

      public int Update(Blog entity)
      {
        throw new NotImplementedException();
      }

      public async Task<int> UpdateAsync(Blog blog)
      {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@BlogID", blog.BlogID);
        parameters.Add("@Title", blog.Title);
        parameters.Add("@Content", blog.Content);
        parameters.Add("@PublicId", blog.PublicId);

        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
          int rowsAffected = await connection.ExecuteAsync("""
            UPDATE "Blog" 
            SET "Title" = @Title,
            "Content" = @Content, 
            "PublicId" = @PublicId
            WHERE "BlogID" = @BlogID
            """, parameters);
          return rowsAffected;
        }
      }
    }

    public IExceptionManager GetExceptionManager()
    {
      return new ExceptionManager(_connectionString);
    }

    public class ExceptionManager : IExceptionManager
    {
      private readonly string _connectionString;

      public ExceptionManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Business.Exception Create(Business.Exception entity)
      {
        throw new NotImplementedException();
      }
      
      public async Task<Business.Exception> CreateAsync(Business.Exception entity)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Message", entity.Message);
        p.Add("@StackTrace", entity.StackTrace);
        p.Add("@Source", entity.Source);
        p.Add("@HResult", entity.HResult);
        p.Add("@TargetSite", entity.TargetSite);
        p.Add("@InnerException", entity.InnerException);
        p.Add("@RequestLogId", entity.RequestLogId);

        IEnumerable<Business.Exception> result;

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          result = await con.QueryAsync<Business.Exception>(""" 
            INSERT INTO "Exception" ("Message", "StackTrace", "Source", "HResult", "TargetSite", "InnerException", "RequestLogId") 
            VALUES (@Message, @StackTrace, @Source, @HResult, @TargetSite, @InnerException, @RequestLogId)
            RETURNING *;
            """, p);
        }

        return result.Single();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Business.Exception Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Business.Exception> GetAll()
      {
        throw new NotImplementedException();
      }

      public async Task<(IEnumerable<Business.Exception> exceptions, int? nextPage)> GetAllAsync(
        int page, 
        int pageSize)
      {
        DynamicParameters p = new DynamicParameters();
        p.Add("@Page", page);
        p.Add("@PageSize", pageSize);
        IEnumerable<Business.Exception> paginatedResult;
        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        {
          paginatedResult = await con.QueryAsync<Business.Exception>($"""
            SELECT * FROM (
                SELECT *,
                       ROW_NUMBER() OVER (ORDER BY "ExceptionID" DESC) AS RowNumber
                FROM "Exception"
            ) AS NumberedExceptions
            WHERE RowNumber BETWEEN @PageSize * (@Page - 1) + 1 AND @PageSize * @Page + 1
            """, p);
        }
        var result = paginatedResult.ToList();
        int? nextPage = null;
        if (result.Count > pageSize)
        {
          result.RemoveAt(result.Count - 1);
          nextPage = page + 1;
        }
        return (result, nextPage);
      }

      public int Update(Business.Exception entity)
      {
        throw new NotImplementedException();
      }
    }

    public IClaimManager GetClaimManager()
    {
      return new ClaimManager(_connectionString);
    }

    public class ClaimManager : IClaimManager
    {
      private readonly string _connectionString;

      public ClaimManager(string connectionString)
      {
        _connectionString = connectionString;
      }

      public Claim Create(Claim entity)
      {
        throw new NotImplementedException();
      }

      public Task<Claim> CreateAsync(Claim entity)
      {
        throw new NotImplementedException();
      }

      public int Delete(int id)
      {
        throw new NotImplementedException();
      }

      public Claim Get(int id)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Claim> GetAll()
      {
        throw new NotImplementedException();
      }

      public Task<Claim> GetAsync(int userId, string databaseName, string inRole)
      {
        throw new NotImplementedException();
      }

      public int Update(Claim entity)
      {
        throw new NotImplementedException();
      }
    }

    public IUserToDoManager GetUserToDoManager()
    {
      return new UserTaskManager(_connectionString);
    }

    public class UserTaskManager : IUserToDoManager
    {
      private readonly string _connectionString;

      public UserTaskManager(string connectionString)
      {
        _connectionString = connectionString;
      }

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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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