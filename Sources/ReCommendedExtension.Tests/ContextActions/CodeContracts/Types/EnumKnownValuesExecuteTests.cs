using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestNetFramework4]
[TestFixture]
public sealed class EnumKnownValuesExecuteTests : CSharpContextActionExecuteTestBase<EnumKnownValues>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\EnumKnownValues";

    [Test]
    public void TestExecuteTwoMembers() => DoNamedTest2();

    [Test]
    public void TestExecuteFourMembers() => DoNamedTest2();
}