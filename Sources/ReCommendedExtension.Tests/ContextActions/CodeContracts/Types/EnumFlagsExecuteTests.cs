using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class EnumFlagsExecuteTests : CSharpContextActionExecuteTestBase<EnumFlags>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\EnumFlags";

    [Test]
    public void TestExecuteWithZero() => DoNamedTest2();

    [Test]
    public void TestExecuteWithoutZero() => DoNamedTest2();

    [Test]
    public void TestExecuteWithZeroSingleMember() => DoNamedTest2();
}