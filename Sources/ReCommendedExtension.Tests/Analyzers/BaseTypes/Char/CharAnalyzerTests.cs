using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Char;

[TestFixture]
public sealed class CharAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Char";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseBinaryOperatorSuggestion or UseExpressionResultSuggestion or UseCharRangePatternSuggestion or RedundantArgumentHint
            || highlighting.IsError();

    static void Test<R>(Func<char, R> expected, Func<char, R> actual)
    {
        Assert.AreEqual(expected('a'), actual('a'));
        Assert.AreEqual(expected('A'), actual('A'));
        Assert.AreEqual(expected('1'), actual('1'));
        Assert.AreEqual(expected(' '), actual(' '));
        Assert.AreEqual(expected('€'), actual('€'));
        Assert.AreEqual(expected(char.MinValue), actual(char.MinValue));
        Assert.AreEqual(expected(char.MaxValue), actual(char.MaxValue));
    }

    static void Test<R>(Func<char, char, R> expected, Func<char, char, R> actual)
    {
        Assert.AreEqual(expected('a', 'a'), actual('a', 'a'));
        Assert.AreEqual(expected('a', 'A'), actual('a', 'A'));
        Assert.AreEqual(expected('€', ' '), actual('€', ' '));
        Assert.AreEqual(expected(char.MinValue, char.MinValue), actual(char.MinValue, char.MinValue));
        Assert.AreEqual(expected(char.MinValue, char.MaxValue), actual(char.MinValue, char.MaxValue));
        Assert.AreEqual(expected(char.MaxValue, char.MaxValue), actual(char.MaxValue, char.MaxValue));
        Assert.AreEqual(expected(char.MaxValue, char.MinValue), actual(char.MaxValue, char.MinValue));
    }

    [Test]
    public void TestEquals()
    {
        Test((character, obj) => character.Equals(obj), (character, obj) => character == obj);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestGetTypeCode()
    {
        Test(character => character.GetTypeCode(), _ => TypeCode.Char);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiDigit()
    {
        Test(c => MissingCharMethods.IsAsciiDigit(c), c => c is >= '0' and <= '9');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiHexDigit()
    {
        Test(c => MissingCharMethods.IsAsciiHexDigit(c), c => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiHexDigitLower()
    {
        Test(c => MissingCharMethods.IsAsciiHexDigitLower(c), c => c is >= '0' and <= '9' or >= 'a' and <= 'f');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiHexDigitUpper()
    {
        Test(c => MissingCharMethods.IsAsciiHexDigitUpper(c), c => c is >= '0' and <= '9' or >= 'A' and <= 'F');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetter()
    {
        Test(c => MissingCharMethods.IsAsciiLetter(c), c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetterLower()
    {
        Test(c => MissingCharMethods.IsAsciiLetterLower(c), c => c is >= 'a' and <= 'z');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetterOrDigit()
    {
        Test(c => MissingCharMethods.IsAsciiLetterOrDigit(c), c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetterUpper()
    {
        Test(c => MissingCharMethods.IsAsciiLetterUpper(c), c => c is >= 'A' and <= 'Z');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    public void TestIsBetween()
    {
        Test(c => MissingCharMethods.IsBetween(c, 'a', 'c'), c => c is >= 'a' and <= 'c');

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        Test(character => character.ToString(null), character => character.ToString());
        Test(character => character.ToString(CultureInfo.CurrentCulture), character => character.ToString());

        DoNamedTest2();
    }
}