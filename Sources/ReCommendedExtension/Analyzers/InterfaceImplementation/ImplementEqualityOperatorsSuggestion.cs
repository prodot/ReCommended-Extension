using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

public abstract record ImplementEqualityOperatorsSuggestion : Highlighting
{
    private protected ImplementEqualityOperatorsSuggestion(string message, IClassLikeDeclaration declaration) : base(message)
        => Declaration = declaration;

    internal IClassLikeDeclaration Declaration { get; }

    public sealed override DocumentRange CalculateRange() => Declaration.NameIdentifier.GetDocumentRange();
}