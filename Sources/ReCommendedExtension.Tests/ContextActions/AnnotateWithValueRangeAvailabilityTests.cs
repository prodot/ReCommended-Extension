using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class AnnotateWithValueRangeAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithValueRange>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithValueRange";

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestAvailability() => DoNamedTest2();

    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60(ANNOTATIONS_PACKAGE)]
    [Test]
    public void TestAvailabilityLambda() => DoNamedTest2();
}