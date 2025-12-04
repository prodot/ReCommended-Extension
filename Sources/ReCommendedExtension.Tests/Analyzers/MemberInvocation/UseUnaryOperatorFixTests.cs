using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class UseUnaryOperatorFixTests : QuickFixTestBase<UseUnaryOperatorSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUseUnaryOperatorFix() => DoNamedTest2();

    [Test]
    public void TestUseUnaryOperatorFix_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseUnaryOperatorFix_OperandParenthesized() => DoNamedTest2();
}