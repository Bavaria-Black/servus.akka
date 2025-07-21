using Akka.Actor;
using Servus.Core.Diagnostics;

namespace Servus.Akka;

public static class TracedActorMessageExtensions
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static void TellTraced(this ICanTell recipient, IWithTracing message, IActorRef? sender = null)
    {
        sender ??= ActorCell.GetCurrentSelfOrNoSender();
        message.AddTracing();
        recipient.Tell(message, sender);
    }

    public static void ForwardTraced(this IActorRef recipient, IWithTracing message)
        => recipient.TellTraced(message, ActorCell.GetCurrentSenderOrNoSender());
}