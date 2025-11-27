using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
public sealed class UseNullableHasValueNotNullPatternFixTests : QuickFixTestBase<UseNullableHasValueAlternativeSuggestion.NotNullPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUseNullableHasValueNotNullPatternFix() => DoNamedTest2();

    [Test]
    public void TestUseNullableHasValueNotNullPatternFix_Parenthesized() => DoNamedTest2();
}