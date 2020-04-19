using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    internal sealed class NullForgivingOperation : Assertion
    {
        public NullForgivingOperation([NotNull] ISuppressNullableWarningExpression suppressNullableWarningExpression)
            => SuppressNullableWarningExpression = suppressNullableWarningExpression;

        public override AssertionConditionType AssertionConditionType => AssertionConditionType.IS_NOT_NULL;

        [NotNull]
        public ISuppressNullableWarningExpression SuppressNullableWarningExpression { get; }

        public override bool Equals(Assertion other)
            => SuppressNullableWarningExpression.GetDocumentRange() ==
                (other as NullForgivingOperation)?.SuppressNullableWarningExpression.GetDocumentRange();

        public override int GetHashCode() => SuppressNullableWarningExpression.GetDocumentRange().GetHashCode();
    }
}