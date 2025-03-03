using Accounting.Business;
using DigitalOcean.API;
using DigitalOcean.API.Models.Responses;
using Google.Protobuf.WellKnownTypes;
using Renci.SshNet;
using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accounting.Service
{
  public class CloudServices : BaseService
  {
    private readonly DigitalOceanService _digitalOceanService;

    public CloudServices(
      SecretService secretService,
      TenantService tenantService) : base()
    {
      _digitalOceanService = new DigitalOceanService(secretService, tenantService);
    }

    public CloudServices(
      SecretService secretService,
      TenantService tenantService,
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {
      _digitalOceanService = new DigitalOceanService(secretService, tenantService);
    }

    public bool TestSshConnectionAsync(string ipAddress, string privateKey, string username = "root")
    {
      bool success = false;

      try
      {
        using (var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKey)))
        using (var client = new SshClient(ipAddress, username, new PrivateKeyFile(privateKeyStream)))
        {
          client.Connect();
          if (client.IsConnected)
          {
            var result = client.RunCommand("echo 'The quick brown fox jumped over the lazy dog!'");
            if (result.Result.Contains("The quick brown fox jumped over the lazy dog!"))
            {
              success = true;
            }
            else
            {
              Console.WriteLine("Failed to establish SSH connection.");
            }
          }
          else
          {
            Console.WriteLine("Failed to establish SSH connection.");
          }
          client.Disconnect();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred while testing SSH connection: {ex.Message}");
      }

      return success;
    }

    public DigitalOceanService GetDigitalOceanService()
    {
      return _digitalOceanService;
    }

    public async Task<string> ExecuteCommandAsync(string ipAddress, string privateKey, string command)
    {
      string result = string.Empty;
      try
      {
        using (var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKey)))
        using (var client = new SshClient(ipAddress, "root", new PrivateKeyFile(privateKeyStream)))
        {
          client.Connect();
          if (client.IsConnected)
          {
            var sshCommand = client.RunCommand(command);
            result = sshCommand.Result;
          }
          else
          {
            Console.WriteLine("Failed to establish SSH connection.");
          }
          client.Disconnect();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred while executing command: {ex.Message}");
      }
      return result;
    }

    public class DigitalOceanService
    {
      private readonly SecretService _secretService;
      private readonly TenantService _tenantService;

      public DigitalOceanService(SecretService secretService, TenantService tenantService)
      {
        _secretService = secretService;
        _tenantService = tenantService;
      }

      public async Task CreateDropletAsync(
        Tenant tenant,
        int organizationId,
        string databasePassword,
        string ownerEmail,
        string ownerPassword,
        string ownerFirst,
        string ownerLast,
        bool tenantManagement,
        string emailApiKey,
        string fullyQualifiedDomainName)
      {
        Secret? cloudSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Cloud, organizationId);
        if (cloudSecret == null)
        {
          throw new InvalidOperationException("Cloud secret not found");
        }
        var client = new DigitalOceanClient(cloudSecret.Value);

        var keygen = new SshKeyGenerator.SshKeyGenerator(2048);

        var sshKeyRequest = new DigitalOcean.API.Models.Requests.Key
        {
          Name = tenant.FullyQualifiedDomainName,
          PublicKey = keygen.ToRfcPublicKey(tenant.FullyQualifiedDomainName)
        };

        var sshKeyResponse = await client.Keys.Create(sshKeyRequest);

        //CREATE TABLE "Secret"
        //(
        //  "SecretID" SERIAL PRIMARY KEY NOT NULL,
        //  "Master" BOOLEAN DEFAULT FALSE,
        //  "Value" TEXT NOT NULL,
        //  "ValueEncrypted" BOOLEAN NOT NULL DEFAULT FALSE,
        //  "Type" VARCHAR(20) CHECK("Type" IN('email', 'sms', 'cloud', 'no-reply', 'tenant-management')) NULL UNIQUE,
        //  "Purpose" VARCHAR(100) NULL,
        //  "Created" TIMESTAMPTZ NOT NULL DEFAULT(CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
        //  "CreatedById" INT NULL,
        //  "OrganizationId" INT NULL,
        //  FOREIGN KEY("CreatedById") REFERENCES "User"("UserID"),
        //  FOREIGN KEY("OrganizationId") REFERENCES "Organization"("OrganizationID")
        //);

        string emailApiKeyScript =
          @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Secret\"" (\""Master\"", \""Value\"", \""Type\"", \""CreatedById\"", \""OrganizationId\"") VALUES (false, '${EmailApiKey}', 'email', 1, 1);"" > /var/log/accounting/email-api-key-insert.log 2>&1
";

        string noReplyScript =
          @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Secret\"" (\""Master\"", \""Value\"", \""Type\"", \""CreatedById\"", \""OrganizationId\"") VALUES (false, 'no-reply@${FullyQualifiedDomainName}', 'no-reply', 1, 1);"" > /var/log/accounting/no-reply-insert.log 2>&1
";

        string timeCalculationScript =
        @"
# Calculate time taken and store as environment variable
end_time=$(date +%s)
seconds_to_run_script=$((end_time - start_time))
echo ""SetupTimeInSeconds=${seconds_to_run_script}"" | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
";

        // INSERT INTO "Tenant" ("PublicId", "Email", "DatabaseName", "DatabasePassword") VALUES ('1', '[ownerEmail]', 'Accounting', '[databasePassword]');
        string createTenantRecordScript =
          @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Tenant\"" (\""PublicId\"", \""Email\"", \""DatabaseName\"", \""DatabasePassword\"") VALUES ('1', '${OwnerEmail}', 'Accounting', '${DatabasePassword}');"" > /var/log/accounting/tenant-insert.log 2>&1
";

        string createUserRecordScript =
        @"
# Create user record
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""User\"" (\""Email\"", \""FirstName\"", \""LastName\"", \""Password\"") VALUES ('${OwnerEmail}', '${OwnerFirst}', '${OwnerLast}', '${OwnerPassword}');"" > /var/log/accounting/user-insert.log 2>&1
";

        string setupScript = $"""
#!/bin/bash
start_time=$(date +%s)

# Create log directory
sudo mkdir -p /var/log/accounting > /dev/null 2>&1

# Set environment variables
echo 'ConnectionStrings__Psql="Host=localhost;Database=Accounting;Username=postgres;Password={databasePassword};"' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'ConnectionStrings__AdminPsql="Host=localhost;Database=postgres;Username=postgres;Password={databasePassword};"' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'DatabaseName={DatabaseThing.DatabaseConstants.DatabaseName}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'DatabasePassword={databasePassword}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'OwnerEmail={ownerEmail}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'OwnerPassword={ownerPassword}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'OwnerFirst={ownerFirst}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'OwnerLast={ownerLast}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'EmailApiKey={emailApiKey}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'FullyQualifiedDomainName={fullyQualifiedDomainName}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'TenantCreated=false' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'UserCreated=false' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'OrganizationCreated=false' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'UserOrganizationCreated=false' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
echo 'TenantManagement={tenantManagement}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1

# Update package lists
sudo apt-get update > /var/log/accounting/apt-update.log 2>&1

# Install net-tools for network utilities like netstat
sudo apt-get install -y net-tools > /var/log/accounting/net-tools-install.log 2>&1

# Install Nginx
sudo apt-get install -y nginx > /var/log/accounting/nginx-install.log 2>&1

# Install .NET SDK
# ----------------
# Install required packages
sudo apt-get install -y wget gpg > /var/log/accounting/dotnet-install.log 2>&1

# Add Microsoft GPG key and repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb >> /var/log/accounting/dotnet-install.log 2>&1
sudo dpkg -i packages-microsoft-prod.deb >> /var/log/accounting/dotnet-install.log 2>&1

# Update package index
sudo apt-get update >> /var/log/accounting/dotnet-install.log 2>&1

# Install the .NET SDK
sudo apt-get install -y dotnet-sdk-8.0 >> /var/log/accounting/dotnet-install.log 2>&1

# Update .NET workloads
dotnet workload update >> /var/log/accounting/dotnet-install.log 2>&1
# ----------------

# Install PostgreSQL
sudo apt-get install -y postgresql > /var/log/accounting/postgresql-install.log 2>&1

# Set PostgreSQL password
sudo -i -u postgres psql -c "ALTER USER postgres WITH PASSWORD '{databasePassword}';" > /var/log/accounting/postgres-password.log 2>&1

# Update pg_hba.conf to use scram-sha-256 /etc/postgresql/16/main/pg_hba.conf
# sudo sed -i '/^local\s\+all\s\+postgres\s\+peer/c\local   all             postgres                                scram-sha-256' /etc/postgresql/16/main/pg_hba.conf > /var/log/accounting/postgres-hba.log 2>&1

# Restart PostgreSQL
sudo systemctl restart postgresql

# Install PostGIS
sudo apt-get install -y postgis > /var/log/accounting/postgis-install.log 2>&1

# Clone repository
git clone https://github.com/denys-olleik/accounting /opt/accounting > /var/log/accounting/git-clone.log 2>&1

# Create database
sudo -i -u postgres psql -c "CREATE DATABASE \"Accounting\";"
sudo -i -u postgres psql -d "Accounting" -f /opt/accounting/Accounting.Database/create-db-script-psql.sql > /var/log/accounting/create-db.log 2>&1

# Source environment variables from /etc/environment
set -o allexport
source /etc/environment
set +o allexport

# Create tenant record
""" + createTenantRecordScript + """
# sudo -i -u postgres psql -d Accounting -c 'SELECT * FROM "Tenant";'

# Create user record
""" + createUserRecordScript + """
# sudo -i -u postgres psql -d Accounting -c 'SELECT * FROM "User";'

# Load sample data except for user data - /opt/accounting/Accounting.Database/sample-data-production.sql
sudo -i -u postgres psql -d "Accounting" -f /opt/accounting/Accounting.Database/sample-data-production.sql > /var/log/accounting/sample-data.log 2>&1

# Create email API key
""" + emailApiKeyScript + """

# Create no-reply secret
""" + noReplyScript + """

# Build the .NET project
export DOTNET_CLI_HOME=/root
dotnet build /opt/accounting/Accounting/Accounting.csproj > /var/log/accounting/dotnet-build.log 2>&1

# Indicate successful setup
echo "Setup completed successfully" > /var/log/custom-setup.log
""" + timeCalculationScript;

        var dropletRequest = new DigitalOcean.API.Models.Requests.Droplet()
        {
          Name = tenant.FullyQualifiedDomainName,
          Region = "nyc",
          Size = "s-1vcpu-1gb",
          Image = "ubuntu-24-04-x64",
          SshKeys = new List<object> { sshKeyResponse.Fingerprint },
          UserData = setupScript
        };

        var dropletResponse = await client.Droplets.Create(dropletRequest);

        await _tenantService.UpdateDropletIdAsync(tenant.Identifiable, dropletResponse.Id);
        await _tenantService.UpdateSshPublicAsync(tenant.Identifiable, keygen.ToRfcPublicKey(tenant.FullyQualifiedDomainName));
        await _tenantService.UpdateSshPrivateAsync(tenant.Identifiable, keygen.ToPrivateKey());
      }

      public async Task<string?> DiscoverIpAsync(long? dropletId, Tenant tenant, string privateKey, int organizationId)
      {
        if (dropletId == null)
        {
          throw new ArgumentNullException(nameof(dropletId));
        }

        Secret cloudSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Cloud, organizationId);
        var client = new DigitalOceanClient(cloudSecret!.Value);

        Droplet? droplet;

        droplet = await client.Droplets.Get(dropletId.Value);

        var ipAddress = droplet.Networks.V4.FirstOrDefault(n => n.Type == "public")?.IpAddress;

        return ipAddress;
      }
    }
  }
}