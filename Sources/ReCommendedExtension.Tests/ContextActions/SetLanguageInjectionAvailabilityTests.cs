using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class SetLanguageInjectionAvailabilityTests : CSharpContextActionAvailabilityTestBase<SetLanguageInjection>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\SetLanguageInjection";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}