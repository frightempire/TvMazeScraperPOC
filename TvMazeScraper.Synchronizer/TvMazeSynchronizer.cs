using System.Collections.Concurrent;
using TvMazeScraper.Domain.Entities;
using TvMazeScraper.Domain.Interfaces;
using TvMazeScraper.Infrastructure.Persistence;

namespace TvMazeScraper.Synchronizer;

public class TvMazeSynchronizer : BackgroundService
{
    private const int MaxPageSize = 250;
    private const string LogPrefix = "TvMaze synchronization:";

    private readonly ILogger<TvMazeSynchronizer> _logger;
    private readonly ITvMazeService _tvMazeService;
    private readonly SqLiteTvMazeContext _dbContext;

    public TvMazeSynchronizer(ILogger<TvMazeSynchronizer> logger, ITvMazeService tvMazeService, SqLiteTvMazeContext dbContext)
    {
        _logger = logger;
        _tvMazeService = tvMazeService;
        _dbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{LogPrefix} STARTED.");

            await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
            var lastProcessedId = _dbContext.Shows.Max(x => (int?)x.Id) ?? 0;
            var page = lastProcessedId / MaxPageSize;

            _logger.LogInformation($"{LogPrefix} Retrieved last processed TvShow ID {lastProcessedId}");

            while (!cancellationToken.IsCancellationRequested)
            {
                var tvShows = (await _tvMazeService.GetTvShowsAsync(page, cancellationToken)).ToList();

                var newTvShows = new List<TvShow>();
                foreach (var show in tvShows)
                {
                    if (await _dbContext.Shows.FindAsync(show.Id) == null)
                    {
                        newTvShows.Add(show);
                    }
                }

                if (newTvShows.Any())
                {
                    var groups = await RequestCast(newTvShows, cancellationToken);

                    foreach (var (tvShow, castMembers) in groups)
                    {
                        await PersistTvShowsAndCast(tvShow, castMembers, cancellationToken);
                    }

                    _logger.LogInformation($"{LogPrefix} Completed synchronization of page {page}.");

                    page++;
                    continue;
                }

                _logger.LogInformation($"{LogPrefix} COMPLETED. Will check for updates in 1 hour.");
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{LogPrefix} An error occurred.");
        }
    }

    private async Task<IEnumerable<(TvShow, IEnumerable<CastMember>)>> RequestCast(List<TvShow> newTvShows, CancellationToken cancellationToken)
    {
        var semaphoreSlim = new SemaphoreSlim(initialCount: 10, maxCount: 10);
        var responses = new ConcurrentBag<(TvShow, IEnumerable<CastMember>)>();

        var tasks = newTvShows.Select(async show =>
        {
            await semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                var castMembers = await _tvMazeService.GetCastMembersAsync(show.Id, cancellationToken);
                responses.Add((show, castMembers));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        });
        await Task.WhenAll(tasks);
        return responses.Select(x => x);
    }

    private async Task PersistTvShowsAndCast(TvShow tvShow, IEnumerable<CastMember> cast, CancellationToken cancellationToken)
    {
        foreach (var castMember in cast)
        {
            var existingCastMember = await _dbContext.Cast.FindAsync(castMember.Id);
            if (existingCastMember != null)
            {
                tvShow.Cast?.Add(existingCastMember);
            }
            else
            {
                tvShow.Cast?.Add(castMember);
                _dbContext.Cast.Add(castMember);
            }
        }

        _dbContext.Shows.Add(tvShow);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}