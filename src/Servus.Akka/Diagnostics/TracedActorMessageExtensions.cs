using Akka.Actor;
using Servus.Akka.Messaging;
using Servus.Core.Diagnostics;

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

    public static Task<T> AskTraced<T>(this IActorRef recipient, object message, TimeSpan? timeout = null)
        => recipient.AskTraced<T>(message, timeout, CancellationToken.None);

    public static Task<T> AskTraced<T>(this IActorRef recipient, object message, CancellationToken cancellationToken)
        => recipient.AskTraced<T>(message, null, cancellationToken);

    public static Task<T> AskTraced<T>(this IActorRef recipient, object message, TimeSpan? timeout,
        CancellationToken cancellationToken)
        => recipient.AskTraced<T>(new TracedMessageEnvelope(message), timeout, cancellationToken);

    public static Task<T> AskTraced<T>(this IActorRef recipient, IWithTracing message, TimeSpan? timeout = null)
        => recipient.AskTraced<T>(message, timeout, CancellationToken.None);

    public static Task<T> AskTraced<T>(this IActorRef recipient, IWithTracing message, CancellationToken cancellationToken)
        => recipient.AskTraced<T>(message, null, cancellationToken);

    public static Task<T> AskTraced<T>(this IActorRef recipient, IWithTracing message, TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        message.AddTracing();
        return recipient.Ask<T>(message, timeout, cancellationToken);
    }

    public static void ForwardTraced(this IActorRef recipient, object message)
        => recipient.ForwardTraced(new TracedMessageEnvelope(message));

    public static void ForwardTraced(this IActorRef recipient, IWithTracing message)
        => recipient.TellTraced(message, ActorCell.GetCurrentSenderOrNoSender());
}