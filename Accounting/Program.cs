using Accounting.Business;
using Accounting.Common;
using Accounting.Events;
using Accounting.Filters;
using Accounting.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using static Accounting.Business.Claim;

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
builder.Services.AddScoped<RequestContext>();
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
builder.Services.AddTransient<EmailService>();
builder.Services.AddTransient<LoginWithoutPasswordService>();
builder.Services.AddTransient<InvitationService>();
builder.Services.AddTransient<TagService>();
builder.Services.AddTransient<UserTaskService>();
builder.Services.AddTransient<ToDoService>();
builder.Services.AddTransient<ToDoTagService>();

ConfigurationSingleton.Instance.ApplicationName = builder.Configuration["ApplicationName5"];
ConfigurationSingleton.Instance.ConnectionStringDefaultPsql = builder.Configuration["ConnectionStrings:Psql"];
ConfigurationSingleton.Instance.ConnectionStringAdminPsql = builder.Configuration["ConnectionStrings:AdminPsql"];
ConfigurationSingleton.Instance.InvitationExpirationMinutes = Convert.ToInt32(builder.Configuration["InvitationExpirationMinutes"]);
ConfigurationSingleton.Instance.TempPath = builder.Configuration["TempPath"];
ConfigurationSingleton.Instance.PermPath = builder.Configuration["PermPath"];

var app = builder.Build();

#region Reset-Database
if (app.Environment.IsDevelopment())
{
  var databaseResetConfigPath = Path.Combine(app.Environment.ContentRootPath, "database-reset.json");
  var databaseResetConfig = JsonConvert.DeserializeObject<DatabaseResetConfig>(System.IO.File.ReadAllText(databaseResetConfigPath));

  if (databaseResetConfig!.Reset)
  {
    DatabaseService databaseService = new DatabaseService();

    await databaseService.ResetDatabase();

    string sampleDataPath = Path.Combine(AppContext.BaseDirectory, "sample-data.sql");
    string sampleDataScript = System.IO.File.ReadAllText(sampleDataPath);

    await databaseService.RunSQLScript(sampleDataScript, DatabaseThing.DatabaseConstants.Database);

    databaseResetConfig.Reset = false;
    System.IO.File.WriteAllText(databaseResetConfigPath, JsonConvert.SerializeObject(databaseResetConfig, Formatting.Indented));
  }
}
#endregion

#region LoadTenantManagementConfiguration
ConfigurationSingleton.Instance.TenantManagement
    = Convert.ToBoolean(builder.Configuration["TenantManagement"]);
if (!ConfigurationSingleton.Instance.TenantManagement)
  await LoadTenantManagementFromDatabase(app);
#endregion

// Exception handling
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

app.Use(async (context, next) =>
{
  var requestContext = context.RequestServices.GetRequiredService<RequestContext>();

  if (context.User.Identity?.IsAuthenticated == true)
  {
    var databaseNameClaim = context.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypeConstants.DatabaseName);
    if (databaseNameClaim != null)
    {
      requestContext.DatabaseName = databaseNameClaim.Value;
    }
  }
  else
  {
    requestContext.DatabaseName = DatabaseThing.DatabaseConstants.Database;
  }

  await next();
});

app.UseMiddleware<UpdateClaimsMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task LoadTenantManagementFromDatabase(WebApplication app)
{
  using var scope = app.Services.CreateScope();
  var secretService = scope.ServiceProvider.GetRequiredService<SecretService>();
  var tenantManagement = await secretService.GetAsync(Secret.SecretTypeConstants.TenantManagement);

  if (tenantManagement != null)
  {
    ConfigurationSingleton.Instance.TenantManagement
        = Convert.ToBoolean(tenantManagement.Value);
  }
}