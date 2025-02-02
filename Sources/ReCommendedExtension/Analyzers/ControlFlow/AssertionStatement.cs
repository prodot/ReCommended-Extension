using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ControlFlow;

public sealed record AssertionStatement : Assertion
{
    [Pure]
    internal static AssertionStatement? TryFromInvocationExpression(
        IInvocationExpression invocationExpression,
        AssertionMethodAnnotationProvider assertionMethodAnnotationProvider,
        AssertionConditionAnnotationProvider assertionConditionAnnotationProvider)
    {
        if (invocationExpression.Reference.Resolve().DeclaredElement is IMethod method && assertionMethodAnnotationProvider.GetInfo(method))
        {
            foreach (var parameter in method.Parameters)
            {
                var parameterAssertionCondition = assertionConditionAnnotationProvider.GetInfo(parameter);

                if (parameterAssertionCondition == null && parameter.Type.IsBool())
                {
                    parameterAssertionCondition = AssertionConditionType.IS_TRUE;
                }

                if (parameterAssertionCondition is { } assertionCondition
                    && invocationExpression.ArgumentList.Arguments.FirstOrDefault(a => a.MatchingParameter?.Element.ShortName == parameter.ShortName)
                        is { Value: { } argumentValue })
                {
                    return new AssertionStatement(assertionCondition) { Statement = invocationExpression, Expression = argumentValue };
                }
            }
        }

        return null;
    }

    AssertionStatement(AssertionConditionType assertionConditionType) => AssertionConditionType = assertionConditionType;

    public required IInvocationExpression Statement { get; init; }

    internal override AssertionConditionType AssertionConditionType { get; }

    public required ICSharpExpression Expression { get; init; }

    public bool Equals([NotNullWhen(true)] AssertionStatement? other)
        => other is { } && Statement.GetDocumentRange() == other.Statement.GetDocumentRange();

    public override int GetHashCode() => Statement.GetDocumentRange().GetHashCode();
}