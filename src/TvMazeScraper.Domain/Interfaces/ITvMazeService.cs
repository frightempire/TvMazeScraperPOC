using TvMazeScraper.Domain.Entities;

namespace TvMazeScraper.Domain.Interfaces;

public interface ITvMazeService
{
    Task<IEnumerable<TvShow>> GetTvShowsAsync(int page, CancellationToken cancellationToken = default);

    Task<IEnumerable<CastMember>> GetCastMembersAsync(int tvShowId, CancellationToken cancellationToken = default);
}