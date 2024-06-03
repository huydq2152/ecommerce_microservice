namespace Shared.Configuration;

public class ApiConfiguration
{
    public string ApiName { get; set; }
    public string ApiVersion { get; set; }
    public string IdentityServerBaseUrl { get; set; }
    public string IssuerUri { get; set; }
    public string ApiBaseUrl { get; set; }
    public string ClientId { get; set; }
}