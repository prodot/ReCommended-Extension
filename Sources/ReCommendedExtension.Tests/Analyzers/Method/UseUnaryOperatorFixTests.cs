using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Method;

namespace ReCommendedExtension.Tests.Analyzers.Method;

[TestFixture]
public sealed class UseUnaryOperatorFixTests : QuickFixTestBase<UseUnaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Method\QuickFixes";

    [Test]
    public void TestUseUnaryOperatorFix() => DoNamedTest2();

    [Test]
    public void TestUseUnaryOperatorFix_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseUnaryOperatorFix_OperandParenthesized() => DoNamedTest2();
}