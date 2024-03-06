using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Common.Logging;

public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;

    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending request to {Url} - Method {Method} - Version {Version}", request.RequestUri,
                request.Method, request.Version);
            
            var response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Received response from {Url} - StatusCode {StatusCode} - ReasonPhrase {ReasonPhrase}",
                    response.RequestMessage.RequestUri, response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                _logger.LogWarning("Received non-success status code {StatusCode} from {Url} - ReasonPhrase {ReasonPhrase}",
                    response.StatusCode, response.RequestMessage.RequestUri, response.ReasonPhrase);
            }
        }
        catch (HttpRequestException e)
        when(e.InnerException is SocketException
             {
                 SocketErrorCode: SocketError.ConnectionRefused
             })
        {
            var hostWithPort = request.RequestUri.IsDefaultPort
                ? request.RequestUri.DnsSafeHost
                : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";
            
            _logger.LogCritical("Connection refused to {Host} - {Message}", hostWithPort, e.Message);
        }

        return new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            RequestMessage = request
        };
    }
}