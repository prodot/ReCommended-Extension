using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
[TestNet70]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[NullableContext(NullableContextKind.Enable)]
public sealed class UseListPatternQuickFixTests : QuickFixTestBase<UseLinqListPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\LinqQuickFixes";

    [Test]
    public void TestUseListPattern_FirstOrDefault() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_FirstOrDefault_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_LastOrDefault() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_LastOrDefault_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_FirstOrDefault_DefaultValue() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_FirstOrDefault_DefaultValue_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_LastOrDefault_DefaultValue() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_LastOrDefault_DefaultValue_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_Single() => DoNamedTest2();

    [Test]
    public void TestUseListPattern_Single_Parenthesized() => DoNamedTest2();
}