using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Boolean;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Boolean\QuickFixes";

    [Test]
    public void TestEquals_Boolean_Operator() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Operator_Parenthesized() => DoNamedTest2();
}