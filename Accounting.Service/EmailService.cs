using Accounting.Business;
using Accounting.Common;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Accounting.Service
{
  public class EmailService
  {
    private readonly SecretService _secretService;

    public EmailService(SecretService secretService)
    {
      _secretService = secretService;
    }

    public async Task SendLoginWithoutPasswordAsync(LoginWithoutPassword loginWithoutPassword)
    {
      Secret emailSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Email);
      Secret fromSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.NoReply);
      
      if (emailSecret == null || fromSecret == null)
      {
        throw new Exception("Email secret not found.");
      }

      var client = new SendGridClient(emailSecret!.Value);

      var from = new EmailAddress(fromSecret.Value, ConfigurationSingleton.Instance.ApplicationName);
      var subject = $"Login without password for {ConfigurationSingleton.Instance.ApplicationName}";
      var to = new EmailAddress(loginWithoutPassword.Email);
      var plainTextContent = $"Login code: {loginWithoutPassword.Code}";
      var htmlContent = $"<strong>Login code:</strong><br>{loginWithoutPassword.Code}";

      var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
      var response = await client.SendEmailAsync(msg);
      Console.WriteLine($"SendLoginWithoutPasswordAsync: {response.StatusCode}");
      Console.WriteLine($"EmailSecret: {emailSecret.Value}");
      Console.WriteLine($"FromSecret {fromSecret.Value}");
    }
  }
}