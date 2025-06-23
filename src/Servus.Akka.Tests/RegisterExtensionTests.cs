using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Servus.Akka.Tests;

public class RegisterExtensionTests
{
    public class TestActor1 : ReceiveActor
    {
    }
    public class TestActor2 : ReceiveActor
    {
    }

    [Fact]
    public void RegisterMultipleActors()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddAkka("", configurationBuilder =>
        {
            configurationBuilder
                .WithResolvableActors(helper =>
                {
                    helper
                        .Register<TestActor1>()
                        .Register<TestActor2>();
                })
                .WithActors((b, r) =>
                {
                    r.TryGet<TestActor1>(out var actor1);
                    Assert.NotNull(actor1);
                    r.TryGet<TestActor2>(out var actor2);
                    Assert.NotNull(actor2);
                });
        });
    }
    
    [Fact]
    public void RegisterSingleActor()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddAkka("", configurationBuilder =>
        {
            configurationBuilder
                .WithResolvableActor<TestActor1>()
                .WithActors((b, r) =>
                {
                    r.TryGet<TestActor1>(out var actor);
                    Assert.NotNull(actor);
                });
        });
    }
}