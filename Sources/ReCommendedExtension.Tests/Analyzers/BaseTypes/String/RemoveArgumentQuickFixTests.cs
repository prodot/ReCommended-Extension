using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.String;

[TestFixture]
public sealed class RemoveArgumentQuickFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Strings\QuickFixes";

    [Test]
    public void TestIndexOf_Char_Int32() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_Int32_ParameterName() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32_ParameterName() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32_StringComparison() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32_ParameterName_StringComparison() => DoNamedTest2();

    [Test]
    public void TestIndexOfAny() => DoNamedTest2();

    [Test]
    public void TestIndexOfAny_ParameterName() => DoNamedTest2();

    [Test]
    public void TestPadLeft_Int32_Space() => DoNamedTest2();

    [Test]
    public void TestPadRight_Int32_Space() => DoNamedTest2();

    [Test]
    public void TestSplit_DuplicateArgument() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrim_Empty() => DoNamedTest2();

    [Test]
    public void TestTrim_EmptyArray() => DoNamedTest2();

    [Test]
    public void TestTrim_EmptyArray_2() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrim_EmptyArray_3() => DoNamedTest2();

    [Test]
    public void TestTrim_Null() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimEnd_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrimEnd_EmptyArray_3() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrimEnd_Null() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimStart_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrimStart_EmptyArray_3() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrimStart_Null() => DoNamedTest2();

    [Test]
    public void TestTrim_DuplicateArgument() => DoNamedTest2();

    [Test]
    public void TestTrimEnd_DuplicateArgument() => DoNamedTest2();

    [Test]
    public void TestTrimStart_DuplicateArgument() => DoNamedTest2();
}