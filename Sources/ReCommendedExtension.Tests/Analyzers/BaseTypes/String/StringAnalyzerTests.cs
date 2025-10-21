using System.Reflection;
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
        => highlighting is UseExpressionResultSuggestion
                or UseStringListPatternSuggestion
                or UseStringPropertySuggestion
                or UseRangeIndexerSuggestion
                or RedundantToStringCallWarning // to figure out which cases are supported by R#
                or ReplaceSubstringWithRangeIndexerWarning // to figure out which cases are supported by R#
                or ReturnValueOfPureMethodIsNotUsedWarning // to figure out which cases are supported by R#
            || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

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

    [Pure]
    static IEnumerable<E> GetEnumValues<E>() where E : struct, Enum
    {
        [Pure]
        static IEnumerable<MissingStringSplitOptions> GetMissingStringSplitOptions()
        {
            yield return MissingStringSplitOptions.None;
            yield return MissingStringSplitOptions.RemoveEmptyEntries;
            yield return MissingStringSplitOptions.TrimEntries;
            yield return MissingStringSplitOptions.RemoveEmptyEntries | MissingStringSplitOptions.TrimEntries;
        }

        if (typeof(E).GetCustomAttribute<FlagsAttribute>() is { })
        {
            if (typeof(E) == typeof(MissingStringSplitOptions))
            {
                return GetMissingStringSplitOptions().Cast<E>();
            }

            throw new NotSupportedException();
        }

        return Enum.GetValues(typeof(E)).Cast<E>();
    }

    static void Test<E, R>(string text, Func<string, E, R> expected, Func<string, E, R> actual, bool emptyThrows = false) where E : struct, Enum
    {
        foreach (var value in GetEnumValues<E>())
        {
            // empty
            if (emptyThrows)
            {
                Assert.Catch(() => expected("", value));
                Assert.Catch(() => actual("", value));
            }
            else
            {
                Assert.AreEqual(expected("", value), actual("", value));
            }

            // not empty
            Assert.AreEqual(expected(text, value), actual(text, value));
        }
    }

    static void TestNullable<E, R>(string text, Func<string?, E, R> expected, Func<string?, E, R> actual, bool emptyThrows = false)
        where E : struct, Enum
    {
        foreach (var value in GetEnumValues<E>())
        {
            // null
            Assert.AreEqual(expected(null, value), actual(null, value));

            // empty
            if (emptyThrows)
            {
                Assert.Catch(() => expected("", value));
                Assert.Catch(() => actual("", value));
            }
            else
            {
                Assert.AreEqual(expected("", value), actual("", value));
            }

            // not empty
            Assert.AreEqual(expected(text, value), actual(text, value));
        }
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore30]
    public void TestContains()
    {
        Test("abcde", text => text.Contains(""), _ => true);

        Test<StringComparison, bool>("abcde", (text, comparisonType) => text.Contains("", comparisonType), (_, _) => true);

        Test<StringComparison, bool>(
            "abcde",
            (text, comparisonType) => text.Contains("c", comparisonType),
            (text, comparisonType) => text.Contains('c', comparisonType));
        TestNullable<StringComparison, bool?>(
            "abcde",
            (text, comparisonType) => text?.Contains("c", comparisonType),
            (text, comparisonType) => text?.Contains('c', comparisonType));

        DoNamedTest2();
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

        Test("abcde", text => text.EndsWith(""), _ => true);

        Test<StringComparison, bool>("abcde", (text, comparisonType) => text.EndsWith("", comparisonType), (_, _) => true);
        Test("abcde", text => text.EndsWith("e", StringComparison.Ordinal), text => text is [.., 'e']);
        Test("abcde", text => text.EndsWith("e", StringComparison.OrdinalIgnoreCase), text => text is [.., 'e' or 'E']);

        DoNamedTest2();
    }

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestGetTypeCode()
    {
        Test("abcde", text => text.GetTypeCode(), _ => TypeCode.String);

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

        Test("abcde", text => text.IndexOf(""), _ => 0);

        Test<StringComparison, int>("abcde", (text, comparisonType) => text.IndexOf("", comparisonType), (_, _) => 0);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "UseOtherMethod")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    public void TestIndexOfAny()
    {
        Test("abcde", text => text.IndexOfAny([]), _ => -1);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestJoin()
    {
        Test(() => string.Join(", ", (object?[])[]), () => "");
        Test(() => string.Join(", ", (object?[])[100]), () => $"{100}");

        Test(() => MissingStringMethods.Join(", ", default(ReadOnlySpan<object?>)), () => "");
        Test(() => MissingStringMethods.Join(", ", new ReadOnlySpan<object?>()), () => "");
        Test(() => MissingStringMethods.Join(", ", (ReadOnlySpan<object?>)[]), () => "");
        Test(() => MissingStringMethods.Join(", ", (ReadOnlySpan<object?>)[100]), () => $"{100}");

        Test(() => string.Join(", ", (IEnumerable<int>)[]), () => "");
        Test(() => string.Join(", ", (IEnumerable<int>)[100]), () => $"{100}");

        Test(() => string.Join(", ", (string?[])[]), () => "");
        Test(() => string.Join(", ", (string?[])["item"]), () => "item");

        Test(() => string.Join(", ", (string?[])["item1", "item2"], 0, 0), () => "");
        Test(() => string.Join(", ", (string?[])["item"], 1, 0), () => "");
        Test(() => string.Join(", ", (string?[])["item"], 0, 1), () => "item");

        Test(() => MissingStringMethods.Join(", ", default(ReadOnlySpan<string?>)), () => "");
        Test(() => MissingStringMethods.Join(", ", new ReadOnlySpan<string?>()), () => "");
        Test(() => MissingStringMethods.Join(", ", (ReadOnlySpan<string?>)[]), () => "");
        Test(() => MissingStringMethods.Join(", ", (ReadOnlySpan<string?>)["item"]), () => "item");

        Test(() => MissingStringMethods.Join(',', (object?[])[]), () => "");
        Test(() => MissingStringMethods.Join(',', (object?[])[100]), () => $"{100}");

        Test(() => MissingStringMethods.Join(',', default(ReadOnlySpan<object?>)), () => "");
        Test(() => MissingStringMethods.Join(',', new ReadOnlySpan<object?>()), () => "");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<object?>)[]), () => "");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<object?>)[100]), () => $"{100}");

        Test(() => MissingStringMethods.Join(',', (IEnumerable<int>)[]), () => "");
        Test(() => MissingStringMethods.Join(',', (IEnumerable<int>)[100]), () => $"{100}");

        Test(() => MissingStringMethods.Join(',', (string?[])[]), () => "");
        Test(() => MissingStringMethods.Join(',', (string?[])["item"]), () => "item");

        Test(() => MissingStringMethods.Join(',', (string?[])["item1", "item2"], 0, 0), () => "");
        Test(() => MissingStringMethods.Join(',', (string?[])["item"], 1, 0), () => "");
        Test(() => MissingStringMethods.Join(',', (string?[])["item"], 0, 1), () => "item");

        Test(() => MissingStringMethods.Join(',', default(ReadOnlySpan<string?>)), () => "");
        Test(() => MissingStringMethods.Join(',', new ReadOnlySpan<string?>()), () => "");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<string?>)[]), () => "");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<string?>)["item"]), () => "item");

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
        Test("abcde", text => text.LastIndexOf('c', 0), _ => -1);

        // todo: uncomment the tests below when this is built with .NET 5 (https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/lastindexof-improved-handling-of-empty-values)

        /*
        Test("abcde", text => text.LastIndexOf(""), text => text.Length);
        TestNullable("abcde", text => text?.LastIndexOf(""), text => text?.Length);

        Test<StringComparison, int>("abcde", (text, comparisonType) => text.LastIndexOf("", comparisonType), (text, _) => text.Length);
        TestNullable<StringComparison, int?>("abcde", (text, comparisonType) => text?.LastIndexOf("", comparisonType), (text, _) => text?.Length);
        */

        Test("abcde", text => text.LastIndexOf("c", StringComparison.Ordinal), text => text.LastIndexOf('c'));
        TestNullable("abcde", text => text?.LastIndexOf("c", StringComparison.Ordinal), text => text?.LastIndexOf('c'));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseOtherMethod")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    public void TestLastIndexOfAny()
    {
        Test("abcde", text => text.LastIndexOfAny([]), _ => -1);

        Test("abcde", text => text.LastIndexOfAny(['b', 'c'], 0), _ => -1);

        Test("abcde", text => text.LastIndexOfAny(['b', 'c'], 0, 0), _ => -1);
        Test("abcde", text => text.LastIndexOfAny(['b', 'c'], 0, 1), _ => -1);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet60]
    [SuppressMessage("ReSharper", "UseRangeIndexer")]
    public void TestRemove()
    {
        // todo: uncomment the test below when this is built with .NET 6 (earlier frameworks throw exception for '"".Remove(0)')

        /*Test("abcde", text => text.Remove(0), _ => "");*/

        Test("abcde", text => text.Remove(2), text => text[..2], true);
        TestNullable("abcde", text => text?.Remove(2), text => text?[..2], true);

        Test("abcde", text => text.Remove(0, 2), text => text[2..], true);
        TestNullable("abcde", text => text?.Remove(0, 2), text => text?[2..], true);

        DoNamedTest2();
    }

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    public void TestSplit()
    {
        Test<MissingStringSplitOptions, string[]>("abcde", (text, options) => text.Split(',', 0, options), (_, _) => []);
        Test("  abcde  ", text => text.Split(',', 1), text => [text]);
        Test("  abcde  ", text => text.Split(',', 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);

        Test("abcde", text => text.Split([','], 0), _ => []);
        Test("ab,cd,e", text => text.Split([','], 1), text => [text]);

        Test<MissingStringSplitOptions, string[]>("abcde", (text, options) => text.Split([','], 0, options), (_, _) => []);
        Test("  abcde  ", text => text.Split([','], 1, MissingStringSplitOptions.None), text => [text]);
        Test("  abcde  ", text => text.Split([','], 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);

        Test("  abcde  ", text => text.Split(null as string), text => [text]);
        Test("  abcde  ", text => text.Split(""), text => [text]);
        Test("  abcde  ", text => text.Split(null as string, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);
        Test("  abcde  ", text => text.Split("", MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);

        Test<MissingStringSplitOptions, string[]>("abcde", (text, options) => text.Split("bc", 0, options), (_, _) => []);
        Test("  abcde  ", text => text.Split("bc", 1), text => [text]);
        Test("  abcde  ", text => text.Split("bc", 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);
        Test("  abcde  ", text => text.Split(null as string, 10), text => [text]);
        Test("  abcde  ", text => text.Split("", 10), text => [text]);
        Test("  abcde  ", text => text.Split(null as string, 10, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);
        Test("  abcde  ", text => text.Split("", 10, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);

        Test("  abcde  ", text => text.Split([""]), text => [text]);
        Test("  abcde  ", text => text.Split([""], MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);
        Test<MissingStringSplitOptions, string[]>(
            "  ab,cd,e  ",
            (text, options) => text.Split([","], options),
            (text, options) => text.Split([','], options));
        TestNullable<MissingStringSplitOptions, string[]?>(
            "  ab,cd,e  ",
            (text, options) => text?.Split([","], options),
            (text, options) => text?.Split([','], options));

        Test<MissingStringSplitOptions, string[]>("abcde", (text, options) => text.Split(["ab", "d"], 0, options), (_, _) => []);
        Test("  abcde  ", text => text.Split(["bd"], 1), text => [text]);
        Test("  abcde  ", text => text.Split(["bd"], 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);
        Test("  abcde  ", text => text.Split([""], 10), text => [text]);
        Test("  abcde  ", text => text.Split([""], 10, MissingStringSplitOptions.TrimEntries), text => [text.Trim()]);
        Test<MissingStringSplitOptions, string[]>(
            "  ab,cd,e  ",
            (text, options) => text.Split([","], 10, options),
            (text, options) => text.Split([','], 10, options));
        TestNullable<MissingStringSplitOptions, string[]?>(
            "  ab,cd,e  ",
            (text, options) => text?.Split([","], 10, options),
            (text, options) => text?.Split([','], 10, options));

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

        Test("abcde", text => text.StartsWith(""), _ => true);

        Test<StringComparison, bool>("abcde", (text, comparisonType) => text.StartsWith("", comparisonType), (_, _) => true);
        Test("abcde", text => text.StartsWith("a", StringComparison.Ordinal), text => text is ['a', ..]);
        Test("abcde", text => text.StartsWith("a", StringComparison.OrdinalIgnoreCase), text => text is ['a' or 'A', ..]);

        DoNamedTest2();
    }
}