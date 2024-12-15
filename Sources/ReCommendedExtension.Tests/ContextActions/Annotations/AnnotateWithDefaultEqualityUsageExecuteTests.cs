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
    public void TestExecuteClassTypeParameter() => DoNamedTest2();

    [Test]
    public void TestExecuteMethodParameter() => DoNamedTest2();

    [Test]
    public void TestExecuteMethodReturnValue() => DoNamedTest2();

    [Test]
    public void TestExecuteMethodTypeParameter() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestExecutePositionalRecordParameter() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecutePrimaryConstructorParameter() => DoNamedTest2();
}