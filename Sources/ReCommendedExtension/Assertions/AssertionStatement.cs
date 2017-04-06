using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Assertions
{
    internal sealed class AssertionStatement : Assertion
    {
        internal static AssertionStatement TryFromInvocationExpression(
            [NotNull] IInvocationExpression invocationExpression,
            [NotNull] AssertionMethodAnnotationProvider assertionMethodAnnotationProvider,
            [NotNull] AssertionConditionAnnotationProvider assertionConditionAnnotationProvider)
        {
            if (invocationExpression.Reference?.Resolve().DeclaredElement is IMethod method && assertionMethodAnnotationProvider.GetInfo(method))
            {
                foreach (var parameter in method.Parameters)
                {
                    Debug.Assert(parameter != null);

                    var parameterAssertionCondition = assertionConditionAnnotationProvider.GetInfo(parameter);

                    if (parameterAssertionCondition == null && parameter.Type.IsBool())
                    {
                        parameterAssertionCondition = AssertionConditionType.IS_TRUE;
                    }

                    if (parameterAssertionCondition != null)
                    {
                        var argument = invocationExpression.ArgumentList?.ArgumentsEnumerable.FirstOrDefault(
                            a =>
                            {
                                Debug.Assert(a != null);
                                return a.MatchingParameter != null && a.MatchingParameter.Element.ShortName == parameter.ShortName;
                            });
                        if (argument?.Value != null)
                        {
                            return new AssertionStatement(invocationExpression, (AssertionConditionType)parameterAssertionCondition, argument.Value);
                        }
                    }
                }
            }

            return null;
        }

        AssertionStatement(
            [NotNull] IInvocationExpression statement,
            AssertionConditionType assertionConditionType,
            [NotNull] ICSharpExpression expression)
        {
            Statement = statement;
            AssertionConditionType = assertionConditionType;
            Expression = expression;
        }

        [NotNull]
        public IInvocationExpression Statement { get; }

        public override AssertionConditionType AssertionConditionType { get; }

        [NotNull]
        public ICSharpExpression Expression { get; }

        public override bool Equals(Assertion other) => Statement.GetDocumentRange() == (other as AssertionStatement)?.Statement.GetDocumentRange();

        public override int GetHashCode() => Statement.GetDocumentRange().GetHashCode();
    }
}