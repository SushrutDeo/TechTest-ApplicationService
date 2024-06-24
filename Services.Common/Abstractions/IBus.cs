using Services.Common.Abstractions.Model;

namespace Services.Common.Abstractions.Abstractions;

public interface IBus
{
    Task PublishAsync(DomainEvent domainEvent);
}