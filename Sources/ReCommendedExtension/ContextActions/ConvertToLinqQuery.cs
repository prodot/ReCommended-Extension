using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = $"Converts '{nameof(IEnumerable<int>)}<T>' to a no-op LINQ query" + ZoneMarker.Suffix,
    Description = $"Converts an '{nameof(IEnumerable<int>)}<T>' to a no-op LINQ query.")]
public sealed class ConvertToLinqQuery(ICSharpContextActionDataProvider provider) : ContextAction<ICSharpExpression>(provider)
{
    class Replacement
    {
        string? targetExpression;

        [Pure]
        protected virtual string CreateTargetExpression() => $"from item in {SourceExpression.GetText()} select item";

        public required ICSharpExpression SourceExpression { get; init; }

        public string TargetExpression
        {
            get
            {
                targetExpression ??= CreateTargetExpression();

                return targetExpression;
            }
        }
    }

    sealed class CollectionExpressionReplacement : Replacement
    {
        protected override string CreateTargetExpression() => $"[..{base.CreateTargetExpression()}]";
    }

    sealed class LinqMethodCallReplacement : Replacement
    {
        protected override string CreateTargetExpression()
        {
            string argument;

            if (ExpressionPropertyAsMethodArgument is { } propertyName)
            {
                var factory = CSharpElementFactory.GetInstance(SourceExpression);

                var argumentExpression = factory.CreateExpression($"($0).{propertyName}", SourceExpression);

                if (argumentExpression is IReferenceExpression { QualifierExpression: IParenthesizedExpression qualifierExpression }
                    && qualifierExpression.AreParenthesesRedundant())
                {
                    argumentExpression = factory.CreateExpression($"$0.{propertyName}", SourceExpression);
                }

                argument = argumentExpression.GetText();
            }
            else
            {
                argument = "";
            }

            return $"({base.CreateTargetExpression()}).{MethodName}({argument})";
        }

        public required string MethodName { get; init; }

        public string? ExpressionPropertyAsMethodArgument { get; init; }
    }

    Replacement? replacement;

    [MemberNotNullWhen(true, nameof(replacement))]
    protected override bool IsAvailable(ICSharpExpression selectedElement)
    {
        if (selectedElement.Parent is IQueryFirstFrom { Expression: var expression } && expression == selectedElement)
        {
            return false;
        }

        if (selectedElement.IsNotNullHere(NullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            var type = selectedElement.Type();

            if (type.IsGenericIEnumerable()
                || selectedElement.Parent is ISpreadElement
                || selectedElement.TryGetTargetType(false) is { } targetType
                && targetType.IsGenericIEnumerable()
                && (type.IsGenericIEnumerableOrDescendant() || type.IsGenericArray())
                || type.IsGenericListOrDescendant()
                && selectedElement.Parent is IReferenceExpression referenceExpression
                && referenceExpression.QualifierExpression == selectedElement
                && referenceExpression.Reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    IsStatic: false,
                    TypeParameters: [],
                    Parameters: [],
                    ShortName: nameof(List<int>.ToArray),
                })
            {
                replacement = new Replacement { SourceExpression = selectedElement };
                return true;
            }

            if (selectedElement.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120
                && selectedElement.TryGetTargetType(true) is { } targetTypeForCollectionExpressions
                && (targetTypeForCollectionExpressions.IsGenericIReadOnlyCollection()
                    || targetTypeForCollectionExpressions.IsGenericIReadOnlyList()
                    || targetTypeForCollectionExpressions.IsGenericICollection()
                    || targetTypeForCollectionExpressions.IsGenericIList()
                    || targetTypeForCollectionExpressions.IsGenericList()
                    || targetTypeForCollectionExpressions.IsGenericArray())
                && (type.IsGenericIEnumerableOrDescendant() || type.IsGenericArray()))
            {
                replacement = new CollectionExpressionReplacement { SourceExpression = selectedElement };
                return true;
            }

            if (type.IsGenericList())
            {
                replacement = new LinqMethodCallReplacement { SourceExpression = selectedElement, MethodName = nameof(Enumerable.ToList) };
                return true;
            }

            if (type.IsGenericArray())
            {
                replacement = new LinqMethodCallReplacement { SourceExpression = selectedElement, MethodName = nameof(Enumerable.ToArray) };
                return true;
            }

            if (type.IsGenericHashSet())
            {
                replacement = new LinqMethodCallReplacement
                {
                    SourceExpression = selectedElement,
                    MethodName = nameof(Enumerable.ToHashSet),
                    ExpressionPropertyAsMethodArgument = nameof(HashSet<int>.Comparer),
                };
                return true;
            }
        }

        return false;
    }

    public override string Text
    {
        get
        {
            Debug.Assert(replacement is { });

            return $"To LINQ query '{replacement.TargetExpression.TrimToSingleLineWithMaxLength(120)}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(replacement is { });

            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(replacement.SourceExpression);

                ModificationUtil
                    .ReplaceChild(replacement.SourceExpression, factory.CreateExpression($"({replacement.TargetExpression})"))
                    .TryRemoveParentheses(factory);
            }

            return _ => { };
        }
        finally
        {
            replacement = null;
        }
    }
}