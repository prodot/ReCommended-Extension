using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNetCore30(ANNOTATIONS_PACKAGE)]
public sealed class AnnotateWithNonNegativeValueExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithNonNegativeValue>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithNonNegativeValue";

    [Test]
    public void TestExecute() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60(ANNOTATIONS_PACKAGE)]
    public void TestExecuteLambda() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60(ANNOTATIONS_PACKAGE)]
    public void TestExecuteLambda2() => DoNamedTest2();
}