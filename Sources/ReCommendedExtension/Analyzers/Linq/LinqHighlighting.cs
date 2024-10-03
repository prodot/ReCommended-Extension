using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Linq;

public abstract class LinqHighlighting(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression) : Highlighting(message)
{
    internal IInvocationExpression InvocationExpression { get; } = invocationExpression;

    internal IReferenceExpression InvokedExpression { get; } = invokedExpression;

    public sealed override DocumentRange CalculateRange()
        => InvocationExpression.GetDocumentRange().SetStartTo(InvokedExpression.Reference.GetDocumentRange().StartOffset);
}