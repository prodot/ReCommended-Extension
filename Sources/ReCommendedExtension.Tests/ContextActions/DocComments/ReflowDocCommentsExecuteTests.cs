using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class ReflowDocCommentsExecuteTests : DocCommentsExecuteTests<ReflowDocComments>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ReflowDocComments";

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

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestExecute_Case_3() => DoNamedTestWithSettings();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestExecute_Case_4() => DoNamedTestWithSettings();
}