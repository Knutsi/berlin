using Microsoft.AspNetCore.Builder;

namespace Berlin.Library;

public static class BerlinInstaller
{
    public static IApplicationBuilder AddBerlin(this IApplicationBuilder app)
    {
        app.UseMiddleware<BerlinMiddleware>();
        return app;
    }
}