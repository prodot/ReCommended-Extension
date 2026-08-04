using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void UseBinaryOperatorFix() => DoNamedTest();

    [Test]
    public void UseBinaryOperatorFix_Parenthesized() => DoNamedTest();

    [Test]
    public void UseBinaryOperatorFix_OperandParenthesized() => DoNamedTest();
}