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
    public void UseNullableHasValueNotNullPatternFix() => DoNamedTest();

    [Test]
    public void UseNullableHasValueNotNullPatternFix_Parenthesized() => DoNamedTest();
}