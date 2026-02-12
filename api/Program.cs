using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Read configuration from environment variables (works in Docker cleanly)
string pgConn = builder.Configuration["POSTGRES_CONNECTION"]
    ?? "Host=localhost;Port=5432;Database=appdb;Username=app;Password=app;Pooling=true;";

string redisConn = builder.Configuration["REDIS_CONNECTION"]
    ?? "localhost:6379,abortConnect=false";

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConn));

builder.Services.AddSingleton<Func<NpgsqlConnection>>(_ =>
    () => new NpgsqlConnection(pgConn));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    utc = DateTimeOffset.UtcNow
}));

// Prove DB connectivity by asking the server time
app.MapGet("/db-time", async (Func<NpgsqlConnection> connFactory, CancellationToken ct) =>
{
    await using var conn = connFactory();
    await conn.OpenAsync(ct);

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "select now()";
    var result = await cmd.ExecuteScalarAsync(ct);

    return Results.Ok(new { db_now = result?.ToString() });
});

// Cache set/get to prove Redis connectivity
app.MapPost("/cache/{key}", async (string key, HttpRequest req, IConnectionMultiplexer mux) =>
{
    using var reader = new StreamReader(req.Body);
    var value = await reader.ReadToEndAsync();

    var db = mux.GetDatabase();
    await db.StringSetAsync(key, value);

    return Results.Ok(new { key, value });
});

app.MapGet("/cache/{key}", async (string key, IConnectionMultiplexer mux) =>
{
    var db = mux.GetDatabase();
    var value = await db.StringGetAsync(key);

    return value.HasValue
        ? Results.Ok(new { key, value = value.ToString() })
        : Results.NotFound(new { key, error = "not found" });
});

app.Run();
