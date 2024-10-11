using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class UseListPatternQuickFixTests : QuickFixTestBase<UseLinqListPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\LinqQuickFixes";

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_First() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_First_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_Last() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_Last_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_FirstOrDefault() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_FirstOrDefault_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_LastOrDefault() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_LastOrDefault_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_FirstOrDefault_DefaultValue() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_FirstOrDefault_DefaultValue_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_LastOrDefault_DefaultValue() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_LastOrDefault_DefaultValue_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_Single() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_Single_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_ElementAt_First() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_ElementAt_First_Parenthesized() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_ElementAt_Last() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestUseListPattern_ElementAt_Last_Parenthesized() => DoNamedTest2();
}