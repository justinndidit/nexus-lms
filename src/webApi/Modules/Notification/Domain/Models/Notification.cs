using System;
using System.Numerics;

namespace webApi.Modules.Notification.Domain.Models;

public class Notification
{
    public Guid Id{get; private set;}
    public string Message{get; private set;} = string.Empty;
    public string Recipient {get; private set;} = string.Empty;
    public string NoficationCategory {get; set;} = string.Empty; //welcome email, advert etc
    public List<NotificationChannel> Channels{get; set;} = [];
    public NotificationStatus Status{get; set;} = NotificationStatus.Processing;
    public BigInteger FailedTimesCount{get; set;} = 0;
    public DateTime FailedAt{get; set;}
    public DateTime LastRetriedAt{get; set;}
    public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;
}
