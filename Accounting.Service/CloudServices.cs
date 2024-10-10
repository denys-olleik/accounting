using Accounting.Business;
using DigitalOcean.API;
using Renci.SshNet;
using System.Text;
namespace Accounting.Service
{
  public class CloudServices
  {
    public DigitalOceanService GetDigitalOceanService(SecretService secretService, TenantService tenantService, int organizationId)
    {
      return new DigitalOceanService(secretService, tenantService, organizationId);
    }

    public class DigitalOceanService
    {
      private readonly SecretService _secretService;
      private readonly TenantService _tenantService;
      private readonly int _organizationId;

      public DigitalOceanService(SecretService secretService, TenantService tenantService, int organizationId)
      {
        _secretService = secretService;
        _tenantService = tenantService;
        _organizationId = organizationId;
      }

      public async Task CreateDropletAsync(Tenant tenant)
      {
        Secret? cloudSecret = await _secretService.GetByTypeAsync(Secret.SecretTypeConstants.Cloud, _organizationId);
        if (cloudSecret == null)
        {
          throw new InvalidOperationException("Cloud secret not found");
        }
        var client = new DigitalOceanClient(cloudSecret.Value);

        var keygen = new SshKeyGenerator.SshKeyGenerator(2048);

        var sshKeyRequest = new DigitalOcean.API.Models.Requests.Key
        {
          Name = tenant.Name,
          PublicKey = keygen.ToRfcPublicKey(tenant.Name)
        };

        var sshKeyResponse = await client.Keys.Create(sshKeyRequest);

        var dropletRequest = new DigitalOcean.API.Models.Requests.Droplet()
        {
          Name = tenant.Name,
          Region = "nyc",
          Size = "s-1vcpu-512mb-10gb",
          Image = "ubuntu-24-04-x64",
          SshKeys = new List<object> { sshKeyResponse.Fingerprint }
        };

        var dropletResponse = await client.Droplets.Create(dropletRequest);

        //await Task.Delay(120000);

        //var droplet = await client.Droplets.Get(dropletResponse.Id);
        //string ipAddress = droplet.Networks.V4.First(n => n.Type == "public").IpAddress;

        //bool success = TestSshConnectionAsync(ipAddress, keygen.ToPrivateKey(), "root");

        //tenant.Ipv4 = ipAddress;
        tenant.SshPublic = keygen.ToRfcPublicKey(tenant.Name);
        tenant.SshPrivate = keygen.ToPrivateKey();
        await _tenantService.UpdateAsync(tenant);
      }
    }

    private bool TestSshConnectionAsync(string ipAddress, string privateKey, string username = "root")
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
  }
}