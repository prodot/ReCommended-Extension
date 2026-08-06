using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ExpressionResult;

namespace ReCommendedExtension.Tests.Analyzers.ExpressionResult;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\ExpressionResult\QuickFixes";

    [Test]
    [TestNet70]
    public void UseExpressionResultFix() => DoNamedTest();

    [Test]
    [TestNet70]
    public void UseExpressionResultFix_NonParenthesized() => DoNamedTest();

    [Test]
    [TestNet70]
    public void UseExpressionResultFix_Parenthesized() => DoNamedTest();

    [Test]
    public void UseExpressionResultFix_Parenthesized_Unary() => DoNamedTest();

    [Test]
    public void UseExpressionResultFix_NonParenthesized_Unary() => DoNamedTest();
}