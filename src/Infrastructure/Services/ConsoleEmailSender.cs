using HR.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace HR.Infrastructure.Services;

public sealed class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[DEV EMAIL] To: {Email}\nSubject: {Subject}\n{Body}", toEmail, subject, body);
        return Task.CompletedTask;
    }
}

