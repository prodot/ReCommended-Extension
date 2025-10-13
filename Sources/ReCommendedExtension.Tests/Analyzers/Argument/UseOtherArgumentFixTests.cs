using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
[TestNetCore21]
public sealed class UseOtherArgumentFixTests : QuickFixTestBase<UseOtherArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestUseOtherArgument() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestUseOtherArgument_Named() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgument_AdditionalArgument() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgument_AdditionalArgument_Named() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgument_RedundantArgument() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgument_RedundantArgument_Named() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgument_RedundantArgument_OutOfOrder() => DoNamedTest2();
}