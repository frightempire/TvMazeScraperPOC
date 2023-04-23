namespace TvMazeScraper.Infrastructure.TvMaze.Entities;

public class TvShowApiDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<CastMemberApiDto>? Cast { get; set; }
}