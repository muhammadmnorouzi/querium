using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Arian.Quantiq.Infrastructure.Services;

public class NullEmailSender(ILogger<NullEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation($"Email to {email} with subject '{subject}' has been logged. Message: {htmlMessage}");
        return Task.CompletedTask;
    }
}
