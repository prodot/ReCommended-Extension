using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class UseUnaryOperatorFixTests : QuickFixTestBase<UseUnaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DecimalQuickFixes";

    [Test]
    public void TestNegate() => DoNamedTest2();
}