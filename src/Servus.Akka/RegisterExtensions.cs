using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Hosting;

namespace Servus.Akka;

public class ActorRegistrationHelper
{
    private readonly ActorSystem _system;
    private readonly IActorRegistry _registry;
    private readonly DependencyResolver _resolver;

    internal ActorRegistrationHelper(ActorSystem system, IActorRegistry registry)
    {
        _system = system;
        _registry = registry;
        _resolver = DependencyResolver.For(system);
    }

    public ActorRegistrationHelper Register<TActor>(string? name = null, params object[] args)
        where TActor : ActorBase, new()
    {
        var props = _resolver.Props<TActor>(args);
        var actorRef = _system.ActorOf(props, name);
        _registry.Register<TActor>(actorRef);

        return this;
    }
}

public static class RegisterExtensions
{
    public static AkkaConfigurationBuilder WithResolvableActors(this AkkaConfigurationBuilder builder,
        Action<ActorRegistrationHelper> helper)
    {
        return builder.WithActors((system, registry) => { helper(new ActorRegistrationHelper(system, registry)); });
    }

    public static AkkaConfigurationBuilder WithResolvableActor<TActor>(this AkkaConfigurationBuilder builder,
        string? name = null, params object[] args)
        where TActor : ActorBase, new()
    {
        return builder.WithActors((system, registry) =>
        {
            new ActorRegistrationHelper(system, registry)
                .Register<TActor>(name, args);
        });
    }
}