
using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Hosting;
using Akka.Hosting.TestKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servus.Akka.DependencyInjection;

namespace Servus.Akka.Tests;

public class ResolveExtensionTests :  TestKit
{
    protected override void ConfigureAkka(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithResolvableActor<RegisterExtensionTests.TestActor1>();
    }

    [Fact]
    public void ResolveTest()
    {
        var actorRef = Sys.ResolveActor<RegisterExtensionTests.TestActor1>();
        Assert.NotEqual(Nobody.Instance, actorRef);
    }
}