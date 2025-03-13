using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class StringNotNullAndNotEmptyAvailabilityTests : CSharpContextActionAvailabilityTestBase<StringNotNullAndNotEmpty>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\StringNotNullAndNotEmpty";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}