using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.String;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[NullableContext(NullableContextKind.Enable)]
[TestNet70]
public sealed class UseListPatternQuickFixTests : QuickFixTestBase<UseStringListPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Strings\QuickFixes";

    [Test]
    public void TestEndsWith_Char() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Char_Constant() => DoNamedTest2();

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