﻿using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Linq;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.LanguageUsage, "Use list pattern" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseLinqListPatternSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    ListPatternSuggestionKind kind,
    string? defaultValueExpression = null) : LinqHighlighting(message, invocationExpression, invokedExpression)
{
    const string SeverityId = "UseLinqListPattern";

    internal ListPatternSuggestionKind Kind => kind;

    internal string? DefaultValueExpression => defaultValueExpression;
}