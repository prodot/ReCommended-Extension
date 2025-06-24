using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.String;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[NullableContext(NullableContextKind.Enable)]
[TestNetCore30]
public sealed class UseOtherMethodQuickFixTests : QuickFixTestBase<UseOtherMethodFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Strings\QuickFixes";

    [Test]
    public void TestIndexOf_Char_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_lt_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_lt_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_ne_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_lt_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_ne_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_lt_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_SingleElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_Int32_SingleElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_Int32_Int32_SingleElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestLastIndexOfAny_CharArray_SingleElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestLastIndexOfAny_CharArray_Int32_SingleElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestLastIndexOfAny_CharArray_Int32_Int32_SingleElement() => DoNamedTest2();
}