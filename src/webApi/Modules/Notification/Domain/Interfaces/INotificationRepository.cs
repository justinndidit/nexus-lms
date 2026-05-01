namespace webApi.Modules.Notification.Domain.Interfaces;

public interface INotificationRepository
{
    public Task<Models.Notification> Create(Models.Notification notification);
}
