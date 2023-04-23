using Serilog;
using TvMazeScraper.Domain.Interfaces;
using TvMazeScraper.Infrastructure.Persistence;
using TvMazeScraper.Infrastructure.TvMaze;
using TvMazeScraper.Infrastructure.TvMaze.Entities;
using TvMazeScraper.Synchronizer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) => config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true))
    .UseSerilog((_, _, configuration) => configuration.WriteTo.Console())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddAutoMapper(typeof(TvMazeApiDtoMappingProfile));

        var policyProvider = new HttpPolicyProvider();
        services.AddHttpClient("TvMazeClient").AddPolicyHandler(policyProvider.GetHttpPolicies());
        services.AddTransient<ITvMazeService, HttpTvMazeService>();
        services.AddDbContext<SqLiteTvMazeContext>(ServiceLifetime.Transient);

        services.AddHostedService<TvMazeSynchronizer>();
    })
    .Build();

host.Run();