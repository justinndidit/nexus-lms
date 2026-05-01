using System;
using webApi.Modules.Notification.Domain.Interfaces;

namespace webApi.Modules.Notification.Application;

public class EmailSender : INotificationSender
{

    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }
    public async Task SendAsync(string subject, string recipiient, string message)
    {
        _logger.LogInformation($"subject: {subject}, recipient: {recipiient}, message: {message}");
        return;
    }
}
