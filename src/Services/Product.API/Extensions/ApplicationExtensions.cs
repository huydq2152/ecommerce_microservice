using HealthChecks.UI.Client;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Product.API.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId("microservices_swagger");
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
            options.DisplayRequestDuration();
        });
        app.UseMiddleware<ErrorWrappingMiddleware>();
        
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapDefaultControllerRoute();
        });
    }
}