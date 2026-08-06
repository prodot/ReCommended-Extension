using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
public sealed class AnnotateWithDefaultEqualityUsageExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithDefaultEqualityUsage>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithDefaultEqualityUsage";

    [Test]
    public void ExecuteClassTypeParameter() => DoNamedTest();

    [Test]
    public void ExecuteMethodParameter() => DoNamedTest();

    [Test]
    public void ExecuteMethodReturnValue() => DoNamedTest();

    [Test]
    public void ExecuteMethodTypeParameter() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void ExecutePositionalRecordParameter() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void ExecutePrimaryConstructorParameter() => DoNamedTest();
}