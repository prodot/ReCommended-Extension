using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.InternalConstructor;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Make internal constructor in abstract class protected or private protected" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class InternalConstructorVisibilitySuggestion(
    string message,
    ITokenNode modifierTokenNode,
    IConstructorDeclaration constructorDeclaration,
    AccessRights visibility) : Highlighting(message)
{
    const string SeverityId = "InternalConstructorVisibility";

    internal IConstructorDeclaration ConstructorDeclaration { get; } = constructorDeclaration;

    internal AccessRights Visibility { get; } = visibility;

    public override DocumentRange CalculateRange() => modifierTokenNode.GetDocumentRange();
}