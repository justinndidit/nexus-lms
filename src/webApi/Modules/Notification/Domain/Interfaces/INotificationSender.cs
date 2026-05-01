using System;

namespace webApi.Modules.Notification.Domain.Interfaces;

public interface INotificationSender
{
    //subject, recipiient, message
    public Task SendAsync(string subject, string recipiient, string message);
}
