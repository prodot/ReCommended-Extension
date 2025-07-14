using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNetCore21]
public sealed class RemoveElementQuickFixTests : QuickFixTestBase<RemoveElementFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    [Test]
    public void TestParseExact_RedundantElement() => DoNamedTest2();
}