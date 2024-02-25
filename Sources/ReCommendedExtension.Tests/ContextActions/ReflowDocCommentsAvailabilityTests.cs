using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class ReflowDocCommentsAvailabilityTests : CSharpContextActionAvailabilityTestBase<ReflowDocComments>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ReflowDocComments";

    [Test]
    public void TestAvailability_TopLevelTags() => DoNamedTest2();

    [Test]
    public void TestAvailability_NestedTags() => DoNamedTest2();
}