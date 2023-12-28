using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class AnnotateWithAttributeUsageAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithAttributeUsage>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithAttributeUsage";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}