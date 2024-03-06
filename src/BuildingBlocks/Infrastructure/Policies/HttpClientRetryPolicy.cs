using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static void UseImmediateRetryPolicy(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(ImmediateHttpRetry());
    }

    public static void UseLinearRetryPolicy(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(LinearRetryPolicy());
    }

    public static void UseExponentialRetryPolicy(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(ExponentialRetryPolicy());
    }

    private static IAsyncPolicy<HttpResponseMessage> ImmediateHttpRetry()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3,
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey} due to {exception.Exception.Message}");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> LinearRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey} due to {exception.Exception.Message}");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> ExponentialRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        $"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey} due to {exception.Exception.Message}");
                });
    }
}