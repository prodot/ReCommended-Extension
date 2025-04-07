using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Byte;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\ByteQuickFixes";

    [Test]
    [TestNet70]
    public void TestClamp_EqualMinMax_TargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestClamp_EqualMinMax_NonTargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestClamp_FullRange_Typed() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestClamp_FullRange_TargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestClamp_FullRange_NonTargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem_Left_0_TargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem_Left_0_TargetTyped_Named() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem_Left_0_NonTargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem_Right_1_TargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem_Right_1_NonTargetTyped_LeftTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem_Right_1_NonTargetTyped_LeftConstant() => DoNamedTest2();

    [Test]
    public void TestEquals_Object() => DoNamedTest2();

    [Test]
    public void TestGetTypeCode() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestMax() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestMin() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateRight_NonTargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateLeft_Typed() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateLeft_TargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateLeft_NonTargetTyped() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateRight_Typed() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateRight_TargetTyped() => DoNamedTest2();
}