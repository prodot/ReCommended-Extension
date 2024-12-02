using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestContains_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestContains_Empty_StringComparison() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestContains_Empty_Expression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestEndsWith_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestEndsWith_Empty_StringComparison() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Empty() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Empty_StringComparison() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestLastIndexOfAny_CharArray_Empty() => DoNamedTest2();

    [Test]
    public void TestLastIndexOfAny_CharArray_0() => DoNamedTest2();

    [Test]
    public void TestLastIndexOfAny_CharArray_0_0() => DoNamedTest2();

    [Test]
    public void TestLastIndexOfAny_CharArray_0_1() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_Char_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60]
    public void TestRemove_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestSplit_EmptyArray() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_EmptyArray_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestSplit_ArrayWithOneItem() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_ArrayWithOneItem_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_ArrayWithOneTrimmedItem() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_ArrayWithOneTrimmedItem_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestStartsWith_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestStartsWith_Empty_StringComparison() => DoNamedTest2();
}