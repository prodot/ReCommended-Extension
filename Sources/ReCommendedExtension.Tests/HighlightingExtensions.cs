using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReCommendedExtension.Analyzers;

namespace ReCommendedExtension.Tests;

internal static class HighlightingExtensions
{
    sealed record HighlightingInfo
    {
        public required bool IsReSharperHighlighting { get; init; }

        public required Severity Severity { get; init; }

        public required string Message { get; init; }
    }

    public sealed class HighlightingStatistics
    {
        readonly List<HighlightingInfo> highlightingInfos = [];

        public bool TryAdd(IHighlighting highlighting)
        {
            if (highlighting.Info is { } info)
            {
                lock (highlightingInfos)
                {
                    highlightingInfos.Add(info);
                }

                return true;
            }

            return false;
        }

        public string GetSummary()
        {
            Dictionary<bool, Dictionary<Severity, List<HighlightingInfo>>> allIssues;

            lock (highlightingInfos)
            {
                allIssues = (from item in highlightingInfos group item by item.IsReSharperHighlighting).ToDictionary(
                    g => g.Key,
                    g => (from item in g group item by item.Severity).ToDictionary(sg => sg.Key, sg => sg.ToList()));
            }

            var builder = new StringBuilder();

            void AddToBuilder(string caption, Dictionary<Severity, List<HighlightingInfo>>? issues)
            {
                if (issues is { })
                {
                    const string indentation = "   ";

                    builder.AppendLine($"{caption}:");

                    var issuesOrdered = from p in issues orderby p.Key descending select (severity: p.Key, issues: p.Value);

                    foreach (var (severity, severityIssues) in issuesOrdered)
                    {
                        builder.AppendLine($"{indentation}{severity} [{severityIssues.Count}]:");

                        var messageCounts =
                            from issue in severityIssues group issue by issue.Message into g orderby g.Key select (message: g.Key, count: g.Count());

                        foreach (var (message, count) in messageCounts)
                        {
                            builder.AppendLine($"{indentation}{indentation}{message} [{count}]");
                        }
                    }

                    builder.AppendLine();
                }
            }

            AddToBuilder("Issues", allIssues.GetValueOrDefault(false));
            AddToBuilder("R# issues", allIssues.GetValueOrDefault(true));

            return builder.ToString();
        }
    }

    [Conditional("DEBUG")]
    [AssertionMethod]
    static void Assert(
        [AssertionCondition(AssertionConditionType.IS_TRUE)][DoesNotReturnIf(false)] bool condition,
        [CallerArgumentExpression(nameof(condition))] string message = "")
        => NUnit.Framework.Assert.IsTrue(condition, message);

    extension(IHighlighting highlighting)
    {
        HighlightingInfo? Info
        {
            get
            {
                var type = highlighting.GetType();

                if (highlighting is Highlighting && type.GetCustomAttribute<RegisterConfigurableSeverityAttribute>() is { } attribute)
                {
                    Assert(highlighting.ToolTip is [.., '.']);

                    return new HighlightingInfo
                    {
                        IsReSharperHighlighting = false, Severity = attribute.DefaultSeverity, Message = highlighting.ToolTip,
                    };
                }

                Assert(type.FullName is { });

                if (!type.FullName.StartsWith(nameof(ReCommendedExtension), StringComparison.OrdinalIgnoreCase))
                {
                    if (type.GetCustomAttribute<StaticSeverityHighlightingAttribute>() is { } severityHighlightingAttribute)
                    {
                        return new HighlightingInfo
                        {
                            IsReSharperHighlighting = true,
                            Severity = severityHighlightingAttribute.Severity,
                            Message = $"{highlighting.ToolTip ?? highlighting.ErrorStripeToolTip ?? "<no message>"} ({type.Name})",
                        };
                    }

                    if (type.GetCustomAttribute<RegisterConfigurableSeverityAttribute>() is { } registerConfigurableSeverityAttribute)
                    {
                        return new HighlightingInfo
                        {
                            IsReSharperHighlighting = true,
                            Severity = registerConfigurableSeverityAttribute.DefaultSeverity,
                            Message = $"{highlighting.ToolTip ?? highlighting.ErrorStripeToolTip ?? "<no message>"} ({type.Name})",
                        };
                    }

                    if (type.GetCustomAttribute<ConfigurableSeverityHighlightingAttribute>() is { })
                    {
                        return new HighlightingInfo
                        {
                            IsReSharperHighlighting = true,
                            Severity = Severity.INFO,
                            Message = $"{highlighting.ToolTip ?? highlighting.ErrorStripeToolTip ?? "<no message>"} ({type.Name})",
                        };
                    }

                    return new HighlightingInfo
                    {
                        IsReSharperHighlighting = true,
                        Severity = Severity.INVALID_SEVERITY, // unknown
                        Message = $"{highlighting.ToolTip ?? highlighting.ErrorStripeToolTip ?? "<no message>"} ({type.Name})",
                    };
                }

                return null;
            }
        }

        public bool IsError => highlighting.GetType().GetCustomAttribute<StaticSeverityHighlightingAttribute>() is { Severity: Severity.ERROR };
    }
}