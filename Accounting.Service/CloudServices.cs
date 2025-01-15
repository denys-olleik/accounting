using Accounting.Business;
using DigitalOcean.API;
using DigitalOcean.API.Exceptions;
using DigitalOcean.API.Models.Responses;
using Renci.SshNet;
using System.Text;

namespace Accounting.Service
{
  public class CloudServices
  {
    private readonly DigitalOceanService _digitalOceanService;
    private readonly SecretService _secretService;
    private readonly string _databaseName;

    public CloudServices(
      SecretService secretService,
      TenantService tenantService,
      string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _digitalOceanService = new DigitalOceanService(secretService, tenantService);
      _databaseName = databaseName;
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

    //public async Task<string> UpdateAptAsync(string ipv4, string privateKey)
    //{
    //  return await ExecuteCommandAsync(ipv4, privateKey, "nohup apt update > /var/log/accounting/apt-update.log 2>&1 &");
    //}

    //public async Task<string> UpdateAptResultAsync(string ipv4, string privateKey)
    //{
    //  return await ExecuteCommandAsync(ipv4, privateKey, "cat /var/log/accounting/apt-update.log");
    //}

    //public async Task<string> InstallDotnetSdkAsync(string ipv4, string privateKey)
    //{
    //  return await ExecuteCommandAsync(ipv4, privateKey, "nohup sudo snap install dotnet-sdk --channel=8.0/stable --classic > /var/log/accounting/install-dotnet.log 2>&1 &");
    //}

    //public async Task<string> InstallDotnetResultAsync(string? ipv4, string sshPrivate)
    //{
    //  return await ExecuteCommandAsync(ipv4!, sshPrivate, "cat /var/log/accounting/install-dotnet.log");
    //}

    //public async Task<string> CloneRepositoryAsync(string ipv4, string privateKey, string repoUrl)
    //{
    //  return await ExecuteCommandAsync(ipv4, privateKey, $"nohup git clone {repoUrl} /opt/accounting > /var/log/accounting/git-clone.log 2>&1 &");
    //}

    //public async Task<string> CloneRepositoryResultAsync(string ipv4, string privateKey)
    //{
    //  return await ExecuteCommandAsync(ipv4, privateKey, "cat /var/log/accounting/git-clone.log");
    //}

    //public async Task<string> CreateLogDirectoryAsync(string ipv4, string privateKey)
    //{
    //  return await ExecuteCommandAsync(ipv4, privateKey, "mkdir -p /var/log/accounting && echo 'Log directory created successfully' > /var/log/accounting/create-log-directory.log");
    //}

    //public async Task<string> CreateLogDirectoryResultAsync(string? ipv4, string sshPrivate)
    //{
    //  return await ExecuteCommandAsync(ipv4!, sshPrivate, "cat /var/log/accounting/create-log-directory.log");
    //}

    //public async Task<string> InstallNginxResultAsync(string? ipv4, string sshPrivate)
    //{
    //  return await ExecuteCommandAsync(ipv4!, sshPrivate, "cat /var/log/accounting/install-nginx.log");
    //}

    //public async Task<string> InstallNginxAsync(string ipAddress, string privateKey)
    //{
    //  return await ExecuteCommandAsync(ipAddress, privateKey, "nohup apt install nginx -y > /var/log/accounting/install-nginx.log 2>&1 &");
    //}

    public class DigitalOceanService
    {
      private readonly SecretService _secretService;
      private readonly TenantService _tenantService;

      public DigitalOceanService(SecretService secretService, TenantService tenantService)
      {
        _secretService = secretService;
        _tenantService = tenantService;
      }

      public async Task CreateDropletAsync(Tenant tenant, int organizationId)
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

        string setupScript = """
#!/bin/bash

# Update package lists
sudo apt-get update > /dev/null 2>&1

# Install Nginx
sudo apt-get install -y nginx > /dev/null 2>&1

# Install .NET SDK
# ----------------
# Install required packages
sudo apt-get install -y wget gpg > /dev/null 2>&1

# Add Microsoft GPG key and repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb > /dev/null 2>&1
sudo dpkg -i packages-microsoft-prod.deb > /dev/null 2>&1

# Update package index
sudo apt-get update > /dev/null 2>&1

# Install the .NET SDK
sudo apt-get install -y dotnet-sdk-8.0 > /dev/null 2>&1
# ----------------

# Install PostgreSQL
sudo apt-get install -y postgresql > /dev/null 2>&1

# Install PostGIS
sudo apt-get install -y postgis > /dev/null 2>&1

# Clone repository
git clone https://github.com/denys-olleik/accounting /opt/accounting > /dev/null 2>&1

# Create log directory
sudo mkdir -p /var/log/accounting > /dev/null 2>&1

# Indicate successful setup
echo "Setup completed successfully" > /var/log/custom-setup.log
""";

        var dropletRequest = new DigitalOcean.API.Models.Requests.Droplet()
        {
          Name = tenant.FullyQualifiedDomainName,
          Region = "nyc",
          Size = "s-1vcpu-512mb-10gb",
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