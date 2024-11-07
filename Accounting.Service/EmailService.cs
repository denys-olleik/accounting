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

    public async Task SendInvitationEmailAsync(Invitation invitation, string baseUrl)
    {
      Secret emailSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Email);
      var client = new SendGridClient(emailSecret!.Value);

      Secret fromSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.NoReply);

      var from = new EmailAddress(fromSecret.Value, ConfigurationSingleton.Instance.ApplicationName);
      var subject = $"Invitation from {ConfigurationSingleton.Instance.ApplicationName}";
      var to = new EmailAddress(invitation.Email, $"{invitation.FirstName} {invitation.LastName}");
      var plainTextContent = $"Hello {invitation.FirstName}, you have been invited to {ConfigurationSingleton.Instance.ApplicationName}. Use the following link to accept the invitation: {baseUrl}/i/invitation/{invitation.Guid}";
      var htmlContent = $"<strong>Hello {invitation.FirstName},</strong><br>You have been invited to {ConfigurationSingleton.Instance.ApplicationName}.<br><a href='{baseUrl}/i/invitation/{invitation.Guid}'>Click here</a> to accept the invitation.";

      var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
      var response = await client.SendEmailAsync(msg);
    }

    public async Task SendLoginWithoutPasswordAsync(LoginWithoutPassword loginWithoutPassword)
    {
      var emailSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Email);
      var client = new SendGridClient(emailSecret!.Value);

      Secret fromSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.NoReply);

      var from = new EmailAddress(fromSecret.Value, ConfigurationSingleton.Instance.ApplicationName);
      var subject = $"Login without password for {ConfigurationSingleton.Instance.ApplicationName}";
      var to = new EmailAddress(loginWithoutPassword.Email);
      var plainTextContent = $"Login code: {loginWithoutPassword.Code}";
      var htmlContent = $"<strong>Login code:</strong><br>{loginWithoutPassword.Code}";

      var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
      var response = await client.SendEmailAsync(msg);
    }
  }
}