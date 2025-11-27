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
    public void TestUseExpressionResultFix() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestUseExpressionResultFix_NonParenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestUseExpressionResultFix_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseExpressionResultFix_Parenthesized_Unary() => DoNamedTest2();

    [Test]
    public void TestUseExpressionResultFix_NonParenthesized_Unary() => DoNamedTest2();
}