using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80(ANNOTATIONS_PACKAGE)]
public sealed class AnnotateWithMustDisposeResourceFalseExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithMustDisposeResourceFalse>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustDisposeResourceFalse";

    [Test]
    public void ExecuteType() => DoNamedTest();

    [Test]
    public void ExecuteConstructor() => DoNamedTest();

    [Test]
    public void ExecutePrimaryConstructor() => DoNamedTest();

    [Test]
    public void ExecuteMethod() => DoNamedTest();

    [Test]
    public void ExecuteMethod_MustUseReturnValue() => DoNamedTest();

    [Test]
    public void ExecuteMethod_Pure() => DoNamedTest();

    [Test]
    public void ExecuteMethod_MustDisposeResource() => DoNamedTest();

    [Test]
    public void ExecuteMethod_Multiple() => DoNamedTest();

    [Test]
    public void ExecuteParameter() => DoNamedTest();
}