using Berlin.Library.MethodListing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Berlin.Library.MethodExecution;

public static class BerlinInstaller
{
    public static IServiceCollection AddBerlin(this IServiceCollection services, Func<BerlinConfig>? config = null)
    {
        config ??= () => new BerlinConfig(); 
        
        services.AddSingleton(config());
        services.AddSingleton<RpcMethodCache>();
        services.AddSingleton<ServiceInvoker>();
        services.AddSingleton<RpcListRenderer>();
        
        return services;
    }
    
    public static WebApplication UseBerlin(this WebApplication app)
    {
        app.UseMiddleware<BerlinMiddleware>();
        return app;
    }
}