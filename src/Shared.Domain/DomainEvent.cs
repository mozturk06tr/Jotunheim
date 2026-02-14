namespace Shared.Domain;

internal interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}

internal sealed record PositionAdded(
    Guid PortfolioId,
    Guid InstrumentId,
    decimal Quantity,
    DateTimeOffset OccurredOn
    ) : IDomainEvent;

internal sealed record PortfolioCreated(
    Guid PortfolioId,
    string Name,
    DateTimeOffset OccurredOn
    ) : IDomainEvent;

internal sealed record PortfolioCreationFailed(
    string Name,
    Exception Exception,
    string Message,
    DateTimeOffset OccurredOn
    ) : IDomainEvent;

internal sealed record InstrumentPriceUpdated(
    Guid InstrumentId,
    decimal Price,
    DateTimeOffset OccurredOn
    ) : IDomainEvent;


public sealed record InstrumentPriceUpdateFailed(
    Guid InstrumentId,
    decimal Price,
    Exception Exception,
    string Message,
    DateTimeOffset OccurredOn
    ) : IDomainEvent;

internal ref struct DomainEventFailed
{
    internal Exception Exception { get; }
    internal string Message { get; private set; }
    internal DateTimeOffset OccurredOn { get; }
    internal DomainEventFailed(Exception exception, string message)
    {
        Exception = exception;
        Message = message;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}

