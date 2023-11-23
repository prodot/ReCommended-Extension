using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

public abstract record ImplementEqualityOperatorsSuggestion : Highlighting
{
    readonly IClassLikeDeclaration declaration;

    private protected ImplementEqualityOperatorsSuggestion(string message, IClassLikeDeclaration declaration) : base(message)
        => this.declaration = declaration;

    public sealed override DocumentRange CalculateRange() => declaration.NameIdentifier.GetDocumentRange();
}