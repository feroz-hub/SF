namespace Zentra.Contracts.Events;

public sealed record UserProvisionedEvent(Guid UserId, string UserName, DateTimeOffset OccurredAtUtc);
