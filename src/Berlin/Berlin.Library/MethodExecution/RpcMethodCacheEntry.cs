using System.Reflection;

namespace Berlin.Library.MethodExecution;

public record RpcMethodCacheEntry(string Name, string ServiceName, MethodInfo Method);