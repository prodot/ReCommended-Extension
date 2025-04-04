using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Boolean;

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\BooleanQuickFixes";

    [Test]
    public void TestEquals_Boolean_Qualifier_Inverted() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Qualifier_Inverted_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Argument_Inverted() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Argument_Inverted_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestEquals_Object() => DoNamedTest2();

    [Test]
    public void TestGetTypeCode() => DoNamedTest2();
}