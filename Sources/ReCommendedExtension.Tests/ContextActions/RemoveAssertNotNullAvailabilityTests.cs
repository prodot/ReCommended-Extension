using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class RemoveAssertNotNullAvailabilityTests : CSharpContextActionAvailabilityTestBase<RemoveAssertNotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\RemoveAssertNotNull";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}