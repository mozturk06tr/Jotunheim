using System;

namespace Shared.Domain;

public readonly record struct PortfolioId(Guid Value);
public readonly record struct InstrumentId(Guid Value);


public sealed class Portfolio
{
    public Guid Id { get; }
    public string Name { get; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private readonly List<Position> _postition = new();
    public IReadOnlyList<Position> Positions => _postition.AsReadOnly();

    private Portfolio()
    {
    }

    public Portfolio(string name, DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Portfolio name is required", nameof(name));
        Id = Guid.NewGuid();
        Name = name.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public void AddPosition(Guid instrumentId, decimal qty, DateTimeOffset createdAtUtc)
    {
        if (instrumentId == Guid.Empty)
            throw new ArgumentException("instrumentId is required", nameof(instrumentId));
        if (qty == 0m)
            throw new ArgumentException("qty is required", nameof(qty));

        if (_postition.Any(p => p.InstrumentId == instrumentId))
            throw new ArgumentException("instrumentId is already added", nameof(instrumentId));

        _postition.Add(new Position(Id, instrumentId, qty, createdAtUtc));
    }

    public sealed class Instrument
    {
        public Guid Id { get; private set; }
        public string Symbol { get; private set; }
        public string? Description { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; private set; }

        private Instrument()
        {
        }

        public Instrument(string symbol, string? description, DateTimeOffset createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("instrument symbol is required", nameof(symbol));
            Id = Guid.NewGuid();
            Symbol = symbol.Trim().ToUpperInvariant();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            CreatedAtUtc = createdAtUtc;
        }
    }
}

public sealed class Instrument
{
    public Guid Id { get; private set; }
    public string Symbol { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    
    private Instrument() { }

    public Instrument(string symbol, string? description, DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("instrument symbol is required", nameof(symbol));
        Id = Guid.NewGuid();
        Symbol = symbol.Trim().ToUpperInvariant();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedAtUtc = createdAtUtc;
    }
}

public sealed class Position
{
    public Guid PositionId { get; private set; }
    public Guid InstrumentId { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    
    public uint Version { get; private set; }
    
    private Position() { } // EF

    internal Position(Guid positionId, Guid instrumentId, decimal quantity, DateTimeOffset createdAtUtc)
    {
        PositionId = positionId;
        InstrumentId = instrumentId;
        Quantity = quantity;
    }
    public void SetQuantity(decimal quantity)
    {
        if (quantity == 0m)
            throw new ArgumentException("QTY CANT BE ZERO", nameof(quantity));
        Quantity = quantity;
    }
}

public sealed class Price 
{
    public Guid InstrumentId { get; private set; }
    public DateTimeOffset TsUtc { get; private set; }
    public decimal Px { get; private set; }
    
    private Price() { }

    public Price(Guid instrumentId, DateTimeOffset tsUtc, decimal px)
    {
        if (instrumentId == Guid.Empty)
            throw new ArgumentException("instrumentId is required", nameof(instrumentId));
        if (px <= 0m)
            throw new ArgumentException("px is required", nameof(px));
        InstrumentId = instrumentId;
        TsUtc = tsUtc;
        Px = px;
    }
}