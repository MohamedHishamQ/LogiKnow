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

    public async Task SendWelcomeEmailAsync(string toEmail, string name, CancellationToken ct = default)
    {
        var subject = "Welcome to LogiKnow!";
        var body = $@"
<div style='font-family: Inter, Arial, sans-serif; max-width: 600px; margin: 0 auto; background: #0a0e1a; color: #f0f4ff; padding: 40px; border-radius: 16px;'>
    <div style='text-align: center; margin-bottom: 32px;'>
        <h1 style='color: #d4a843; font-size: 28px; margin: 0;'>⚓ LogiKnow</h1>
        <p style='color: #22d3ee; font-size: 14px; margin-top: 4px;'>Logistics Knowledge Platform</p>
    </div>
    <h2 style='color: #ffffff;'>Welcome aboard, {name}!</h2>
    <p style='color: #94a3b8; line-height: 1.8;'>
        Thank you for joining LogiKnow — the comprehensive knowledge platform for the logistics industry.
        You can now explore terminology, browse our book catalog, search academic research, and contribute your own knowledge.
    </p>
    <div style='text-align: center; margin: 32px 0;'>
        <a href='https://logiknow.com' style='background: linear-gradient(135deg, #d4a843, #b8922e); color: #0a0e1a; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 700;'>Explore LogiKnow</a>
    </div>
    <hr style='border: none; border-top: 1px solid rgba(255,255,255,0.1); margin: 32px 0;' />
    <p style='color: #64748b; font-size: 12px; text-align: center;'>© 2026 LogiKnow. All rights reserved.</p>
</div>";

        await SendEmailAsync(toEmail, name, subject, body, ct);
    }

    public async Task SendSubmissionStatusEmailAsync(
        string toEmail, string submitterName,
        string entityType, string status, string? reason = null,
        CancellationToken ct = default)
    {
        var isApproved = status.Equals("approved", StringComparison.OrdinalIgnoreCase);
        var subject = $"LogiKnow — Your {entityType} submission has been {status}";
        var statusColor = isApproved ? "#22c55e" : "#ef4444";
        var statusIcon = isApproved ? "✅" : "❌";

        var reasonBlock = reason != null
            ? $"<div style='background: rgba(239,68,68,0.1); border-left: 4px solid #ef4444; padding: 16px; border-radius: 8px; margin: 16px 0;'><strong style='color: #ef4444;'>Reason:</strong><br/><span style='color: #94a3b8;'>{reason}</span></div>"
            : "";

        var body = $@"
<div style='font-family: Inter, Arial, sans-serif; max-width: 600px; margin: 0 auto; background: #0a0e1a; color: #f0f4ff; padding: 40px; border-radius: 16px;'>
    <div style='text-align: center; margin-bottom: 32px;'>
        <h1 style='color: #d4a843; font-size: 28px; margin: 0;'>⚓ LogiKnow</h1>
        <p style='color: #22d3ee; font-size: 14px; margin-top: 4px;'>Logistics Knowledge Platform</p>
    </div>
    <h2 style='color: #ffffff;'>Dear {submitterName},</h2>
    <p style='color: #94a3b8; line-height: 1.8;'>
        Your <strong style='color: #ffffff;'>{entityType}</strong> submission has been
        <span style='color: {statusColor}; font-weight: 700;'>{statusIcon} {status}</span>.
    </p>
    {reasonBlock}
    <p style='color: #94a3b8; line-height: 1.8;'>Thank you for your contribution to LogiKnow.</p>
    <hr style='border: none; border-top: 1px solid rgba(255,255,255,0.1); margin: 32px 0;' />
    <p style='color: #64748b; font-size: 12px; text-align: center;'>© 2026 LogiKnow. All rights reserved.</p>
</div>";

        await SendEmailAsync(toEmail, submitterName, subject, body, ct);
    }

    private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct)
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
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlBody);
            await client.SendEmailAsync(msg, ct);
            _logger.LogInformation("Sent email '{Subject}' to {Email}", subject, toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email '{Subject}' to {Email}", subject, toEmail);
        }
    }
}
