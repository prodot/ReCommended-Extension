using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.CodeStyle;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

public abstract class DocCommentsExecuteTests<A, N> : CSharpContextActionExecuteTestBase<A>
    where A : XmlDocCommentContextAction<N> where N : class, ITreeNode
{
    static void SetValue<T>(IContextBoundSettingsStore store, Expression<Func<XmlDocFormatterSettingsKey, T>> lambdaExpression, T value)
        where T : notnull
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