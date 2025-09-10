using System.Reflection;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ReCommendedExtension.Tests;

internal static class HighlightingExtensions
{
    [Pure]
    public static bool IsError(this IHighlighting highlighting)
        => highlighting.GetType().GetCustomAttribute<StaticSeverityHighlightingAttribute>() is { Severity: Severity.ERROR };
}