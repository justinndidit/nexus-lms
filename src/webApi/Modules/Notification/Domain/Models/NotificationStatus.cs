namespace webApi.Modules.Notification.Domain.Models;

public enum NotificationStatus
{
    Processing,
    Failed,
    Sent,
    Retrying
}