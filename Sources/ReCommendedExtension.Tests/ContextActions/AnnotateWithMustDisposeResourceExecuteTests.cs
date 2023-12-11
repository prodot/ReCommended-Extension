using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithMustDisposeResourceExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithMustDisposeResource>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustDisposeResource";

    [Test]
    public void TestExecuteType() => DoNamedTest2();

    [Test]
    public void TestExecuteConstructor() => DoNamedTest2();

    [Test]
    public void TestExecuteMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteMethod_MustUseReturnValue() => DoNamedTest2();

    [Test]
    public void TestExecuteMethod_Pure() => DoNamedTest2();

    [Test]
    public void TestExecuteParameter() => DoNamedTest2();
}