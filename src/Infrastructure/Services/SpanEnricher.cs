using Application.Services.SpanEnricher;
using System.Diagnostics;

namespace Infrastructure.Services;

internal sealed class SpanEnricher : ISpanEnricher
{
    public void EnrichWithException(Exception exception) =>
        Activity.Current?.AddException(exception);
}
