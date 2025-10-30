using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUseBinaryOperatorFix() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFix_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseBinaryOperatorFix_OperandParenthesized() => DoNamedTest2();
}