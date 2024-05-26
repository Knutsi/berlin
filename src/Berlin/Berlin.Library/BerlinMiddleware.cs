using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Berlin.Library;

public class BerlinMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RpcMethodCache _methodCache;
    private readonly string _rpcPrefix;
    private readonly string _listPrefix;

    public BerlinMiddleware(RequestDelegate next, BerlinConfig config, RpcMethodCache methodCache)
    {
        _listPrefix = config.RpcListPath;
        _rpcPrefix = config.RpcUrlRoutePrefix;
        _methodCache = methodCache;
        
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.HasValue)
        {
            await _next(context);
            return;
        }

        // return list of RPCs
        var path = context.Request.Path.Value;
        if (path == _listPrefix)
        {
            var listHtml = await HandleListRequestAsync(context);
            await context.Response.WriteAsync(listHtml);
            await _next(context);
            return;
        }
       
        // execute RPC:
        ValidPathOrThrow(path);
        
        var serviceProvider = context.RequestServices;
        await context.Response.WriteAsync("Boom!");
        await _next(context);
    }

    private async Task<string> HandleListRequestAsync(HttpContext context)
    {
        var methods = _methodCache.AllRpcs();
        var htmlBuilder = new StringBuilder("<html><body><ul>");

        foreach (var method in methods)
        {
            var link = $"{context.Request.Scheme}://{context.Request.Host}{_rpcPrefix}{method.Name}";
            htmlBuilder.Append($"<li><a href=\"{link}\">{method.Name}</a></li>");
        }

        htmlBuilder.Append("</ul></body></html>");

        return htmlBuilder.ToString();
    }

    private static void ValidPathOrThrow(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be only whitespace");
        }    
    }
}

