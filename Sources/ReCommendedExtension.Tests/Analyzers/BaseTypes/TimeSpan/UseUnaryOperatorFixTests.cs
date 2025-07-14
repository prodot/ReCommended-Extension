using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class UseUnaryOperatorFixTests : QuickFixTestBase<UseUnaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan\QuickFixes";

    [Test]
    public void TestNegate() => DoNamedTest2();

    [Test]
    public void TestNegate_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestNegate_Parenthesized_Result() => DoNamedTest2();
}