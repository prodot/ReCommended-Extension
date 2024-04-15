using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithPureExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithPure>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithPure";

    [Test]
    public void TestExecuteMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteMethod_MustUseReturnValue() => DoNamedTest2();

    [Test]
    public void TestExecuteMethod_MustDisposeResource() => DoNamedTest2();
}