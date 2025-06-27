using Microsoft.Extensions.DependencyInjection;

namespace Servus.Akka.DependencyInjection;

public class ActorRefProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        var provider = containerBuilder.BuildServiceProvider();
        return new ActorRefServiceProvider(provider);
    }
}