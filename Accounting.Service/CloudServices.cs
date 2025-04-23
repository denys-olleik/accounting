using Accounting.Business;
using Accounting.Common;
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
          client.Disconnect();
        }
      }
      catch (System.Exception ex)
      {
        //Console.WriteLine($"An error occurred while executing command: {ex.Message}");
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

      private string GetCertbotConfig(string fullyQualifiedDomainName, string ownerEmail)
      {
        bool isSubdomain = fullyQualifiedDomainName.Count(c => c == '.') > 1;

        string certbotCommand = isSubdomain
            ? $"sudo certbot --nginx -d {fullyQualifiedDomainName} -n --agree-tos --email {ownerEmail}"
            : $"sudo certbot --nginx -d {fullyQualifiedDomainName} -d www.{fullyQualifiedDomainName} -n --agree-tos --email {ownerEmail}";

        return certbotCommand + " > /var/log/accounting/certbot.log 2>&1";
      }

      private string GetNginxConfig(string fullyQualifiedDomainName)
      {
        if (Uri.CheckHostName(fullyQualifiedDomainName) != UriHostNameType.Dns)
        {
          throw new ArgumentException("Invalid domain name format", nameof(fullyQualifiedDomainName));
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(fullyQualifiedDomainName, @"^[a-zA-Z0-9.-]+$"))
        {
          throw new ArgumentException("Domain contains invalid characters", nameof(fullyQualifiedDomainName));
        }

        string nginxTemplate = System.IO.File.ReadAllText("nginx.txt");
        bool isSubdomain = fullyQualifiedDomainName.Count(c => c == '.') > 1;

        string serverNameLine;
        if (isSubdomain)
        {
          serverNameLine = $"server_name {fullyQualifiedDomainName};";
        }
        else
        {
          serverNameLine = $"server_name {fullyQualifiedDomainName} www.{fullyQualifiedDomainName};";
        }

        return nginxTemplate.Replace("server_name example.com www.example.com;", serverNameLine);
      }

      public async Task CreateDropletAsync(
        Tenant tenant,
        string databasePassword,
        string ownerEmail,
        string? ownerPassword,
        string ownerFirst,
        string ownerLast,
        bool tenantManagement,
        string emailApiKey,
        string fullyQualifiedDomainName,
        string cloudApiKey = null,
        string noReplyEmailAddress = null)
      {
        Secret? cloudSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Cloud, 1);
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

        string emailApiKeyScript =
            @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Secret\"" (\""Master\"", \""Value\"", \""Type\"", \""CreatedById\"", \""OrganizationId\"", \""TenantId\"") VALUES (false, '${EmailApiKey}', 'email', 1, 1, 1);"" > /var/log/accounting/email-api-key-insert.log 2>&1
";

        string cloudApiKeyScript =
            @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Secret\"" (\""Master\"", \""Value\"", \""Type\"", \""CreatedById\"", \""OrganizationId\"", \""TenantId\"") VALUES (false, '${CloudApiKey}', 'cloud', 1, 1, 1);"" > /var/log/accounting/cloud-api-key-insert.log 2>&1
";

        string noReplyScript =
            @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Secret\"" (\""Master\"", \""Value\"", \""Type\"", \""CreatedById\"", \""OrganizationId\"", \""TenantId\"") VALUES (false, '${NoReplyEmailAddress}', 'no-reply', 1, 1, 1);"" > /var/log/accounting/no-reply-insert.log 2>&1
";

        string timeCalculationScript =
        @"
# Calculate time taken and store as environment variable
end_time=$(date +%s)
seconds_to_run_script=$((end_time - start_time))
echo ""SetupTimeInSeconds=${seconds_to_run_script}"" | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
";

        string createTenantRecordScript =
            @"
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""Tenant\"" (\""PublicId\"", \""Email\"", \""DatabaseName\"", \""DatabasePassword\"") VALUES ('1', '${OwnerEmail}', 'Accounting', '${DatabasePassword}');"" > /var/log/accounting/tenant-insert.log 2>&1
";

        ownerPassword = string.IsNullOrEmpty(ownerPassword) ? null : PasswordStorage.CreateHash(ownerPassword);

        string createUserRecordScript =
        @"
# Create user record
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""User\"" (\""Email\"", \""FirstName\"", \""LastName\"", \""Password\"") VALUES ('${OwnerEmail}', '${OwnerFirst}', '${OwnerLast}', '${OwnerPassword}');"" > /var/log/accounting/user-insert.log 2>&1
";

        string createUserOrganizationRecordScript =
            @"
# Create user organization record
sudo -i -u postgres psql -d ""Accounting"" -c ""INSERT INTO \""UserOrganization\"" (\""UserId\"", \""OrganizationId\"") VALUES (1, 1);"" > /var/log/accounting/user-organization-insert.log 2>&1
";

        string nginxConfig = GetNginxConfig(fullyQualifiedDomainName);

        string certbotConfig = GetCertbotConfig(fullyQualifiedDomainName, ownerEmail);

        string systemdConfiguration = System.IO.File.ReadAllText("systemd.txt");

        string setupScript = $"""
#!/bin/bash
start_time=$(date +%s)

# Create log directory
sudo mkdir -p /var/log/accounting > /dev/null 2>&1
sudo mkdir -p /var/accounting > /dev/null 2>&1

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
echo 'NoReplyEmailAddress={noReplyEmailAddress}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
[ -n '{cloudApiKey}' ] && echo 'CloudApiKey={cloudApiKey}' | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1
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

# Configure Nginx
echo '{nginxConfig}' | sudo tee /etc/nginx/sites-available/default > /var/log/accounting/nginx-config.log 2>&1
sudo systemctl restart nginx >> /var/log/accounting/nginx-config.log 2>&1

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
git -C /opt/accounting config core.fileMode false

# Adjust directory permissions and group access
# Create a new group for accounting
sudo groupadd accounting

# Add both root and postgres users to the group
sudo usermod -aG accounting root
sudo usermod -aG accounting postgres

# Change the group ownership of the directory
sudo chown -R :accounting /opt/accounting

# Set permissions to allow group access
sudo chmod -R 775 /opt/accounting

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

# Create user organization record
""" + createUserOrganizationRecordScript + """

# Create email API key
""" + emailApiKeyScript + """

# Create cloud API key

# Create cloud API key
# [ -n "$CloudApiKey" ] && echo "CloudApiKey=$CloudApiKey" | sudo tee -a /etc/environment >> /var/log/accounting/env-setup.log 2>&1

# Create no-reply secret
""" + noReplyScript + $"""

# Create cloud API key
""" + cloudApiKeyScript + $"""

# Build the .NET project
export DOTNET_CLI_HOME=/root
dotnet build /opt/accounting/Accounting/Accounting.csproj -c Release > /var/log/accounting/dotnet-build.log 2>&1

# Configure systemd
echo '{systemdConfiguration}' | sudo tee /etc/systemd/system/accounting.service > /var/log/accounting/systemd-config.log 2>&1

# Accounting.TODO: Consider creating a dedicated 'accounting' user for improved security.
# Temporary solution: Setting ownership to postgres:root
# Ensure correct ownership and permissions
# sudo chown -R postgres:root /opt/accounting
# sudo chmod -R 770 /opt/accounting

sudo chown -R postgres:root /var/accounting
sudo chmod -R 770 /var/accounting

# Reload systemd to apply changes
sudo systemctl daemon-reload

# Enable the service to start on boot
sudo systemctl enable accounting.service

# Restart the service
sudo systemctl restart accounting.service

# Install Certbot and Nginx plugin
sudo apt-get install -y certbot python3-certbot-nginx > /var/log/accounting/certbot-install.log 2>&1

""" + certbotConfig + $"""

# Indicate successful setup
echo "Setup completed successfully" > /var/log/custom-setup.log

# sudo -i -u postgres psql -d Accounting -c 'INSERT INTO "Secret" ("Value", "Type", "TenantId") VALUES (\'true\', \'tenant-management\', 1);' <<< might have to delete this line.
# sudo -i -u postgres psql -d Accounting -c 'SELECT * FROM "Secret";' 

# sudo sed -i 's/TenantManagement=True/TenantManagement=false/' /etc/environment
# sudo sed -i 's/TenantManagement=false/TenantManagement=True/' /etc/environment
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

      public async Task<string?> DiscoverIpAsync(long? dropletId, Tenant tenant, string privateKey)
      {
        if (dropletId == null)
        {
          throw new ArgumentNullException(nameof(dropletId));
        }

        Secret cloudSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Cloud, 1);
        var client = new DigitalOceanClient(cloudSecret!.Value);

        Droplet? droplet;

        droplet = await client.Droplets.Get(dropletId.Value);

        var ipAddress = droplet.Networks.V4.FirstOrDefault(n => n.Type == "public")?.IpAddress;

        return ipAddress;
      }
    }
  }
}