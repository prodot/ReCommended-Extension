using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Char;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet70]
public sealed class CharAnalyzerTests : BaseTypeAnalyzerTests<char>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Char";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseCharRangePatternSuggestion || highlighting.IsError();

    protected override char[] TestValues { get; } = ['a', 'A', '1', ' ', 'ä', 'ß', '€', char.MinValue, char.MaxValue];

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiDigit()
    {
        Test(c => MissingCharMethods.IsAsciiDigit(c), c => c is >= '0' and <= '9');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiHexDigit()
    {
        Test(c => MissingCharMethods.IsAsciiHexDigit(c), c => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiHexDigitLower()
    {
        Test(c => MissingCharMethods.IsAsciiHexDigitLower(c), c => c is >= '0' and <= '9' or >= 'a' and <= 'f');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiHexDigitUpper()
    {
        Test(c => MissingCharMethods.IsAsciiHexDigitUpper(c), c => c is >= '0' and <= '9' or >= 'A' and <= 'F');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetter()
    {
        Test(c => MissingCharMethods.IsAsciiLetter(c), c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetterLower()
    {
        Test(c => MissingCharMethods.IsAsciiLetterLower(c), c => c is >= 'a' and <= 'z');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetterOrDigit()
    {
        Test(c => MissingCharMethods.IsAsciiLetterOrDigit(c), c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsAsciiLetterUpper()
    {
        Test(c => MissingCharMethods.IsAsciiLetterUpper(c), c => c is >= 'A' and <= 'Z');

        DoNamedTest2();
    }

    [Test]
    public void TestIsBetween()
    {
        Test(c => MissingCharMethods.IsBetween(c, 'a', 'c'), c => c is >= 'a' and <= 'c');

        DoNamedTest2();
    }
}