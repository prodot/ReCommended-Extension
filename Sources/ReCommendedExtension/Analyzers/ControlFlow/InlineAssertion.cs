using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ControlFlow;

public sealed record InlineAssertion : Assertion
{
    [Pure]
    internal static InlineAssertion? TryFromInvocationExpression(IInvocationExpression invocationExpression)
    {
        if (invocationExpression.InvokedExpression is IReferenceExpression { QualifierExpression: { } qualifierExpression } referenceExpression
            && referenceExpression.Reference.Resolve().DeclaredElement is IMethod { IsExtensionMethod: true, Parameters: [var parameter] } method
            && method.ShortName.StartsWith("AssertNotNull", StringComparison.Ordinal)
            && parameter.Type.Equals(method.ReturnType)
            && method.GetSingleDeclaration<IMethodDeclaration>() is { } methodDeclaration
            && methodDeclaration.Attributes.Any(
                attribute => attribute.GetAttributeType().IsClrType(PredefinedType.DEBUGGER_STEP_THROUGH_ATTRIBUTE_CLASS))
            && methodDeclaration.Attributes.Any(
                attribute => attribute.GetAttributeType().GetClrName().ShortName == NullnessProvider.NotNullAttributeShortName))
        {
            return new InlineAssertion
            {
                InvocationExpression = invocationExpression, QualifierExpression = qualifierExpression, MethodName = method.ShortName,
            };
        }

        return null;
    }

    internal override AssertionConditionType AssertionConditionType => AssertionConditionType.IS_NOT_NULL;

    /// <remarks>
    /// The complete expression including the qualifier (part before the "AssertNotNull") and the "AssertNotNull" invocation.
    /// </remarks>
    public required IInvocationExpression InvocationExpression { get; init; }

    /// <remarks>
    /// The expression part before the "AssertNotNull" method.
    /// </remarks>
    public required ICSharpExpression QualifierExpression { get; init; }

    /// <remarks>
    /// The exact "AssertNotNull" method name.
    /// </remarks>
    public required string MethodName { get; init; }

    public bool Equals([NotNullWhen(true)] InlineAssertion? other)
        => other is { } && InvocationExpression.GetDocumentRange() == other.InvocationExpression.GetDocumentRange();

    public override int GetHashCode() => InvocationExpression.GetDocumentRange().GetHashCode();
}