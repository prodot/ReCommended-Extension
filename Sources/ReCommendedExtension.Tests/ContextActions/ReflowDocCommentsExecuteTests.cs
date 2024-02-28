using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Xml.CodeStyle;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class ReflowDocCommentsExecuteTests : CSharpContextActionExecuteTestBase<ReflowDocComments>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ReflowDocComments";

    static void SetValue<T>(IContextBoundSettingsStore store, Expression<Func<XmlDocFormatterSettingsKey, T>> lambdaExpression, T value)
        => store.SetValue(lambdaExpression, value);

    void DoNamedTestWithSettings()
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

    [Test]
    public void TestExecute_WrapTopLevelTags() => DoNamedTestWithSettings();

    [Test]
    public void TestExecute_WrapNestedTags() => DoNamedTestWithSettings();

    [Test]
    public void TestExecute_WrapNestedTags_Para() => DoNamedTestWithSettings();

    [Test]
    public void TestExecute_ReorderTopLevelTags() => DoNamedTestWithSettings();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestExecute_Case_1() => DoNamedTestWithSettings();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestExecute_Case_2() => DoNamedTestWithSettings();
}