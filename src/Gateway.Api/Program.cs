using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;
using Shared.Infrastructure;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});


builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres"));

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapPost("/v1/portfolios", async Task<Results<Created<PortfolioResponse>, BadRequest<string>>> (
        CreatePortfolioRequest req,
        AppDbContext db,
        CancellationToken ct) =>
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            return TypedResults.BadRequest("Name is required");

        var now = DateTimeOffset.UtcNow;
        var portfolio = new Portfolio(req.Name, now);
        
        db.Portfolios.Add(portfolio);
        var portfolio = new Portfolio(req.Name, now);
        
        db.Portfolios.Add(portfolio);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/v1/portfolios/{portfolio.Id}", new PortfolioResponse(
            portfolio.Id,
            portfolio.Name,
            portfolio.CreatedAtUtc
        ));
}));

app.MapGet("/v1/portfolios/{id:guid}", async Task<Results<Ok<PortfolioDetailsResponse>, NotFound>> (
    Guid id,
    AppDbContext db,
    CancellationToken ct) =>
{
    var b = await db.Portfolios
        
}))
    