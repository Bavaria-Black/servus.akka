using Akka.Actor;
using Akka.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Servus.Akka.DependencyInjection;

public class ActorRefServiceProvider(IServiceProvider inner) : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        var service = inner.GetService(serviceType);
        if (service != null) return service;

        // Custom fallback logic
        if (!serviceType.IsAssignableTo(typeof(IActorRef)) && serviceType.GenericTypeArguments.Length > 0) return null!;
        var registry = inner.GetRequiredService<IActorRegistry>();
        //var actorRef = registry.TryGet(serviceType.GenericTypeArguments.First(), out var actor);
        
        // Cache this for better performance
        var genericType = typeof(ActorRef<>).MakeGenericType(serviceType.GenericTypeArguments.First());
        var constructor = genericType.GetConstructor([typeof(IActorRegistry)]);
        var a = constructor?.Invoke([registry]) ?? null!;
        return a;
    }

    public object GetRequiredService(Type serviceType)
    {
        return GetService(serviceType) ?? throw new InvalidOperationException($"Service {serviceType} not found");
    }
}