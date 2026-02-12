using System;

namespace Shared.Domain;

public readonly record struct PortfolioId(Guid Value);
public readonly record struct InstrumentId(Guid Value);

public sealed class Portfolio
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private readonly List<Position> _positions = new();
    public IReadOnlyList<Position> Positions => _positions;

    private Portfolio() { } // EF

    public Portfolio(string name, DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Portfolio name is required.", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public void AddPosition(Guid instrumentId, decimal quantity, DateTimeOffset createdAtUtc)
    {
        if (instrumentId == Guid.Empty)
            throw new ArgumentException("InstrumentId required.", nameof(instrumentId));
        if (quantity == 0m)
            throw new ArgumentException("Quantity cannot be zero.", nameof(quantity));

        // invariant: one position per instrument in this milestone (enforced also by DB unique index)
        if (_positions.Any(p => p.InstrumentId == instrumentId))
            throw new InvalidOperationException("Position already exists for this instrument.");

        _positions.Add(new Position(Id, instrumentId, quantity, createdAtUtc));
    }
}

public sealed class Instrument
{
    public Guid Id { get; private set; }
    public string Symbol { get; private set; } = "";
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private Instrument() { } // EF

    public Instrument(string symbol, string? description, DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol required.", nameof(symbol));

        Id = Guid.NewGuid();
        Symbol = symbol.Trim().ToUpperInvariant();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedAtUtc = createdAtUtc;
    }
}

public sealed class Position
{
    public Guid PortfolioId { get; private set; }
    public Guid InstrumentId { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    // optimistic concurrency marker (EF will treat as concurrency token)
    public uint Version { get; private set; }

    private Position() { } // EF

    internal Position(Guid portfolioId, Guid instrumentId, decimal quantity, DateTimeOffset createdAtUtc)
    {
        PortfolioId = portfolioId;
        InstrumentId = instrumentId;
        Quantity = quantity;
        CreatedAtUtc = createdAtUtc;
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
    public Guid InstrumentId { get; private set; }
    public DateTimeOffset TsUtc { get; private set; }
    public decimal Px { get; private set; }

    private Price() { } // EF

    public Price(Guid instrumentId, DateTimeOffset tsUtc, decimal px)
    {
        if (instrumentId == Guid.Empty)
            throw new ArgumentException("InstrumentId required.", nameof(instrumentId));
        if (px <= 0m)
            throw new ArgumentException("Price must be > 0.", nameof(px));

        InstrumentId = instrumentId;
        TsUtc = tsUtc;
        Px = px;
    }
}
