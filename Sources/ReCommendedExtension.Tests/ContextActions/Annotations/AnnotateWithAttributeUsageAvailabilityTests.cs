using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithAttributeUsageAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithAttributeUsage>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithAttributeUsage";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}