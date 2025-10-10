using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNetCore21]
public sealed class UseOtherArgumentFixTests : QuickFixTestBase<UseOtherArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    public void TestUseOtherArgument() => DoNamedTest2();

    [Test]
    public void TestUseOtherArgument_Named() => DoNamedTest2();
}