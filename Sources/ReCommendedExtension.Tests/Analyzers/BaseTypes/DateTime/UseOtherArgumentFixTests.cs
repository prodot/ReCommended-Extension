using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class UseOtherArgumentFixTests : QuickFixTestBase<UseOtherArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime\QuickFixes";

    [Test]
    public void TestParseExact_Single_Named() => DoNamedTest2();
}