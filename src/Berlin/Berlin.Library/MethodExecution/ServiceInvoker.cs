using System.Reflection;

namespace Berlin.Library.MethodExecution;

public class ServiceInvoker
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceInvoker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object InvokeMethod(MethodInfo methodInfo)
    {
        // Get the service type from the MethodInfo
        Type serviceType = methodInfo.DeclaringType;
        
        // Resolve the service from the IServiceProvider
        object service = _serviceProvider.GetService(serviceType);
        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {serviceType} not found.");
        }

        // Get the method parameters
        ParameterInfo[] parameters = methodInfo.GetParameters();
        object[] parameterValues = new object[parameters.Length];

        // Resolve the parameters from the IServiceProvider
        for (int i = 0; i < parameters.Length; i++)
        {
            parameterValues[i] = _serviceProvider.GetService(parameters[i].ParameterType);
            if (parameterValues[i] == null)
            {
                throw new InvalidOperationException($"Unable to resolve service for parameter type {parameters[i].ParameterType}.");
            }
        }

        // Invoke the method
        return methodInfo.Invoke(service, parameterValues);
    }
}