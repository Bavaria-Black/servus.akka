using Akka.Hosting;
using Akka.Hosting.TestKit;
using Servus.Akka.Diagnostics;
using Servus.Akka.Messaging;

namespace Servus.Akka.Tests.Diagnostics;

public class TracedAskExtensionsTests : TestKit
{
    private class TestTracedActor : TracedMessageActor
    {
        public TestTracedActor()
        {
            Receive<SimpleTracedMessage>(msg => msg.Asked, Reply);
            Receive<SimpleTracedMessage>(msg => ReplyTraced(new SimpleTracedMessage("reply")));
            Receive<string>(msg => ReplyTraced(new TracedMessageEnvelope("reply")));
        }
    }

    protected override void ConfigureAkka(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithResolvableActor<TestTracedActor>();
    }
    
    [Fact]
    public async Task AskTracedTest()
    {
        var actor = Sys.ResolveActor<TestTracedActor>();
        var msg = new SimpleTracedMessage("message", Asked: true);

        var result = await actor.AskTraced<SimpleTracedMessage>(msg);

        Assert.Equal("message", result.Message);
        Assert.NotNull(result.TraceId);
        Assert.NotNull(result.SpanId);
        Assert.Equal(msg.SpanId, result.SpanId);
    }

    [Fact]
    public async Task AskTracedTimeoutTest()
    {
        var actor = Sys.ResolveActor<TestTracedActor>();
        var msg = new SimpleTracedMessage("message", Asked: true);

        var result = await actor.AskTraced<SimpleTracedMessage>(msg, TimeSpan.FromSeconds(5));

        Assert.Equal("message", result.Message);
        Assert.NotNull(result.TraceId);
        Assert.NotNull(result.SpanId);
    }

    [Fact]
    public async Task AskTracedCancellationTokenTest()
    {
        var actor = Sys.ResolveActor<TestTracedActor>();
        var msg = new SimpleTracedMessage("message");
        using var cts = new CancellationTokenSource();

        var result = await actor.AskTraced<SimpleTracedMessage>(msg, cts.Token);

        Assert.Equal("reply", result.Message);
        Assert.NotNull(result.TraceId);
        Assert.NotNull(result.SpanId);
    }

    [Fact]
    public async Task AskTracedObjectTest()
    {
        var actor = Sys.ResolveActor<TestTracedActor>();
        const string msg = "pure message";
        var result = await actor.AskTraced<TracedMessageEnvelope>(msg);

        Assert.Equal("reply", result.Message);
        Assert.NotNull(result.TraceId);
    }
}