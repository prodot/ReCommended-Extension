﻿using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.DelegateInvoke;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    $"Redundant '{nameof(Action.Invoke)}' expression" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantDelegateInvokeHint(string message, IReferenceExpression referenceExpression) : Highlighting(message)
{
    const string SeverityId = "RedundantDelegateInvoke";

    internal IReferenceExpression ReferenceExpression { get; } = referenceExpression;

    public override DocumentRange CalculateRange()
    {
        var dotToken = ReferenceExpression.NameIdentifier.GetPreviousMeaningfulToken();

        return ReferenceExpression.NameIdentifier.GetDocumentRange().JoinLeft(dotToken.GetDocumentRange());
    }
}