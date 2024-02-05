using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbedIntoSeeCRefAvailabilityTests : CSharpContextActionAvailabilityTestBase<EmbedIntoSeeCRef>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoSeeCRef";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}