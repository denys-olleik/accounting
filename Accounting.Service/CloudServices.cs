using Accounting.Business;
using DigitalOcean.API;
using DigitalOcean.API.Models.Requests;
using DigitalOcean.API.Models.Responses;

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
      }

      public async Task CreateDropletAsync(Tenant tenant)
      {
        Secret? cloudSecret = await _secretService.GetByTypeAsync(Secret.SecretTypeConstants.Cloud, _organizationId);
        var client = new DigitalOceanClient(cloudSecret!.Value);

        //DigitalOcean.API.Models.Responses.Key key = await client.Keys.Create(

        var dropletRequest = new Droplet
        {
          Name = "example.com",
          Region = "nyc",
          Size = "s-1vcpu-512mb-10gb",
          Image = "ubuntu-24-04-x64",
          SshKeys = 
        };

        var createdDroplet = await client.Droplets.Create(dropletRequest);
      }
    }
  }
}