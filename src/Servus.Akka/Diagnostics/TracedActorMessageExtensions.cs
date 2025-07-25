using Akka.Actor;
using Servus.Akka.Messaging;
using Servus.Core.Diagnostics;
using Servus.Core.Reflection;

namespace Servus.Akka.Diagnostics;

public static class TracedActorMessageExtensions
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static void TellTraced(this ICanTell recipient, object message, IActorRef? sender = null)
    {
        var msg = message as IWithTracing ?? new TracedMessageEnvelope(message);
        TellTraced(recipient, msg, sender);
    }

    public static void TellTraced(this ICanTell recipient, IWithTracing message, IActorRef? sender = null)
    {
        sender ??= ActorCell.GetCurrentSelfOrNoSender();
        message.AddTracing();
        recipient.Tell(message, sender);
    }

    public static void ForwardTraced(this IActorRef recipient, object message)
        => recipient.ForwardTraced(new TracedMessageEnvelope(message));

    public static void ForwardTraced(this IActorRef recipient, IWithTracing message)
        => recipient.TellTraced(message, ActorCell.GetCurrentSenderOrNoSender());
}