using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime\QuickFixes";

    [Test]
    public void Test_Constructors_ExpressionResult() => DoNamedTest2();

    [Test]
    public void TestEquals_Null() => DoNamedTest2();

    [Test]
    public void TestGetTypeCode() => DoNamedTest2();
}