using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.Xml.CodeStyle;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

public abstract class DocCommentsExecuteTests<A> : CSharpContextActionExecuteTestBase<A> where A : XmlDocCommentContextAction
{
    static void SetValue<T>(IContextBoundSettingsStore store, Expression<Func<XmlDocFormatterSettingsKey, T>> lambdaExpression, T value)
        => store.SetValue(lambdaExpression, value);

    protected void DoNamedTestWithSettings()
        => ExecuteWithinSettingsTransaction(
            store =>
            {
                RunGuarded(
                    () =>
                    {
                        SetValue(store, s => s.INDENT_SIZE, 4);
                        SetValue(store, s => s.WRAP_LIMIT, 150);
                        SetValue(store, s => s.TagSpacesAroundAttributeEq, false);
                        SetValue(store, s => s.TagSpaceAfterLastAttr, false);
                        SetValue(store, s => s.TagSpaceBeforeHeaderEnd1, false);
                    });

                DoNamedTest2();
            });
}