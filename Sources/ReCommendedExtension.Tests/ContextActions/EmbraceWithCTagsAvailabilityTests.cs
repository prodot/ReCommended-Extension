using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbraceWithCTagsAvailabilityTests : CSharpContextActionAvailabilityTestBase<EmbraceWithCTags>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbraceWithCTags";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}