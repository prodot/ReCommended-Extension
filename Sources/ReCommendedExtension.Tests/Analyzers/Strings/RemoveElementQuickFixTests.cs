using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
public sealed class RemoveElementQuickFixTests : QuickFixTestBase<RemoveElementFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_DuplicateElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_Int32_DuplicateElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestIndexOfAny_CharArray_Int32_Int32_DuplicateElement() => DoNamedTest2();

    [Test]
    public void TestSplit_DuplicateElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_DuplicateElement_CollectionExpression() => DoNamedTest2();

    [Test]
    public void TestTrim_DuplicateElement() => DoNamedTest2();

    [Test]
    public void TestTrimEnd_DuplicateElement() => DoNamedTest2();

    [Test]
    public void TestTrimStart_DuplicateElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrim_DuplicateElement_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimEnd_DuplicateElement_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimStart_DuplicateElement_CollectionExpression() => DoNamedTest2();
}