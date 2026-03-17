using LogiKnow.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LogiKnow.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendSubmissionStatusEmailAsync(
        string toEmail, string submitterName,
        string entityType, string status, string? reason = null,
        CancellationToken ct = default)
    {
        var apiKey = _config["Email:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey == "REPLACE")
        {
            _logger.LogWarning("Email API key not configured. Skipping email to {Email}", toEmail);
            return;
        }

        try
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config["Email:FromAddress"] ?? "noreply@logiknow.com", "LogiKnow");
            var to = new EmailAddress(toEmail, submitterName);

            var subject = $"LogiKnow — Your {entityType} submission has been {status}";
            var body = $@"Dear {submitterName},

Your {entityType} submission has been {status.ToLower()}.
{(reason != null ? $"\nReason: {reason}" : "")}

Thank you for your contribution to LogiKnow.

Best regards,
The LogiKnow Team";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, null);
            await client.SendEmailAsync(msg, ct);
            _logger.LogInformation("Sent submission status email to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }
}
