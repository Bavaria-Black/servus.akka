using Akka.Actor;

namespace Servus.Akka;

public interface IActorRef<TActor> : IActorRef where TActor : ActorBase
{
}