using InsomniaShop.Models;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("AllowAll");

string dbPath = "shop.db";

string HashPassword(string password)
{
    using var sha = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}

using (var con = new SqliteConnection($"Data Source={dbPath}"))
{
    con.Open();

    var cmd = con.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT UNIQUE,
            PasswordHash TEXT
        );

        CREATE TABLE IF NOT EXISTS Games (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT,
            Price REAL,
            ImageUrl TEXT
        );
    ";
    cmd.ExecuteNonQuery();

    cmd.CommandText = "SELECT COUNT(*) FROM Games";
    long count = (long)cmd.ExecuteScalar();

    if (count == 0)
    {
        cmd.CommandText = @"
            INSERT INTO Games (Name, Price, ImageUrl) VALUES
            ('Hytale', 29.99, 'https://cdn.arcanitegames.ca/18614a82e6bad2075066d91fa66cb2f5_hero.jpg'),
            ('Dead By Daylight', 14.99, 'https://m.media-amazon.com/images/M/MV5BZTQ3OWJiYjQtOGYzNC00MGIyLWJhZmMtZjNjYWM5YWVkMmU2XkEyXkFqcGc@._V1_FMjpg_UX1000_.jpg'),
            ('Doctor Lunatic', 3.99, 'https://static-cdn.jtvnw.net/ttv-boxart/1324431838_IGDB-272x380.jpg'),
            ('Naruto Online', 6.99, 'https://assets-prd.ignimgs.com/2024/05/07/naruonline-1715047627239.jpg?crop=1%3A1&format=jpg&auto=webp&quality=80'),
            ('Acod''s Mod', 1337.69, 'https://i.imgur.com/slaK78G.png');
        ";
        cmd.ExecuteNonQuery();
    }
}

app.MapGet("/api/games", () =>
{
    var games = new List<Game>();

    using var con = new SqliteConnection($"Data Source={dbPath}");
    con.Open();

    var cmd = con.CreateCommand();
    cmd.CommandText = "SELECT * FROM Games";

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        games.Add(new Game
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Price = reader.GetDouble(2),
            ImageUrl = reader.GetString(3)
        });
    }

    return games;
});

app.MapGet("/api/users", () =>
{
    var users = new List<User>();

    using var con = new SqliteConnection($"Data Source={dbPath}");
    con.Open();

    var cmd = con.CreateCommand();
    cmd.CommandText = "SELECT * FROM Users";

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        users.Add(new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            PasswordHash = reader.GetString(2)
        });
    }

    return users;
});

app.MapPost("/api/auth/register", (UserDto userDto) =>
{
    using var con = new SqliteConnection($"Data Source={dbPath}");
    con.Open();

    var cmd = con.CreateCommand();
    cmd.CommandText = @"
        INSERT INTO Users (Username, PasswordHash)
        VALUES ($u, $p)";
    cmd.Parameters.AddWithValue("$u", userDto.Username);
    cmd.Parameters.AddWithValue("$p", HashPassword(userDto.Password));

    try
    {
        cmd.ExecuteNonQuery();
        return Results.Ok("Registered successfully");
    }
    catch
    {
        return Results.BadRequest("User may exist");
    }
});

app.MapPost("/api/auth/login", (UserDto userDto) =>
{
    using var con = new SqliteConnection($"Data Source={dbPath}");
    con.Open();

    var cmd = con.CreateCommand();
    cmd.CommandText = @"
        SELECT Id, Username FROM Users
        WHERE Username=$u AND PasswordHash=$p";
    cmd.Parameters.AddWithValue("$u", userDto.Username);
    cmd.Parameters.AddWithValue("$p", HashPassword(userDto.Password));

    using var reader = cmd.ExecuteReader();
    if (!reader.Read())
        return Results.Unauthorized();

    return Results.Ok(new
    {
        Id = reader.GetInt32(0),
        Username = reader.GetString(1)
    });
});

app.Run();