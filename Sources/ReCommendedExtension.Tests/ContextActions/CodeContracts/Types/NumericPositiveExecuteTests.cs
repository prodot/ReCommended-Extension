using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class NumericPositiveExecuteTests : CSharpContextActionExecuteTestBase<NumericPositive>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\NumericPositive";

    [Test]
    public void TestExecuteInt() => DoNamedTest2();

    [Test]
    public void TestExecuteUInt() => DoNamedTest2();

    [Test]
    public void TestExecuteLong() => DoNamedTest2();

    [Test]
    public void TestExecuteUlong() => DoNamedTest2();

    [Test]
    public void TestExecuteByte() => DoNamedTest2();

    [Test]
    public void TestExecuteSbyte() => DoNamedTest2();

    [Test]
    public void TestExecuteShort() => DoNamedTest2();

    [Test]
    public void TestExecuteUshort() => DoNamedTest2();

    [Test]
    public void TestExecuteDecimal() => DoNamedTest2();

    [Test]
    public void TestExecuteDouble() => DoNamedTest2();

    [Test]
    public void TestExecuteFloat() => DoNamedTest2();
}