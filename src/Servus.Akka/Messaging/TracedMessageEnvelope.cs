using Servus.Core.Diagnostics;

namespace Servus.Akka.Messaging;

public sealed record TracedMessageEnvelope(object Message) : IWithTracing, IMessageEnvelope
{
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
}