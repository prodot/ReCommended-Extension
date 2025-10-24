using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Method;

namespace ReCommendedExtension.Tests.Analyzers.Method;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Method\QuickFixes";

    [Test]
    public void TestUseBinaryOperatorFix() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFix_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFix_OperandParenthesized() => DoNamedTest2();
}