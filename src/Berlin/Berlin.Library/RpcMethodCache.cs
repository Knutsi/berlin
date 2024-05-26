using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Berlin.Library;

// "Name" is the name of the class and the method, so e.g. UserService.GetUsers
public record RpcMethodCacheEntry(string Name, MethodInfo Method);

public class RpcMethodCache
{
    private readonly Dictionary<string, RpcMethodCacheEntry> _methods = [];

    public RpcMethodCache(ILogger<RpcMethodCache> logger)
    {
        var t1 = DateTime.Now;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var classTypes = assemblies.SelectMany(x => x.GetTypes()).Where(x => x.IsClass);

        foreach (var classType in classTypes)
        {
            var methods = classType.GetMethods();
            foreach (var method in methods)
            {
                var rpcAttribute = method.GetCustomAttribute<RpcAttribute>();
                if (rpcAttribute == null) continue;
                
                var name = classType.Name + "." + method.Name;
                _methods[name] = new (name, method);
            }
        }
        logger.LogInformation("RPC reflection done. Loaded {Count} RPC methods in {Elapsed} ms", 
            _methods.Count, (DateTime.Now - t1).TotalMilliseconds);
    }

    public RpcMethodCacheEntry? GetMethod(string methodName)
    {
        return _methods.GetValueOrDefault(methodName);
    }
    
    public IEnumerable<RpcMethodCacheEntry> AllRpcs()
    {
        return _methods.Values;
    }
}