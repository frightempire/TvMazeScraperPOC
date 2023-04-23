using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TvMazeScraper.Domain.Entities;

namespace TvMazeScraper.Infrastructure.Persistence;

public class SqLiteTvMazeContext : DbContext
{
    private readonly IConfiguration _config;

    public SqLiteTvMazeContext(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public DbSet<TvShow> Shows { get; set; }

    public DbSet<CastMember> Cast { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_config.GetConnectionString("SqLiteTvMaze"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TvShow>().HasMany(e => e.Cast).WithMany();
        modelBuilder.Entity<CastMember>();
    }
}