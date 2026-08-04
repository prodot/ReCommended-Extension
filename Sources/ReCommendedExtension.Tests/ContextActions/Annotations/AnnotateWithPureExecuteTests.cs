using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80(ANNOTATIONS_PACKAGE)]
public sealed class AnnotateWithPureExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithPure>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithPure";

    [Test]
    public void ExecuteMethod() => DoNamedTest();

    [Test]
    public void ExecuteMethod_MustUseReturnValue() => DoNamedTest();

    [Test]
    public void ExecuteMethod_MustDisposeResource() => DoNamedTest();
}