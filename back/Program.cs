using InsomniaShop.Models;

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

var games = new List<Game>
{
    new Game { Id = 1, Name = "Hytale", Price = 29.99, ImageUrl = "https://cdn.arcanitegames.ca/18614a82e6bad2075066d91fa66cb2f5_hero.jpg" },
    new Game { Id = 2, Name = "Dead By Daylight", Price = 14.99, ImageUrl = "https://m.media-amazon.com/images/M/MV5BZTQ3OWJiYjQtOGYzNC00MGIyLWJhZmMtZjNjYWM5YWVkMmU2XkEyXkFqcGc@._V1_FMjpg_UX1000_.jpg" },
    new Game { Id = 3, Name = "Doctor Lunatic", Price = 3.99, ImageUrl = "https://static-cdn.jtvnw.net/ttv-boxart/1324431838_IGDB-272x380.jpg" },
    new Game { Id = 4, Name = "Naruto Online", Price = 6.99, ImageUrl = "https://assets-prd.ignimgs.com/2024/05/07/naruonline-1715047627239.jpg?crop=1%3A1%2Csmart&format=jpg&auto=webp&quality=80" },
    new Game { Id = 5, Name = "Acod's Mod", Price = 1337.69, ImageUrl = "https://i.imgur.com/slaK78G.png" }
};

app.MapGet("/api/games", () => games);

app.Run();