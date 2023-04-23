namespace TvMazeScraper.API.ViewModels;

public class TvShowModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<CastMemberModel>? Cast { get; set; }
}