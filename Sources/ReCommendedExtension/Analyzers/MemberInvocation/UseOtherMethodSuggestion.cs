using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.BestPractice, "Use another method" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherMethodSuggestion(
    string message,
    ICSharpExpression qualifier,
    ReplacedMethodInvocation replacedMethodInvocation) : MultipleHighlightings(message)
{
    const string SeverityId = "UseOtherMethod";

    internal ICSharpExpression Qualifier => qualifier;

    internal ReplacedMethodInvocation ReplacedMethodInvocation => replacedMethodInvocation;
}