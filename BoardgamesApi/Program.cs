using BoardgamesApi.Data;
using BoardgamesApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BoardgameDbContext>(options => options.UseNpgsql(connectionString));

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BoardgameDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/health", async (BoardgameDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect ? Results.Ok(new {status = "ok"}) : Results.StatusCode(503);
    } catch
    {
        return Results.StatusCode(503);
    }
});

app.MapGet("/api/boardgames", async (string? name, BoardgameDbContext db) =>
{
    var query = db.Boardgames.AsQueryable();
    if (name != null)
    {
        query = query.Where(b => b.Name == name);

    }
    
    var boardgames = await query.OrderBy(b => b.Id).ToListAsync();

    return Results.Ok(boardgames);
});

app.MapPost("/api/boardgames", async (JsonElement input, BoardgameDbContext db) =>
{
    if (input.ValueKind != JsonValueKind.Object ||
        !input.TryGetProperty("name", out var nameElement) ||
        nameElement.ValueKind != JsonValueKind.String ||
        !input.TryGetProperty("players", out var playersElement) ||
        playersElement.ValueKind != JsonValueKind.Number ||
        !playersElement.TryGetInt32(out var players))
    {
        return Results.BadRequest(new ErrorResponse
        {
            Error = "Invalid boardgame"
        });
    }

    var name = nameElement.GetString();

    if (string.IsNullOrEmpty(name) ||
        name.Length > 100 ||
        name.Contains('\0') ||
        players < 0 ||
        players > 1000000)
    {
        return Results.BadRequest(new ErrorResponse
        {
            Error = "Invalid boardgame"
        });
    }

    var boardgame = new Boardgame
    {
        Name = name,
        Players = players
    };

    db.Boardgames.Add(boardgame);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/boardgames/{boardgame.Id}",
        boardgame
    );
});

app.MapGet("/api/boardgames/{id}", async (string id, BoardgameDbContext db) =>
{
    if (!int.TryParse(id, out var boardgameId) || boardgameId < 1)
    {
        return Results.NotFound(new ErrorResponse
        {
            Error = "Boardgame not found"
        });
    }
    var boardgame = await db.Boardgames.FindAsync(boardgameId);

    if (boardgame == null)
    {
        return Results.NotFound(new ErrorResponse
        {
            Error = "Boardgame not found"
        });
    }

    return Results.Ok(boardgame);
});

app.MapDelete("/api/boardgames/{id}", async (string id, BoardgameDbContext db) =>
{
    if (!int.TryParse(id, out var boardgameId) || boardgameId < 1)
    {
        return Results.NotFound(new ErrorResponse
        {
            Error = "Boardgame not found"
        });
    }
    var boardgame = await db.Boardgames.FindAsync(boardgameId);

    if (boardgame == null)
    {
        return Results.NotFound(new ErrorResponse
        {
            Error = "Boardgame not found"
        });
    }

    db.Boardgames.Remove(boardgame);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
