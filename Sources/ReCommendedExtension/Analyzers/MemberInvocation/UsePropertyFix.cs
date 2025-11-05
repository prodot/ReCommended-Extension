using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[QuickFix]
public sealed class UsePropertyFix(UsePropertySuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.PropertyName}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

            var castType = highlighting.EnsureTargetType is { } targetType
                ? highlighting.InvocationExpression.TryGetTargetType(false) is var type && highlighting.InvokedExpression.HasConditionalAccessSign
                    ? targetType.IsNullableType(type) ? null : targetType.Name
                    : targetType.IsType(type) || targetType.IsNullableType(type)
                        ? null
                        : targetType.Name
                : null;

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                castType is { }
                    ? factory.CreateExpression(
                        $"({castType}{conditionalAccess})$0{conditionalAccess}.{highlighting.PropertyName}",
                        highlighting.InvokedExpression.QualifierExpression)
                    : factory.CreateExpression(
                        $"$0{conditionalAccess}.{highlighting.PropertyName}",
                        highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}