using System.Text;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Collection;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use target-typed collection expressions" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseTargetTypedCollectionExpressionSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseTargetTypedCollectionExpression"; // a collection expression is always target-typed (needed a distinguished id)

    internal string? OtherTypeNameHint { get; init; }

    public required ICSharpExpression Expression { get; init; }

    internal ICSharpExpression? SpreadItem { get; init; }

    internal TreeNodeCollection<IInitializerElement>? Items { get; init; }

    internal IReferenceExpression? MethodReferenceToSetInferredTypeArguments { get; init; }

    public override DocumentRange CalculateRange()
    {
        if (Expression is ICreationExpression { NewKeyword : { } } creationExpression)
        {
            if (Expression is IArrayCreationExpression arrayCreationExpression)
            {
                if (arrayCreationExpression.Dims is [{ } rankSpecifier])
                {
                    return new DocumentRange(creationExpression.NewKeyword.GetDocumentStartOffset(), rankSpecifier.GetDocumentEndOffset());
                }

                if (arrayCreationExpression.RBracket is { NodeType: TokenNodeType { TokenRepresentation: "]" } })
                {
                    return new DocumentRange(
                        creationExpression.NewKeyword.GetDocumentStartOffset(),
                        arrayCreationExpression.RBracket.GetDocumentEndOffset());
                }
            }

            return creationExpression.NewKeyword.GetDocumentRange();
        }

        return Expression.GetDocumentRange();
    }

    [QuickFix]
    public sealed class Fix(UseTargetTypedCollectionExpressionSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append("Replace with '[");

                if (highlighting is { SpreadItem: { } } or { Items: [_, ..] })
                {
                    builder.Append("...");
                }

                builder.Append("]'");

                if (highlighting.OtherTypeNameHint is { })
                {
                    builder.Append(" (");
                    builder.Append(highlighting.OtherTypeNameHint);
                    builder.Append(')');
                }

                return builder.ToString();
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

                if (highlighting.MethodReferenceToSetInferredTypeArguments is { })
                {
                    // set inferred type arguments

                    var (declaredElement, substitution) = highlighting.MethodReferenceToSetInferredTypeArguments.Reference.Resolve().Result;

                    if (declaredElement is ITypeParametersOwner typeParametersOwner)
                    {
                        highlighting.MethodReferenceToSetInferredTypeArguments.SetTypeArguments(
                            [.. from typeParameter in typeParametersOwner.TypeParameters select substitution[typeParameter]]);
                    }
                }

                var items = from item in highlighting.Items ?? TreeNodeCollection<IInitializerElement>.Empty select item.GetText();

                if (highlighting.SpreadItem is { })
                {
                    items = items.Prepend($"..{highlighting.SpreadItem.GetText()}");
                }

                ModificationUtil.ReplaceChild(highlighting.Expression, factory.CreateExpression($"[{string.Join(", ", items)}]"));
            }

            return null;
        }
    }
}