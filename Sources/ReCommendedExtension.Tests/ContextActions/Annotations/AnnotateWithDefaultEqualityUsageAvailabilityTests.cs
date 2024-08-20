using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithDefaultEqualityUsageAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithDefaultEqualityUsage>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithDefaultEqualityUsage";

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailability() => DoNamedTest2();
}