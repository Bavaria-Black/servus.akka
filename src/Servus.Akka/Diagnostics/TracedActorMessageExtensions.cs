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

    public static async Task<T> AskTraced<T>(this IActorRef recipient, object message, TimeSpan? timeout = null)
        => await recipient.AskTraced<T>(message, timeout, CancellationToken.None);

    public static async Task<T> AskTraced<T>(this IActorRef recipient, object message,
        CancellationToken cancellationToken)
        => await recipient.AskTraced<T>(message, null, cancellationToken);

    public static async Task<T> AskTraced<T>(this IActorRef recipient, object message, TimeSpan? timeout,
        CancellationToken cancellationToken)
        => await recipient.AskTraced<T>(new TracedMessageEnvelope(message), timeout, cancellationToken);

    public static async Task<T> AskTraced<T>(this IActorRef recipient, IWithTracing message, TimeSpan? timeout = null)
        => await recipient.AskTraced<T>(message, timeout, CancellationToken.None);

    public static async Task<T> AskTraced<T>(this IActorRef recipient, IWithTracing message,
        CancellationToken cancellationToken)
        => await recipient.AskTraced<T>(message, null, cancellationToken);

    public static async Task<T> AskTraced<T>(this IActorRef recipient, IWithTracing message, TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        message.AddTracing();
        return await recipient.Ask<T>(message, timeout, cancellationToken);
    }

    public static void ForwardTraced(this IActorRef recipient, object message)
        => recipient.ForwardTraced(new TracedMessageEnvelope(message));

    public static void ForwardTraced(this IActorRef recipient, IWithTracing message)
        => recipient.TellTraced(message, ActorCell.GetCurrentSenderOrNoSender());
}