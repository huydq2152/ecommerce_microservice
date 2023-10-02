using Hangfire.Dashboard;

namespace Hangfire.API.Extensions;

public class AuthorizationFilter: IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}