using System.Diagnostics;
using System.Reflection;
using System.Text;
using Berlin.Library.MethodListing;
using Microsoft.AspNetCore.Http;

namespace Berlin.Library.MethodExecution;

public class BerlinMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RpcMethodCache _methodCache;
    private readonly RpcListRenderer _listRenderer;
    private readonly string _rpcPrefix;
    private readonly string _listPrefix;

    public BerlinMiddleware(RequestDelegate next, BerlinConfig config, RpcMethodCache methodCache, ServiceInvoker serviceInvoker, RpcListRenderer listRenderer)
    {
        _listPrefix = config.RpcListPath;
        _rpcPrefix = config.RpcUrlRoutePrefix;
        _methodCache = methodCache;
        _listRenderer = listRenderer;

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
        ValidateRequestOrThrow(context);
        var methodName = GetValidMethodNameOrThrow(path);
        var methodInfo = GetRpcMethodOrThrow(path);
        
        // TODO: abort if too long body:
        var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        
        var serviceProvider = context.RequestServices;
        await context.Response.WriteAsync("Boom!"); 
        await _next(context);
    }

    private static void ValidateRequestOrThrow(HttpContext context)
    {
        if (context.Request.Method.ToLower() != "post")
        {
            throw new ArgumentException("Only POST requests are allowed to execute RPCs.");
        }
        
        if (!context.Request.HasJsonContentType())
        {
            throw new ArgumentException("For RPC calls content-Type must be application/json");
        }
        
        if ( context.Request.ContentLength == 0 || context.Request.ContentLength == null)
        {
            throw new ArgumentException("For RPC calls request body cannot be empty or null.");
        }
        
    }

    private async Task<string> HandleListRequestAsync(HttpContext context)
    {
        return _listRenderer.RenderRpcList();
    }

    private string GetValidMethodNameOrThrow(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be only whitespace");
        }
        
        if(!path.StartsWith(_rpcPrefix))
        {
            throw new UnreachableException($"Path ({path}) must start with {_rpcPrefix}. This should have been caught ealier in the code flow.");
        }
        
        var methodName = path.Remove(0, _rpcPrefix.Length); 
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException($"Method name is missing from path {_rpcPrefix}. Has prefix been misconfigured somehow?");
        }

        if (!methodName.Contains(".") || methodName.Length < 3)
        {
            throw new ArgumentException("Method name must be on the format 'Service.Method'");
        }
        
        return methodName;
    }
    
    private MethodInfo GetRpcMethodOrThrow(string path)
    { 
        var method = _methodCache.GetMethod(path);
        if (method == null)
        {
            throw new InvalidOperationException($"Method {path} not found.");
        }
        
        return method.Method;
    }
}

