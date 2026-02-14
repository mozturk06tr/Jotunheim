using Shared.Domain.Utils;

namespace Shared.Domain;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private IReadOnlyCollection<IDomainEvent> _domainEventsCollection => _domainEvents;

    /* internal void Raise(IDomainEvent domainEvent)
    {
        if (domainEvent is null) throw new ArgumentNullException(nameof(domainEvent));
        _domainEvents.Add(domainEvent);
    } */ // Todo -> Manage Fucking Raise()
    // internal void RaiseFailed(DomainEventFailed failedEvent)
  
    public void ClearEvents () => _domainEvents.Clear();
    
}
public readonly record struct PortfolioId(Guid Value)
{
    public static PortfolioId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct InstrumentId(Guid Value)
{
    public static InstrumentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed class Portfolio : Entity
{
    public PortfolioId Id { get; private set; }
    public string Name { get; private set; } = "";
    public DateTimeOffset CreatedAtUtc { get; private set; }
    private readonly List<Position> _positions = new();
    public IReadOnlyCollection<Position> Positions => _positions;

    private Portfolio() { } // EF

    public Portfolio(PortfolioId portfolioId, string name, DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Portfolio name is required.", nameof(name));

        Id = PortfolioId.New();
        Name = name.Trim();
        CreatedAtUtc = createdAtUtc;

    }

public void AddPosition(InstrumentId instrumentId, decimal quantity, DateTimeOffset createdAtUtc, IClock clock)
    {
        Guard.GuidNotEmpty(instrumentId.Value, nameof(instrumentId));
        Guard.NotZero(quantity, nameof(quantity));
        Guard.NotBefore(createdAtUtc, clock.UtcNow, nameof(createdAtUtc));

        Guard.False(_positions.Any(p => p.InstrumentId == instrumentId), $"Position for InstrumentId {instrumentId} already exists in the portfolio.");
        Guard.NotFuture(createdAtUtc, clock.UtcNow, nameof(createdAtUtc));
        _positions.Add(new Position(Id, instrumentId, quantity, createdAtUtc));
    }
}
public sealed class Position
{
    public PortfolioId PortfolioId { get; private set; }
    public InstrumentId InstrumentId { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
   
    internal enum PositionType : byte
    {
        Call = 1,
        Put  = 2,
    }
    public uint Version { get; private set; }

    private Position() { } // EF

    internal Position(PortfolioId portfolioId, InstrumentId instrumentId, decimal quantity, DateTimeOffset createdAtUtc)
    {
        if (portfolioId.Value == Guid.Empty)  { throw new ArgumentException("PortfolioId required.", nameof(portfolioId)); }
        if (instrumentId.Value == Guid.Empty) { throw new ArgumentException("InstrumentId required.", nameof(instrumentId)); }
        if (quantity == 0m)                   { throw new ArgumentException("Quantity cannot be zero.", nameof(quantity));}

        PortfolioId = portfolioId;
        InstrumentId = instrumentId;
        Quantity = quantity;
        CreatedAtUtc = createdAtUtc;
        Version = Version;
    }

    public void SetQuantity(decimal quantity)
    {
        if (quantity == 0m)
            throw new ArgumentException("Quantity cannot be zero.", nameof(quantity));
        Quantity = quantity;
    }
}

public sealed class Price
{
    public InstrumentId InstrumentId { get; private set; }
    public DateTimeOffset TsUtc { get; private set; }
    public decimal Px { get; private set; }

    private Price() { } // EF

    public Price(InstrumentId instrumentId, DateTimeOffset tsUtc, decimal px)
    {
        if (instrumentId.Value == Guid.Empty)
            throw new ArgumentException("InstrumentId required.", nameof(instrumentId));
        if (px <= 0m)
            throw new ArgumentOutOfRangeException(nameof(px), "Price must be > 0.");

        InstrumentId = instrumentId;
        TsUtc = tsUtc;
        Px = px;
    }
}

public sealed class FxMarket : Entity
{
    public Guid ParityId { get; private set; }
    public DateTimeOffset TsUtc { get; private set; }
    public decimal Px { get; private set; }

    private FxMarket() { } //ef 

    public FxMarket(Guid parityId, DateTimeOffset tsUtc, decimal px)
    {
        if (parityId == Guid.Empty)
            throw new ArgumentException("ParityId required.", nameof(parityId));
        if (px <= 0m)
            throw new ArgumentOutOfRangeException(nameof(px), "Price must be > 0.");

        ParityId = parityId;
        TsUtc = tsUtc;
        Px = px;
    }
}
