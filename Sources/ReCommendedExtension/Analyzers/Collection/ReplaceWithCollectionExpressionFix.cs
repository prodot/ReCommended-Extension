using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Utils;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Collection;

[QuickFix]
public sealed class ReplaceWithCollectionExpressionFix(UseTargetTypedCollectionExpressionSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '[{(highlighting.Expression.ArrayInitializer is { InitializerElements: [_, ..] } ? "..." : "")}]'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

            if (highlighting.MethodReferenceToSetInferredTypeArguments is { })
            {
                // set inferred type arguments

                var (declaredElement, substitution) = highlighting.MethodReferenceToSetInferredTypeArguments.Reference.Resolve().Result;
                Debug.Assert(declaredElement is ITypeParametersOwner);

                var typeArguments = (from typeParameter in ((ITypeParametersOwner)declaredElement).TypeParameters select substitution[typeParameter])
                    .ToList();

                highlighting.MethodReferenceToSetInferredTypeArguments.SetTypeArguments(typeArguments);
            }

            ModificationUtil.ReplaceChild(
                highlighting.Expression,
                highlighting.Expression.ArrayInitializer is { InitializerElements: [_, ..] items }
                    ? factory.CreateExpression($"[{string.Join(", ", from item in items select item.GetText())}]")
                    : factory.CreateExpression("[]"));
        }

        return _ => { };
    }
}