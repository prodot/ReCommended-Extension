using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class UseOtherArgumentRangeFixTests : QuickFixTestBase<UseOtherArgumentRangeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    public void TestUseOtherArgumentRange() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgumentRange_Named() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgumentRange_OutOfOrder() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestUseOtherArgumentRange_RedundantArgument() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestUseOtherArgumentRange_RedundantArgument_Named() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestUseOtherArgumentRange_RedundantArgument_OutOfOrder() => DoNamedTest2();
}