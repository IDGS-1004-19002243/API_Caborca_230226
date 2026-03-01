using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CMS_Caborca_API.Services;

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.office365.com";
    public int Port { get; set; } = 587;
    public string UserEmail { get; set; } = "";
    public string Password { get; set; } = "";
    public string DisplayName { get; set; } = "Caborca Boots";
}

public interface IEmailService
{
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string htmlBody);
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _smtp = config.GetSection("Smtp").Get<SmtpSettings>() ?? new SmtpSettings();
        _logger = logger;
    }

    public async Task SendEmailAsync(IEnumerable<string> recipients, string subject, string htmlBody)
    {
        var recipientList = recipients.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        if (recipientList.Count == 0)
        {
            _logger.LogWarning("SendEmailAsync: no hay destinatarios configurados.");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.DisplayName, _smtp.UserEmail));
        foreach (var r in recipientList)
            message.To.Add(MailboxAddress.Parse(r.Trim()));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtp.UserEmail, _smtp.Password);
            await client.SendAsync(message);
            _logger.LogInformation("Email enviado a: {Recipients}", string.Join(", ", recipientList));
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
