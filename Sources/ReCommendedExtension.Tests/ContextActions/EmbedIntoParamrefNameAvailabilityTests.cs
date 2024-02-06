using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbedIntoParamrefNameAvailabilityTests : CSharpContextActionAvailabilityTestBase<EmbedIntoParamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoParamrefName";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}