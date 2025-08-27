using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeOnly;

[TestFixture]
[TestNet60]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeOnly\QuickFixes";

    [Test]
    public void Test_Constructors_ExpressionResult() => DoNamedTest2();

    [Test]
    public void TestEquals_Null() => DoNamedTest2();
}