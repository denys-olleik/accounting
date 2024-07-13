using Accounting.Business;
using Accounting.Common;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Accounting.Service
{
    public class EmailService
    {
        public async Task SendInvitationEmailAsync(Invitation invitation, string baseUrl)
        {
            var apiKey = ConfigurationSingleton.Instance.SendgridKey;
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(ConfigurationSingleton.Instance.NoReplyEmailAddress, ConfigurationSingleton.Instance.ApplicationName);
            var subject = $"Invitation from {ConfigurationSingleton.Instance.ApplicationName}";
            var to = new EmailAddress(invitation.Email, $"{invitation.FirstName} {invitation.LastName}");
            var plainTextContent = $"Hello {invitation.FirstName}, you have been invited to {ConfigurationSingleton.Instance.ApplicationName}. Please use the following link to accept the invitation: {baseUrl}/i/invitation/{invitation.Guid}";
            var htmlContent = $"<strong>Hello {invitation.FirstName},</strong><br>You have been invited to {ConfigurationSingleton.Instance.ApplicationName}.<br><a href='{baseUrl}/i/invitation/{invitation.Guid}'>Click here</a> to accept the invitation.";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}