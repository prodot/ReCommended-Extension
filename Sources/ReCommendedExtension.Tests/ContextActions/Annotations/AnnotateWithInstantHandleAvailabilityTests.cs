using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
public sealed class AnnotateWithInstantHandleAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithInstantHandle>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

    [Test]
    public void TestAvailabilityCS80() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestAvailabilityCS90() => DoNamedTest2();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityAsyncEnumerableCS80() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityAsyncEnumerableCS90() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityLambdaCS90() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityLambdaCS100() => DoNamedTest2();
}