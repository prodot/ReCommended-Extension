using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;

namespace ReCommendedExtension.Analyzers.NullCoalescingAssignment
{
    [ElementProblemAnalyzer(typeof(IIfStatement), HighlightingTypes = new[] { typeof(NullCoalescingAssignmentSuggestion) })]
    public sealed class NullCoalescingAssignmentAnalyzer : ElementProblemAnalyzer<IIfStatement>
    {
        [Pure]
        [ContractAnnotation("null => null", true)]
        static IReferenceExpressionReference TryGetReferenceIfComparedWithNull(ICSharpExpression condition)
        {
            switch (condition)
            {
                case IEqualityExpression equalityExpression when equalityExpression.EqualityType == EqualityExpressionType.EQEQ:
                    if (equalityExpression.LeftOperand is IReferenceExpression leftReferenceExpression &&
                        equalityExpression.RightOperand is ICSharpLiteralExpression rightLiteralExpression &&
                        rightLiteralExpression.IsNullLiteral())
                    {
                        return leftReferenceExpression.Reference;
                    }

                    if (equalityExpression.LeftOperand is ICSharpLiteralExpression leftLiteralExpression &&
                        leftLiteralExpression.IsNullLiteral() &&
                        equalityExpression.RightOperand is IReferenceExpression rightReferenceExpression)
                    {
                        return rightReferenceExpression.Reference;
                    }
                    break;

                case IIsExpression isExpression:
                    if (isExpression.Operand is IReferenceExpression referenceExpression &&
                        isExpression.Pattern is IConstantPattern constantPattern &&
                        constantPattern.ConstantValue.IsNull())
                    {
                        return referenceExpression.Reference;
                    }
                    break;
            }

            return null;
        }

        [Pure]
        [ContractAnnotation(
            "then:null => assignmentDestination:null, assignedExpression:null; assignmentDestination:notnull => assignedExpression:notnull",
            true)]
        static void TryGetSingleAssignment(
            ICSharpStatement then,
            out IReferenceExpression assignmentDestination,
            out ICSharpExpression assignedExpression)
        {
            IExpressionStatement expressionStatement;
            switch (then)
            {
                case IBlock block when block.Statements.Count == 1 && block.Statements[0] is IExpressionStatement blockExpressionStatement:
                    expressionStatement = blockExpressionStatement;
                    break;

                case IExpressionStatement directExpressionStatement:
                    expressionStatement = directExpressionStatement;
                    break;

                default:
                    expressionStatement = null;
                    break;
            }

            if (expressionStatement?.Expression is IAssignmentExpression assignmentExpression &&
                assignmentExpression.AssignmentType == AssignmentType.EQ &&
                assignmentExpression.Dest is IReferenceExpression referenceExpression)
            {
                assignmentDestination = referenceExpression;
                assignedExpression = assignmentExpression.Source;
            }
            else
            {
                assignmentDestination = null;
                assignedExpression = null;
            }
        }

        protected override void Run(IIfStatement element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (!element.IsCSharp8Supported())
            {
                return;
            }

            if (element.Else != null)
            {
                return;
            }

            var reference = TryGetReferenceIfComparedWithNull(element.Condition);
            if (reference == null)
            {
                return;
            }

            TryGetSingleAssignment(element.Then, out var assignmentDestination, out var assignedExpression);
            if (assignmentDestination == null || assignedExpression == null)
            {
                return;
            }

            var assignmentDestinationElement = assignmentDestination.Reference.Resolve().DeclaredElement;
            var referenceElement = reference.Resolve().DeclaredElement;

            if (Equals(assignmentDestinationElement, referenceElement))
            {
                consumer.AddHighlighting(
                    new NullCoalescingAssignmentSuggestion(
                        "Replace 'if' statement with compound assignment.",
                        element,
                        assignmentDestination,
                        assignedExpression));
            }
        }
    }
}