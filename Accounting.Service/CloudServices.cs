using Accounting.Business;
using DigitalOcean.API;
using DigitalOcean.API.Exceptions;
using DigitalOcean.API.Models.Responses;
using System.Security.Cryptography;

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

        // Generate a new SSH key pair
        var sshKey = GenerateSSHKeyPair();

        // Create the SSH key on DigitalOcean
        var keyRequest = new DigitalOcean.API.Models.Requests.Key
        {
            Name = $"Key for {tenant.Name}",
            PublicKey = sshKey.PublicKey
        };

        Key key;
        try
        {
            key = await client.Keys.Create(keyRequest);
            Console.WriteLine($"SSH key created successfully. ID: {key.Id}, Fingerprint: {key.Fingerprint}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error creating SSH key: {ex.Message}");
            throw;
        }

        var dropletRequest = new DigitalOcean.API.Models.Requests.Droplet()
        {
          Name = "example.com",
          Region = "nyc",
          Size = "s-1vcpu-512mb-10gb",
          Image = "ubuntu-24-04-x64",
          SshKeys = new List<object> { key.Fingerprint }
        };

        try
        {
          var createdDroplet = await client.Droplets.Create(dropletRequest);
          Console.WriteLine($"Droplet created successfully. ID: {createdDroplet.Id}");
        }
        catch (ApiException ex)
        {
          Console.WriteLine($"Error creating droplet: {ex.Message}");
          throw;
        }
      }

      // Add this method to generate an SSH key pair
      private (string PublicKey, string PrivateKey) GenerateSSHKeyPair()
      {
          using (var rsa = new RSACryptoServiceProvider(2048))
          {
              var publicKeyBytes = rsa.ExportRSAPublicKey();
              var privateKeyBytes = rsa.ExportRSAPrivateKey();

              // Convert to Base64
              var publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);
              var privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

              // Format the public key in the expected SSH format
              var publicKeySSH = $"ssh-rsa {publicKeyBase64} generated-key";

              return (
                  PublicKey: publicKeySSH,
                  PrivateKey: privateKeyBase64
              );
          }
      }
    }
  }
}