using Akka.Actor;
using Akka.Util;
using Servus.Akka.Diagnostics;

namespace Servus.Akka.Tests.Diagnostics;

public class TracedActorMessageExtensionsTest
{
    [Fact]
    public void TellTracedTest()
    {
        var a = new TestActorRef();
        var msg = new SimpleTracedMessage("message");

        Assert.Null(msg.TraceId);
        Assert.Null(msg.SpanId);

        a.TellTraced(msg);

        Assert.Equal(msg, a.LatestMessage);
        Assert.NotNull(msg.TraceId);
        Assert.NotNull(msg.SpanId);
    }

    [Fact]
    public void ForwardTracedTest()
    {
        var a = new TestActorRef();
        var msg = new SimpleTracedMessage("message");

        Assert.Null(msg.TraceId);
        Assert.Null(msg.SpanId);

        a.ForwardTraced(msg);

        Assert.Equal(msg, a.LatestMessage);
        Assert.NotNull(msg.TraceId);
        Assert.NotNull(msg.SpanId);
    }
}

public class TestTeller : ICanTell
{
    public object? LatestMessage { get; private set; }

    public void Tell(object message, IActorRef sender)
    {
        LatestMessage = message;
    }
}

public sealed class TestActorRef : TestTeller, IActorRef
{

    public bool Equals(IActorRef? other)
    {
        return other?.Equals(this) ?? false;
    }

    public ISurrogate ToSurrogate(ActorSystem system)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(IActorRef? other)
    {
        return other?.CompareTo(this) ?? 1;
    }

    public int CompareTo(object? obj)
    {
        throw new NotImplementedException();
    }

    public ActorPath Path { get; } = ActorPath.Parse("akka://test/user/blub");
}