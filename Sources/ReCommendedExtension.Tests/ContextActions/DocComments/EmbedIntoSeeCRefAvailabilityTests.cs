using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class EmbedIntoSeeCRefAvailabilityTests : CSharpContextActionAvailabilityTestBase<EmbedIntoSeeCRef>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoSeeCRef";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}