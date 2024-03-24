namespace ArbTech.Infrastructure.Messaging.Outbox;

public enum EventStateType
{
    NotPublished = 0,
    InProgress = 1,
    Published = 2,
    PublishedFailed = 3
}
