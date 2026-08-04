using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ExpressionResult;

namespace ReCommendedExtension.Tests.Analyzers.ExpressionResult;

[TestFixture]
[TestNet70]
public sealed class UseExpressionResultAlternativeQuickFixTests : QuickFixTestBase<UseExpressionResultSuggestion.AlternativeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\ExpressionResult\QuickFixes";

    [Test]
    public void UseExpressionResultAlternativeFix() => DoNamedTest();

    [Test]
    public void UseExpressionResultAlternativeFix_Parenthesized() => DoNamedTest();
}