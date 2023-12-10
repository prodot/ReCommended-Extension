using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

public abstract class ImplementOperatorsHighlighting(string message, IClassLikeDeclaration declaration) : Highlighting(message)
{
    public sealed override DocumentRange CalculateRange() => declaration.NameIdentifier.GetDocumentRange();
}