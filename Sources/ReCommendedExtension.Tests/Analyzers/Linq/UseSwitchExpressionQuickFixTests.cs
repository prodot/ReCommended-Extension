using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[NullableContext(NullableContextKind.Enable)]
[TestNet70]
public sealed class UseSwitchExpressionQuickFixTests : QuickFixTestBase<UseSwitchExpressionFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq\QuickFixes";

    [Test]
    public void TestUseSwitchExpression_SingleOrDefault() => DoNamedTest2();

    [Test]
    public void TestUseSwitchExpression_SingleOrDefault_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseSwitchExpression_SingleOrDefault_DefaultValue() => DoNamedTest2();
}