using Accounting.Business;
using Accounting.Common;
using DigitalOcean.API;

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
        var client = new DigitalOceanClient(cloudSecret!.Value);

        //var (publicKey, privateKey) = SshKeyGenerator.GenerateKeyPair();

        //var key = await client.Keys.Create(new DigitalOcean.API.Models.Requests.Key
        //{
        //  Name = tenant.Name + " Key",
        //  PublicKey = publicKey
        //});

        var dropletRequest = new DigitalOcean.API.Models.Requests.Droplet()
        {
          Name = "example.com",
          Region = "nyc",
          Size = "s-1vcpu-512mb-10gb",
          Image = "ubuntu-24-04-x64",
          //SshKeys = new List<object> { key.Fingerprint }
        };

        var createdDroplet = await client.Droplets.Create(dropletRequest);
      }
    }
  }
}