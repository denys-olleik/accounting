using Accounting.Business;
using DigitalOcean.API;
using System.Security.Cryptography;
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
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
          Secret? cloudSecret = await _secretService.GetByTypeAsync(Secret.SecretTypeConstants.Cloud, _organizationId);
          if (cloudSecret == null)
          {
            throw new InvalidOperationException("Cloud secret not found");
          }
          var client = new DigitalOceanClient(cloudSecret.Value);

          var publicKey = ConvertToOpenSshFormat(rsa);
          var privateKey = rsa.ToXmlString(true);

          var sshKeyRequest = new DigitalOcean.API.Models.Requests.Key
          {
            Name = "My SSH Public Key",
            PublicKey = publicKey
          };

          DigitalOcean.API.Models.Responses.Key sshKeyResponse;

          sshKeyResponse = await client.Keys.Create(sshKeyRequest);

          var dropletRequest = new DigitalOcean.API.Models.Requests.Droplet()
          {
            Name = "example.com",
            Region = "nyc",
            Size = "s-1vcpu-512mb-10gb",
            Image = "ubuntu-24-04-x64",
            SshKeys = new List<object> { sshKeyResponse.Fingerprint }
          };

          var dropletResponse = await client.Droplets.Create(dropletRequest);
        }
      }

      private static string ConvertToOpenSshFormat(RSACryptoServiceProvider rsa)
      {
        var keyParams = rsa.ExportParameters(false);

        using (var ms = new MemoryStream())
        {
          using (var writer = new BinaryWriter(ms))
          {
            writer.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(7)));
            writer.Write(Encoding.ASCII.GetBytes("ssh-rsa"));

            writer.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(keyParams.Exponent.Length)));
            writer.Write(keyParams.Exponent);

            writer.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(keyParams.Modulus.Length)));
            writer.Write(keyParams.Modulus);
          }

          var publicKey = Convert.ToBase64String(ms.ToArray());
          return $"ssh-rsa {publicKey} generated-key";
        }
      }
    }
  }
}