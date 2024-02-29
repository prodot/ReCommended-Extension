using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithNonNegativeValueAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithNonNegativeValue>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithNonNegativeValue";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityLambda() => DoNamedTest2();
}