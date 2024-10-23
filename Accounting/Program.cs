using Accounting.Business;
using Accounting.Common;
using Accounting.Events;
using Accounting.Filters;
using Accounting.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
  options.Filters.Add<PreventIdentifiableResponseFilter>();
})
.AddNewtonsoftJson(options =>
{
  options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
  {
    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
  };
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
      options.SlidingExpiration = true;
      options.EventsType = typeof(CustomCookieAuthenticationEventsHandler);
      options.LoginPath = new PathString("/user-account/login");

      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddScoped<CustomCookieAuthenticationEventsHandler>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.MinimumSameSitePolicy = SameSiteMode.Strict;
  options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddTransient<AddressService>();
builder.Services.AddTransient<BusinessEntityService>();
builder.Services.AddTransient<AccountService>();
builder.Services.AddTransient<JournalInvoiceInvoiceLinePaymentService>();
builder.Services.AddTransient<JournalInvoiceInvoiceLineService>();
builder.Services.AddTransient<JournalService>();
builder.Services.AddTransient<InvoiceAttachmentService>();
builder.Services.AddTransient<InvoiceLineService>();
builder.Services.AddTransient<InvoiceInvoiceLinePaymentService>();
builder.Services.AddTransient<InvoiceService>();
builder.Services.AddTransient<IronPdfService>();
builder.Services.AddTransient<ItemService>();
builder.Services.AddTransient<OrganizationService>();
builder.Services.AddTransient<PaymentService>();
builder.Services.AddTransient<PaymentTermsService>();
builder.Services.AddTransient<UserOrganizationService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<ReconciliationTransactionService>();
builder.Services.AddTransient<ReconciliationService>();
builder.Services.AddTransient<ReconciliationAttachmentService>();
builder.Services.AddTransient<JournalReconciliationTransactionService>();
builder.Services.AddTransient<LocationService>();
builder.Services.AddTransient<InventoryService>();
builder.Services.AddTransient<RequestLogService>();
builder.Services.AddTransient<InventoryAdjustmentService>();
builder.Services.AddTransient<JournalInventoryAdjustmentService>();
builder.Services.AddTransient<TenantService>();
builder.Services.AddTransient<SecretService>();
builder.Services.AddTransient<CloudServices>();
builder.Services.AddTransient<DatabaseService>();

ConfigurationSingleton.Instance.ApplicationName = builder.Configuration["ApplicationName5"];
ConfigurationSingleton.Instance.ConnectionStringPsql = builder.Configuration["ConnectionStrings:Psql"];
ConfigurationSingleton.Instance.AdminPsql = builder.Configuration["ConnectionStrings:AdminPsql"];
ConfigurationSingleton.Instance.InvitationExpirationMinutes = Convert.ToInt32(builder.Configuration["InvitationExpirationMinutes"]);
ConfigurationSingleton.Instance.SendgridKey = builder.Configuration["SendgridKey"];
ConfigurationSingleton.Instance.NoReplyEmailAddress = builder.Configuration["NoReplyEmailAddress"];
ConfigurationSingleton.Instance.TempPath = builder.Configuration["TempPath"];
ConfigurationSingleton.Instance.PermPath = builder.Configuration["PermPath"];

var app = builder.Build();

#region reset-database
if (app.Environment.IsDevelopment())
{
  var databaseResetConfigPath = Path.Combine(app.Environment.ContentRootPath, "database-reset.json");
  var databaseResetConfig = JsonConvert.DeserializeObject<DatabaseResetConfig>(System.IO.File.ReadAllText(databaseResetConfigPath));

  DatabaseService databaseManager = new DatabaseService();
  if (databaseResetConfig!.Reset)
  {
    await databaseManager.ResetDatabase();

    string sampleDataPath = Path.Combine(AppContext.BaseDirectory, "sample-data.sql");

    string sampleDataScript = System.IO.File.ReadAllText(sampleDataPath);

    await databaseManager.RunSQLScript(sampleDataScript, "Accounting");

    databaseResetConfig.Reset = false;
    System.IO.File.WriteAllText(databaseResetConfigPath, JsonConvert.SerializeObject(databaseResetConfig, Formatting.Indented));
  }
}
#endregion

#region LoadTenantManagementConfiguration see Appsettings.json."TenantManagement"
ConfigurationSingleton.Instance.TenantManagement
    = Convert.ToBoolean(builder.Configuration["TenantManagement"]);
if (!ConfigurationSingleton.Instance.TenantManagement)
  await IfTenantManagementIsNotSetTrueAtConfiguration_TryTheDatabaseMaybeItsTrueThere();
#endregion

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
}
else
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task IfTenantManagementIsNotSetTrueAtConfiguration_TryTheDatabaseMaybeItsTrueThere()
{
  ApplicationSettingService applicationSettingsService = new ApplicationSettingService();
  var tenantManagement = await applicationSettingsService.GetAsync(ApplicationSetting.ApplicationSettingsConstants.TenantManagement);

  if (tenantManagement != null)
  {
    ConfigurationSingleton.Instance.TenantManagement
        = Convert.ToBoolean(tenantManagement.Value);
  }
}