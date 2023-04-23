using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace TvMazeScraper.Infrastructure.TvMaze;

public class HttpPolicyProvider
{
    public IAsyncPolicy<HttpResponseMessage> GetHttpPolicies()
    {
        var simpleRetryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode is HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var tooManyRequestsRetryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode is HttpStatusCode.TooManyRequests && msg.Headers.RetryAfter != null)
            .WaitAndRetryAsync(2,
                sleepDurationProvider: (_, result, _) =>
                {
                    var httpResponse = result.Result;
                    var retryAfter = httpResponse.Headers.RetryAfter;
                    return retryAfter == null
                        ? TimeSpan.FromSeconds(10)
                        : retryAfter.Date.HasValue
                            ? retryAfter.Date.Value - DateTime.UtcNow
                            : retryAfter.Delta.GetValueOrDefault();
                },
                onRetryAsync: (_, _, _, _) => Task.CompletedTask);

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

        return Policy.WrapAsync(simpleRetryPolicy, tooManyRequestsRetryPolicy, circuitBreakerPolicy);
    }
}