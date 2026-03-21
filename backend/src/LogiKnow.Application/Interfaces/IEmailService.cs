namespace LogiKnow.Application.Interfaces;

public interface IEmailService
{
    Task SendSubmissionStatusEmailAsync(
        string toEmail, string submitterName,
        string entityType, string status, string? reason = null,
        CancellationToken ct = default);

    Task SendWelcomeEmailAsync(
        string toEmail, string name,
        CancellationToken ct = default);
}
