using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;

namespace Infrastructure.Services
{
    public class EmailService(
        IResend resend,
        IConfiguration configuration,
        ILogger<EmailService> logger
    ) : IEmailService
    {
        private readonly IResend _resend = resend;
        private readonly ILogger<EmailService> _logger = logger;
        private readonly string _fromEmail =
            configuration["Email:FromAddress"] ?? throw new Exception("From email not found");
        private readonly string _toEmail =
            configuration["Email:ToAddress"] ?? throw new Exception("To email not found");

        public async Task<bool> SendContactEmailAsync(Domain.Entities.Contact contact)
        {
            try
            {
                EmailMessage message = new() { From = $"Contact Form <{_fromEmail}>" };
                message.To.Add(_toEmail);
                message.Subject = $"New Contact: {contact.Subject}";
                message.HtmlBody =
                    $@"
                    <h2>New Contact Message</h2>
                    <p><strong>Name:</strong> {contact.Name}</p>
                    <p><strong>Email:</strong> {contact.Email}</p>
                    <p><strong>Subject:</strong> {contact.Subject}</p>
                    <p><strong>Message:</strong></p>
                    <p>{contact.Message}</p>
                    <hr>
                    <p><small>Sent from IP: {contact.IpAddress} at {contact.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC</small></p>";
                message.ReplyTo = contact.Email;

                await _resend.EmailSendAsync(message);

                _logger.LogInformation(
                    "Email sent successfully for contact {ContactId}",
                    contact.Id
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for contact {ContactId}", contact.Id);
                return false;
            }
        }
    }
}
