using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[TestNetCore30]
public sealed class UseRangeIndexerFixTests : QuickFixTestBase<UseRangeIndexerFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestRemove_Int32() => DoNamedTest2();

    [Test]
    public void TestRemove_Int32_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestRemove_0_Int32() => DoNamedTest2();

    [Test]
    public void TestRemove_0_Int32_Parenthesized() => DoNamedTest2();
}