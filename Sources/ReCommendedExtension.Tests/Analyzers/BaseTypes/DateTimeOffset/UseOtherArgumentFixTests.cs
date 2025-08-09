using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTimeOffset;

[TestFixture]
public sealed class UseOtherArgumentFixTests : QuickFixTestBase<UseOtherArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTimeOffset\QuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestParseExact_Single_Named() => DoNamedTest2();

    [Test]
    public void TestTryParseExact() => DoNamedTest2();
}