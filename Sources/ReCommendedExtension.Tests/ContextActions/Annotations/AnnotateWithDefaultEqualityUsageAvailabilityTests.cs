using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
public sealed class AnnotateWithDefaultEqualityUsageAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithDefaultEqualityUsage>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithDefaultEqualityUsage";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}