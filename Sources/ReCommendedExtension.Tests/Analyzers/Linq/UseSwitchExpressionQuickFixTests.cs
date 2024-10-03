using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class UseSwitchExpressionQuickFixTests : QuickFixTestBase<UseSwitchExpressionFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\LinqQuickFixes";

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseSwitchExpression_SingleOrDefault() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseSwitchExpression_SingleOrDefault_DefaultValue() => DoNamedTest2();
}