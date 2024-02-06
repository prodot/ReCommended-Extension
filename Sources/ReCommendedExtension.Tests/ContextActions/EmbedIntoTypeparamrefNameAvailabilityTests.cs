using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbedIntoTypeparamrefNameAvailabilityTests : CSharpContextActionAvailabilityTestBase<EmbedIntoTypeparamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoTypeparamrefName";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}