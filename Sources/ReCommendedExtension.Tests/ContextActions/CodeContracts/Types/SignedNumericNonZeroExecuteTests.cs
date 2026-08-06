using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class SignedNumericNonZeroExecuteTests : CSharpContextActionExecuteTestBase<SignedNumericNonZero>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\SignedNumericNonZero";

    [Test]
    public void ExecuteInt() => DoNamedTest();

    [Test]
    public void ExecuteLong() => DoNamedTest();

    [Test]
    public void ExecuteSbyte() => DoNamedTest();

    [Test]
    public void ExecuteShort() => DoNamedTest();

    [Test]
    public void ExecuteDecimal() => DoNamedTest();

    [Test]
    public void ExecuteDouble() => DoNamedTest();

    [Test]
    public void ExecuteFloat() => DoNamedTest();
}