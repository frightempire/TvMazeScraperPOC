using Microsoft.EntityFrameworkCore;
using Serilog;
using TvMazeScraper.API.ViewModels;
using TvMazeScraper.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Host.UseSerilog((_, _, configuration) => configuration.WriteTo.File(Path.Combine("Logs", "tvMazeApi-log.txt")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(TvMazeApiModelMappingProfile));

builder.Services.AddDbContext<SqLiteTvMazeContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("SqLiteTvMaze")),
    ServiceLifetime.Transient);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();