using Akka.Actor;
using Akka.Util;
using Servus.Core.Diagnostics;

namespace Servus.Akka;

public static class ContextExtensions
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static Option<IActorRef> GetChild(this IActorContext context, string name)
    {
        var actorRef = context.Child(name);
        return actorRef.IsNobody() ? Option<IActorRef>.None : Option<IActorRef>.Create(actorRef);
    }

    public static bool ChildTellTraced(this IActorContext context, string name, IWithTracing message)
    {
        message.AddTracing();
        return context.ChildTell(name, message);
    }
    
    public static bool ChildTell(this IActorContext context, string name, object message)
    {
        return context
            .GetChild(name)
            .Match(a =>
            {
                a.Tell(message);
                return true;
            }, () => false);
    }

    public static bool ChildForwardTraced(this IActorContext context, string name, IWithTracing message)
    {
        message.AddTracing();
        return context.ChildForward(name, message);
    }

    public static bool ChildForward(this IActorContext context, string name, object message)
    {
        return context
            .GetChild(name)
            .Match(a =>
            {
                a.Forward(message);
                return true;
            }, () => false);
    }
}