using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using TvMazeScraper.Domain.Entities;
using TvMazeScraper.Domain.Interfaces;
using TvMazeScraper.Infrastructure.TvMaze.Entities;

namespace TvMazeScraper.Infrastructure.TvMaze;

public class HttpTvMazeService : ITvMazeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    
    public HttpTvMazeService(IHttpClientFactory httpClientFactory, IMapper mapper, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<IEnumerable<TvShow>> GetTvShowsAsync(int page, CancellationToken cancellationToken = default)
    {
        var url = ConstructUrl($"shows?page={page}");
        using var httpClient = _httpClientFactory.CreateClient("TvMazeClient");
        var response = await httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<TvShow>();
        }

        response.EnsureSuccessStatusCode();
        var tvShows = await response.Content.ReadFromJsonAsync<IEnumerable<TvShowApiDto>>(cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<TvShow>>(tvShows);
    }

    public async Task<IEnumerable<CastMember>> GetCastMembersAsync(int tvShowId, CancellationToken cancellationToken = default)
    {
        var url = ConstructUrl($"shows/{tvShowId}/cast");
        using var httpClient = _httpClientFactory.CreateClient("TvMazeClient");
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var cast = await response.Content.ReadFromJsonAsync<IEnumerable<CastMemberApiDto>>(cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<CastMember>>(cast);
    }

    private string ConstructUrl(string relativeUrl) => $"{_config.GetValue<string>("TvMazeBaseUrl")}/{relativeUrl}";
}