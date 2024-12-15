using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
public sealed class PassSingleCharactersQuickFixTests : QuickFixTestBase<PassSingleCharactersFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_ParameterName1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_ParameterName2() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_ParameterNames() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_ParameterName1() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_ParameterName2() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_ParameterNames() => DoNamedTest2();

    [Test]
    public void TestSplit_SingleCharacters() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_SingleCharacters_CollectionExpression() => DoNamedTest2();
}