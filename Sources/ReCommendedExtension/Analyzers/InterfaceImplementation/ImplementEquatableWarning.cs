using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IEquatable<T> when overriding Equals" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementEquatableWarning : ImplementOperatorsHighlighting
{
    const string SeverityId = "ImplementEquatable";

    internal ImplementEquatableWarning(string message, IStructDeclaration declaration) : base(message, declaration) { }
}