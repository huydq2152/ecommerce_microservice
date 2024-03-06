using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder,
        int retryCount = 3)
    {
        return builder.AddPolicyHandler(ConfigureImmediateHttpRetry(retryCount));
    }

    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder,
        int retryCount = 3, int secondBetweenRetries = 3)
    {
        return builder.AddPolicyHandler(ConfigureLinearHttpRetry(retryCount, secondBetweenRetries));
    }

    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder,
        int retryCount = 3, int baseOfExponentiation = 2)
    {
        return builder.AddPolicyHandler(ConfigureExponentialHttpRetry(retryCount, baseOfExponentiation));
    }

    public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder,
        int eventsAllowedBeforeBreaking = 3, int fromSeconds = 30)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy(eventsAllowedBeforeBreaking, fromSeconds));
    }

    public static IHttpClientBuilder ConfigureTimeoutPolicy(this IHttpClientBuilder builder, int seconds = 5)
    {
        return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(seconds));
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(retryCount,
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey} due to {exception.Exception.Message}");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry(int retryCount, int secondBetweenRetries)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(secondBetweenRetries),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey} due to {exception.Exception.Message}");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry(int retryCount,
        int baseOfExponentiation)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(baseOfExponentiation, retryAttempt)),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey} due to {exception.Exception.Message}");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(int eventsAllowedBeforeBreaking,
        int fromSeconds)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: eventsAllowedBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(fromSeconds)
            );
    }
}