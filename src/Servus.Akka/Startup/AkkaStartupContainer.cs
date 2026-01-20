using Microsoft.Extensions.Hosting;
using Servus.Akka.DependencyInjection;
using Servus.Core.Application.Startup;

namespace Servus.Akka.Startup;

public class AkkaStartupContainer : IHostBuilderSetupContainer
{
    public void ConfigureHostBuilder(IHostBuilder builder)
    {
        builder.UseServiceProviderFactory(new ActorRefProviderFactory());
    }
}