﻿using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Servus.Akka;

public static class ContextExtensions
{
    public static Option<IActorRef> GetChild(this IActorContext context, string name)
    {
        var actorRef = context.Child(name);
        return actorRef.IsNobody() ? Option<IActorRef>.None : Option<IActorRef>.Create(actorRef);
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