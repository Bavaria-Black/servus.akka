using Akka.Actor;
using Akka.Hosting;
using Akka.Util;

namespace Servus.Akka;

public class ActorRef<TActor>(IActorRegistry registry) : IActorRef<TActor>
    where TActor : ActorBase
{
    private readonly IActorRef _actorRef = registry.Get<TActor>();

    public void Tell(object message, IActorRef sender) => _actorRef.Tell(message, sender);

    public bool Equals(IActorRef? other) => _actorRef.Equals(other);

    public int CompareTo(IActorRef? other) => _actorRef.CompareTo(other);

    public ISurrogate ToSurrogate(ActorSystem system) =>  _actorRef.ToSurrogate(system);

    public int CompareTo(object? obj) => _actorRef.CompareTo(obj);

    public ActorPath Path => _actorRef.Path; 
}