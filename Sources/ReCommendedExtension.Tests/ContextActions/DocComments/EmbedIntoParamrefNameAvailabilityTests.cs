using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class EmbedIntoParamrefNameAvailabilityTests : CSharpContextActionAvailabilityTestBase<EmbedIntoParamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoParamrefName";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}