using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("HR Management", emailSettings["Username"]));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(emailSettings["Host"], int.Parse(emailSettings["Port"]), bool.Parse(emailSettings["EnableSSL"]));
            await smtpClient.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await smtpClient.SendAsync(emailMessage);
            _logger.LogInformation($"Email sent to {email} successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send email to {email}. Error: {ex.Message}");
            throw;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }
}
