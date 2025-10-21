using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Method;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.Method;

[TestFixture]
public sealed class MethodAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Method";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantMethodInvocationHint or UseOtherMethodSuggestion || highlighting.IsError();

    static void Test<T, R>(Func<T, R> expected, Func<T, R> actual, T[] args)
    {
        foreach (var a in args)
        {
            Assert.AreEqual(expected(a), actual(a), $"with value: {a}");
        }
    }

    static void Test<T, U, R>(Func<T, U, R> expected, Func<T, U, R> actual, T[] args1, U[] args2)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                Assert.AreEqual(expected(a, b), actual(a, b), $"with values: {a}, {b}");
            }
        }
    }

    static void Test<T, U, V, R>(Func<T, U, V, R> expected, Func<T, U, V, R> actual, T[] args1, U[] args2, V[] args3)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                foreach (var c in args3)
                {
                    Assert.AreEqual(expected(a, b, c), actual(a, b, c), $"with values: {a}, {b}, {c}");
                }
            }
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestBoolean()
    {
        var values = new[] { true, false };

        // redundant method invocation

        Test(flag => flag.Equals(true), flag => flag, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestDateTime()
    {
        var values = new[]
        {
            DateTime.MinValue,
            DateTime.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
        };

        // redundant method invocation

        Test(dateTime => dateTime.AddTicks(0), dateTime => dateTime, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestDateTimeOffset()
    {
        var values = new[]
        {
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.Zero),
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.FromHours(2)),
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.FromHours(-6)),
        };

        // redundant method invocation

        Test(dateTimeOffset => dateTimeOffset.AddTicks(0), dateTimeOffset => dateTimeOffset, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestDateOnly()
    {
        var values = new[] { DateOnly.MinValue, DateOnly.MaxValue, new(2025, 7, 15) };

        // redundant method invocation

        Test(dateOnly => dateOnly.AddDays(0), dateOnly => dateOnly, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "StringStartsWithIsCultureSpecific")]
    [SuppressMessage("ReSharper", "UseOtherMethod")]
    public void TestString()
    {
        var values = new[] { null, "", "abcde", "  abcde  ", "ab;cd;e", "ab;cd:e", "..abcde.." };
        var chars = new[] { 'c', 'x' };
        var comparisons = new[]
        {
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase,
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
        };

        var valuesNonNull = (from item in values where item is { } select item).ToArray();

        // redundant method invocation

        Test(text => text?.PadLeft(0), text => text, values);
        Test(text => text?.PadLeft(0, 'x'), text => text, values);

        Test(text => text?.PadRight(0), text => text, values);
        Test(text => text?.PadRight(0, 'x'), text => text, values);

        Test(text => text?.Replace('c', 'c'), text => text, values);
        Test(text => text?.Replace("cd", "cd"), text => text, values);
        Test(text => text?.Replace("cd", "cd", StringComparison.Ordinal), text => text, values);

        Test(text => text?.Substring(0), text => text, values);

        // other method invocation

        Test((text, c) => text.IndexOf(c) != -1, (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => text.IndexOf(c) > -1, (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => text.IndexOf(c) >= 0, (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => -1 != text.IndexOf(c), (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => -1 < text.IndexOf(c), (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => 0 <= text.IndexOf(c), (text, c) => text.Contains(c), valuesNonNull, chars);

        Test((text, c) => text.IndexOf(c) == -1, (text, c) => !text.Contains(c), valuesNonNull, chars);
        Test((text, c) => text.IndexOf(c) < 0, (text, c) => !text.Contains(c), valuesNonNull, chars);
        Test((text, c) => -1 == text.IndexOf(c), (text, c) => !text.Contains(c), valuesNonNull, chars);
        Test((text, c) => 0 > text.IndexOf(c), (text, c) => !text.Contains(c), valuesNonNull, chars);

        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) != -1,
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) > -1,
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) >= 0,
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => -1 != text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => -1 < text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => 0 <= text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);

        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) == -1,
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) < 0,
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => -1 == text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => 0 > text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);

        Test((text, s) => text.IndexOf(s) == 0, (text, s) => text.StartsWith(s), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 == text.IndexOf(s), (text, s) => text.StartsWith(s), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) != 0, (text, s) => !text.StartsWith(s), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 != text.IndexOf(s), (text, s) => !text.StartsWith(s), valuesNonNull, valuesNonNull);

        Test((text, s) => text.IndexOf(s) != -1, (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) > -1, (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) >= 0, (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => -1 != text.IndexOf(s), (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => -1 < text.IndexOf(s), (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 <= text.IndexOf(s), (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);

        Test((text, s) => text.IndexOf(s) == -1, (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) < 0, (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => -1 == text.IndexOf(s), (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 > text.IndexOf(s), (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);

        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) == 0,
            (text, s, comparisonType) => text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 == text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) != 0,
            (text, s, comparisonType) => !text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 != text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => !text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);

        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) != -1,
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) > -1,
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) >= 0,
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => -1 != text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => -1 < text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 <= text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);

        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) == -1,
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) < 0,
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => -1 == text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 > text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);

        Test((text, c) => text?.IndexOfAny([c]), (text, c) => text?.IndexOf(c), values, chars);
        Test((text, c) => text?.IndexOfAny([c], 1), (text, c) => text?.IndexOf(c, 1), [..values.Except([""])], chars);
        Test((text, c) => text?.IndexOfAny([c], 1, 3), (text, c) => text?.IndexOf(c, 1, 3), [..values.Except([""])], chars);

        Test((text, c) => text?.LastIndexOfAny([c]), (text, c) => text?.LastIndexOf(c), values, chars);
        Test((text, c) => text?.LastIndexOfAny([c], 4), (text, c) => text?.LastIndexOf(c, 4), values, chars);
        Test((text, c) => text?.LastIndexOfAny([c], 4, 3), (text, c) => text?.LastIndexOf(c, 4, 3), values, chars);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public void TestStringBuilder()
    {
        var values = new[] { "", "abcde" };

        // redundant method invocation

        Test(value => new StringBuilder(value).Append(null as string).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append("").ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as char[]).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append([]).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as object).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as StringBuilder).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append('x', 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as string, 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append("abcde", 1, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as StringBuilder, 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(
            value => new StringBuilder(value).Append(new StringBuilder("abcde"), 1, 0).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(value => new StringBuilder(value).Append(null as char[], 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append([], 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);

        Test(
            value => new StringBuilder(value).AppendJoin(',', (IEnumerable<int>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (string[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (object[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (ReadOnlySpan<string?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (ReadOnlySpan<object?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (IEnumerable<int>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (string[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (object[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (ReadOnlySpan<string?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (ReadOnlySpan<object?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);

        Test(value => new StringBuilder(value).Insert(0, null as object).ToString(), value => new StringBuilder(value).ToString(), values);

        Test(value => new StringBuilder(value).Replace('c', 'c').ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Replace("cd", "cd").ToString(), value => new StringBuilder(value).ToString(), values);

        // other method invocation

        Test(
            value => new StringBuilder(value).AppendJoin(',', (IEnumerable<int>)[1]).ToString(),
            value => new StringBuilder(value).Append((object)1).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (IEnumerable<int>)[1]).ToString(),
            value => new StringBuilder(value).Append((object)1).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (string[])["item"]).ToString(),
            value => new StringBuilder(value).Append("item").ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (string[])["item"]).ToString(),
            value => new StringBuilder(value).Append("item").ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (object[])[1]).ToString(),
            value => new StringBuilder(value).Append((object)1).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (object[])[1]).ToString(),
            value => new StringBuilder(value).Append((object)1).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (ReadOnlySpan<string?>)["item"]).ToString(),
            value => new StringBuilder(value).Append("item").ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (ReadOnlySpan<string?>)["item"]).ToString(),
            value => new StringBuilder(value).Append("item").ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (ReadOnlySpan<object?>)[1]).ToString(),
            value => new StringBuilder(value).Append((object)1).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (ReadOnlySpan<object?>)[1]).ToString(),
            value => new StringBuilder(value).Append((object)1).ToString(),
            values);

        DoNamedTest2();
    }
}