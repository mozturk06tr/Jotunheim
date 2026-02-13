using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;
using Shared.Infrastructure;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextPool<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});


builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services.AddControllers();
var app = builder.Build();

app.MapHealthChecks("/health");

app.MapPost("/v1/portfolios", async Task<Results<Created<PortfolioResponse>, BadRequest<string>>> (
    CreatePortfolioRequest req,
    AppDbContext db,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(req.Name))
        return TypedResults.BadRequest("Name is required.");

    var now = DateTimeOffset.UtcNow;
    var portfolio = new Portfolio(req.Name, now);

    db.Portfolios.Add(portfolio);
    await db.SaveChangesAsync(ct);

    return TypedResults.Created($"/v1/portfolios/{portfolio.Id}", new PortfolioResponse(
        portfolio.Id,
        portfolio.Name,
        portfolio.CreatedAtUtc
    ));
});

app.MapGet("/v1/portfolios/{id:guid}", async Task<Results<Ok<PortfolioDetailsResponse>, NotFound>>(
    Guid id,
    AppDbContext db,
    CancellationToken ct) =>
{
    var p = await db.Portfolios
        .AsNoTracking()
        .Include(x => x.Positions)
        .FirstOrDefaultAsync(x => x.Id == id, ct);

    if (p is null) return TypedResults.NotFound();

    var resp = new PortfolioDetailsResponse(
        p.Id,
        p.Name,
        p.CreatedAtUtc,
        p.Positions.Select(pos => new PositionResponse(
            pos.PortfolioId,
            pos.InstrumentId,
            pos.Quantity,
            pos.CreatedAtUtc
        )).ToArray()
    );

    return TypedResults.Ok(resp);
});

app.MapPost("/v1/portfolios/{id:guid}/positions", async Task<Results<Ok, NotFound, BadRequest<string>, Conflict<string>>>(
    Guid id,
    AddPositionRequest req,
    AppDbContext db,
    CancellationToken ct) =>
{
    var p = await db.Portfolios
        .Include(x => x.Positions)
        .FirstOrDefaultAsync(x => x.Id == id, ct);

    if (p is null) return TypedResults.NotFound();

    if (req.InstrumentId == Guid.Empty)
        return TypedResults.BadRequest("InstrumentId is required.");

    if (req.Quantity == 0m)
        return TypedResults.BadRequest("Quantity cannot be zero.");

    try
    {
        p.AddPosition(req.InstrumentId, req.Quantity, DateTimeOffset.UtcNow);
        await db.SaveChangesAsync(ct);
        return TypedResults.Ok();
    }
    catch (InvalidOperationException ex)
    {
        return TypedResults.Conflict(ex.Message);
    }
});
app.MapControllers();
app.Run();

public sealed record CreatePortfolioRequest(string Name);
public sealed record AddPositionRequest(Guid InstrumentId, decimal Quantity);

public sealed record PortfolioResponse(Guid Id, string Name, DateTimeOffset CreatedAtUtc);

public sealed record PortfolioDetailsResponse(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAtUtc,
    PositionResponse[] Positions);

public sealed record PositionResponse(
    Guid PortfolioId,
    Guid InstrumentId,
    decimal Quantity,
    DateTimeOffset CreatedAtUtc);
