using System;
using webApi.Modules.Notification.Domain.Interfaces;

namespace webApi.Modules.Notification.Application;

public class PushNotificationSender : INotificationSender
{
    private readonly ILogger<EmailSender> _logger;

    public PushNotificationSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }
    public async Task SendAsync(string subject, string recipiient, string message)
    {
        _logger.LogInformation($"subject: {subject}, recipient: {recipiient}, message: {message}");
        return;
    }
}