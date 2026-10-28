using System.Reflection;
using ExpenseTracker.Maui.Services.Config;

namespace ExpenseTracker.Maui.Repositories;

// Dynamic DispatchProxy delegating calls in real-time to either ApiRepository or MockRepository
// based on IServerConfigService.IsLiveApiMode. Solves the architectural issue where ViewModels
// hold static references after user toggles between Live API and Offline Mock mode.
public class DynamicRepositoryProxy<TInterface> : DispatchProxy where TInterface : class
{
    private IServiceProvider _serviceProvider = null!;
    private Type _apiType = null!;
    private Type _mockType = null!;

    public static TInterface Create(IServiceProvider serviceProvider, Type apiType, Type mockType)
    {
        object proxy = Create<TInterface, DynamicRepositoryProxy<TInterface>>();
        var dynamicProxy = (DynamicRepositoryProxy<TInterface>)proxy;
        dynamicProxy._serviceProvider = serviceProvider;
        dynamicProxy._apiType = apiType;
        dynamicProxy._mockType = mockType;
        return (TInterface)proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) return null;

        var serverConfig = _serviceProvider.GetRequiredService<IServerConfigService>();
        var targetType = serverConfig.IsLiveApiMode ? _apiType : _mockType;
        var targetInstance = _serviceProvider.GetRequiredService(targetType);

        try
        {
            return targetMethod.Invoke(targetInstance, args);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }
}
