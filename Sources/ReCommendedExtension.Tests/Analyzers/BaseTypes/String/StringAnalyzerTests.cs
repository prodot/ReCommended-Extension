using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.String;

[TestFixture]
public sealed class StringAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Strings";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseStringListPatternSuggestion
                or UseStringPropertySuggestion
                or UseRangeIndexerSuggestion
                or RedundantToStringCallWarning // to figure out which cases are supported by R#
                or ReplaceSubstringWithRangeIndexerWarning // to figure out which cases are supported by R#
                or ReturnValueOfPureMethodIsNotUsedWarning // to figure out which cases are supported by R#
            || highlighting.IsError();

    static void Test<R>(string text, Func<string, R> expected, Func<string, R> actual, bool emptyThrows = false)
    {
        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected(""));
            Assert.Catch(() => actual(""));
        }
        else
        {
            Assert.AreEqual(expected(""), actual(""));
        }

        // not empty
        Assert.AreEqual(expected(text), actual(text));
    }

    static void TestNullable<R>(string text, Func<string?, R> expected, Func<string?, R> actual, bool emptyThrows = false)
    {
        // null
        Assert.AreEqual(expected(null), actual(null));

        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected(""));
            Assert.Catch(() => actual(""));
        }
        else
        {
            Assert.AreEqual(expected(""), actual(""));
        }

        // not empty
        Assert.AreEqual(expected(text), actual(text));
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet70]
    [SuppressMessage("ReSharper", "StringEndsWithIsCultureSpecific")]
    [SuppressMessage("ReSharper", "MergeIntoPattern")]
    public void TestEndsWith()
    {
        Test("abcde", text => text.EndsWith('e'), text => text is [.., 'e']);
        Test("abcde", text => text.EndsWith('e'), text => text is [.., var lastChar] && lastChar == 'e');

        Test("abcde", text => text.EndsWith("e", StringComparison.Ordinal), text => text is [.., 'e']);
        Test("abcde", text => text.EndsWith("e", StringComparison.OrdinalIgnoreCase), text => text is [.., 'e' or 'E']);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet70]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.2")]
    [SuppressMessage("ReSharper", "StringStartsWithIsCultureSpecific")]
    [SuppressMessage("ReSharper", "MergeIntoPattern")]
    [SuppressMessage("ReSharper", "MergeIntoNegatedPattern")]
    [SuppressMessage("ReSharper", "UseStringListPattern")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestIndexOf()
    {
        Test("abcde", text => text.IndexOf('a') == 0, text => text is ['a', ..]);
        TestNullable("abcde", text => text?.IndexOf('a') == 0, text => text is ['a', ..]);

        Test("abcde", text => text.IndexOf('a') == 0, text => text is [var firstChar, ..] && firstChar == 'a');
        TestNullable("abcde", text => text?.IndexOf('a') == 0, text => text is [var firstChar, ..] && firstChar == 'a');

        Test("abcde", text => text.IndexOf('c') != 0, text => text is not ['c', ..]);
        TestNullable("abcde", text => text?.IndexOf('c') != 0, text => text is not ['c', ..]);

        Test("abcde", text => text.IndexOf('c') != 0, text => text is not [var firstChar, ..] || firstChar != 'c');
        TestNullable("abcde", text => text?.IndexOf('c') != 0, text => text is not [var firstChar, ..] || firstChar != 'c');

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet50]
    [SuppressMessage("ReSharper", "StringLastIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "PassSingleCharacter")]
    public void TestLastIndexOf()
    {
        // todo: uncomment the tests below when this is built with .NET 5 (https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/lastindexof-improved-handling-of-empty-values)

        /*
        Test("abcde", text => text.LastIndexOf(""), text => text.Length);
        TestNullable("abcde", text => text?.LastIndexOf(""), text => text?.Length);

        Test<StringComparison, int>("abcde", (text, comparisonType) => text.LastIndexOf("", comparisonType), (text, _) => text.Length);
        TestNullable<StringComparison, int?>("abcde", (text, comparisonType) => text?.LastIndexOf("", comparisonType), (text, _) => text?.Length);
        */

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet60]
    [SuppressMessage("ReSharper", "UseRangeIndexer")]
    public void TestRemove()
    {
        Test("abcde", text => text.Remove(2), text => text[..2], true);
        TestNullable("abcde", text => text?.Remove(2), text => text?[..2], true);

        Test("abcde", text => text.Remove(0, 2), text => text[2..], true);
        TestNullable("abcde", text => text?.Remove(0, 2), text => text?[2..], true);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet70]
    [SuppressMessage("ReSharper", "StringStartsWithIsCultureSpecific")]
    [SuppressMessage("ReSharper", "MergeIntoPattern")]
    public void TestStartsWith()
    {
        Test("abcde", text => text.StartsWith('a'), text => text is ['a', ..]);
        Test("abcde", text => text.StartsWith('a'), text => text is [var firstChar, ..] && firstChar == 'a');

        Test("abcde", text => text.StartsWith("a", StringComparison.Ordinal), text => text is ['a', ..]);
        Test("abcde", text => text.StartsWith("a", StringComparison.OrdinalIgnoreCase), text => text is ['a' or 'A', ..]);

        DoNamedTest2();
    }
}