using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class TimeSpanNonZeroAvailabilityTests : CSharpContextActionAvailabilityTestBase<TimeSpanNonZero>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\TimeSpanNonZero";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}