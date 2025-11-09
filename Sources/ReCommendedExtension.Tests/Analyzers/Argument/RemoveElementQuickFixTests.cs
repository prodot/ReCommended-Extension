using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNetCore21]
public sealed class RemoveElementQuickFixTests : QuickFixTestBase<RemoveElementFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    [TestNet70]
    public void TestRemoveElement_Middle() => DoNamedTest2();

    [Test]
    public void TestRemoveElement_Last() => DoNamedTest2();
}