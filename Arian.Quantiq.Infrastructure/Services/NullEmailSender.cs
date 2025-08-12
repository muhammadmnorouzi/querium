using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arian.Quantiq.Infrastructure.Services;

public class NullEmailSender(ILogger<NullEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation($"Email to {email} with subject '{subject}' has been logged. Message: {htmlMessage}");
        return Task.CompletedTask;
    }
}
