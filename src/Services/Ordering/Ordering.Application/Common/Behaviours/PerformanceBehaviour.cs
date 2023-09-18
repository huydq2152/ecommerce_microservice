using System.Diagnostics;
using MediatR;
using Serilog;

namespace Ordering.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _stopwatch;
    private readonly ILogger _logger;

    public PerformanceBehaviour(ILogger logger)
    {
        _stopwatch = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _stopwatch.Start();
        var response = await next();
        _stopwatch.Stop();

        var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
        if (elapsedMilliseconds < 5000) return response;
        var requestName = typeof(TRequest).Name;
        _logger.Warning(
            $"Application Long Running Request: {requestName} ({elapsedMilliseconds} milliseconds) {request}");

        return response;
    }
}