using MediatR;
using Serilog;

namespace Ordering.Application.Common.Behaviours;

public class UnhandledExceptionBehaviours<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    public UnhandledExceptionBehaviours(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (System.Exception e)
        {
            var requestName = typeof(TRequest).Name;
            _logger.Error(e, $"Application Request: Unhandled Exception for Request {requestName} {request}");
            throw;
        }
    }
}