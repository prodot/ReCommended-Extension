using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[TestNet70]
public sealed class UseListPatternQuickFixTests : QuickFixTestBase<UseStringListPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestEndsWith_Char() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Char_ParameterName() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Argument() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Argument_ParameterName() => DoNamedTest2();

    [Test]
    public void TestEndsWith_String_Ordinal() => DoNamedTest2();

    [Test]
    public void TestEndsWith_String_OrdinalIgnoreCase() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf_Char_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_CharConst_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_ne_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_CharConst_ne_0() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Char() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Char_ParameterName() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Argument() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Argument_ParameterName() => DoNamedTest2();

    [Test]
    public void TestStartsWith_String_Ordinal() => DoNamedTest2();

    [Test]
    public void TestStartsWith_String_OrdinalIgnoreCase() => DoNamedTest2();
}