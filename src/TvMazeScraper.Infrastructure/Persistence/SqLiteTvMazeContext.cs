using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Domain.Entities;

namespace TvMazeScraper.Infrastructure.Persistence;

public class SqLiteTvMazeContext : DbContext
{
    public DbSet<TvShow> Shows { get; set; }

    public DbSet<CastMember> Cast { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Environment.GetEnvironmentVariable("ConnectionStrings__TvMazeDb"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TvShow>().HasMany(e => e.Cast).WithMany();
        modelBuilder.Entity<CastMember>();
    }
}