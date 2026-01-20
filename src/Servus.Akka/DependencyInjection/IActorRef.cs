using Akka.Actor;

namespace Servus.Akka.DependencyInjection;

public interface IActorRef<TActor> : IActorRef where TActor : ActorBase
{
}