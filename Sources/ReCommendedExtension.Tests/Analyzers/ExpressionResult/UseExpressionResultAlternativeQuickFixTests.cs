using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ExpressionResult;

namespace ReCommendedExtension.Tests.Analyzers.ExpressionResult;

[TestFixture]
[TestNet70]
public sealed class UseExpressionResultAlternativeQuickFixTests : QuickFixTestBase<UseExpressionResultAlternativeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\ExpressionResult\QuickFixes";

    [Test]
    public void TestUseExpressionResultAlternativeFix() => DoNamedTest2();

    [Test]
    public void TestUseExpressionResultAlternativeFix_Parenthesized() => DoNamedTest2();
}