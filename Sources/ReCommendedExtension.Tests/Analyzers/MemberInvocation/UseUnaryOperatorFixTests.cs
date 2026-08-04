using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class UseUnaryOperatorFixTests : QuickFixTestBase<UseUnaryOperatorSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void UseUnaryOperatorFix() => DoNamedTest();

    [Test]
    public void UseUnaryOperatorFix_Parenthesized() => DoNamedTest();

    [Test]
    public void UseUnaryOperatorFix_OperandParenthesized() => DoNamedTest();
}