using System.Reflection;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ReCommendedExtension.Tests;

internal static class HighlightingExtensions
{
    extension(IHighlighting highlighting)
    {
        public bool IsError => highlighting.GetType().GetCustomAttribute<StaticSeverityHighlightingAttribute>() is { Severity: Severity.ERROR };
    }
}