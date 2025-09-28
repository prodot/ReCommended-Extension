using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTimeOffset;

[TestFixture]
public sealed class DateTimeOffsetAnalyzerTests : BaseTypeAnalyzerTests<System.DateTimeOffset>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTimeOffset";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantArgumentHint
                or RedundantMethodInvocationHint
                or UseBinaryOperatorSuggestion
                or UseExpressionResultSuggestion
                or UseOtherArgumentSuggestion
                or RedundantElementHint
            || highlighting.IsError();

    protected override System.DateTimeOffset[] TestValues { get; } =
    [
        System.DateTimeOffset.MinValue,
        System.DateTimeOffset.MaxValue,
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.Zero),
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(2)),
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(-6)),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Test_Constructors()
    {
        Test(
            dateTimeOffset => new System.DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                0,
                dateTimeOffset.Offset),
            dateTimeOffset => new System.DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Offset));
        Test(
            dateTimeOffset => MissingDateTimeOffsetMembers._Ctor(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                0,
                dateTimeOffset.Offset),
            dateTimeOffset => new System.DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                dateTimeOffset.Offset));
        Test<Calendar, System.DateTimeOffset>(
            (dateTimeOffset, calendar) => MissingDateTimeOffsetMembers._Ctor(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                0,
                calendar,
                dateTimeOffset.Offset),
            (dateTimeOffset, calendar) => new System.DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                calendar,
                dateTimeOffset.Offset),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            [new GregorianCalendar(), new JulianCalendar(), new JapaneseCalendar()]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestAdd()
    {
        var value = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(dateTimeOffset => dateTimeOffset.Add(value), dateTime => dateTime + value, [..TestValues.Except([System.DateTimeOffset.MaxValue])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestAddTicks()
    {
        Test(dateTimeOffset => dateTimeOffset.AddTicks(0), dateTimeOffset => dateTimeOffset);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((dateTimeOffset, other) => dateTimeOffset.Equals(other), (dateTimeOffset, other) => dateTimeOffset == other, TestValues, TestValues);
        Test(dateTimeOffset => dateTimeOffset.Equals(null), _ => false);
        Test((first, second) => System.DateTimeOffset.Equals(first, second), (first, second) => first == second, TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(
            dateTimeOffset => System.DateTimeOffset.Parse($"{dateTimeOffset}", null),
            dateTimeOffset => System.DateTimeOffset.Parse($"{dateTimeOffset}"));
        Test(
            (dateTimeOffset, formatProvider) => System.DateTimeOffset.Parse(
                dateTimeOffset.ToString(formatProvider),
                formatProvider,
                DateTimeStyles.None),
            (dateTimeOffset, formatProvider) => System.DateTimeOffset.Parse(dateTimeOffset.ToString(formatProvider), formatProvider),
            TestValues,
            FormatProviders);
        Test(
            dateTimeOffset => MissingDateTimeOffsetMembers.Parse($"{dateTimeOffset}".AsSpan(), null),
            dateTimeOffset => MissingDateTimeOffsetMembers.Parse($"{dateTimeOffset}".AsSpan()));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    public void TestParseExact()
    {
        var testValues = TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue]).ToArray();
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" };
        var invariantFormats = new[] { "o", "O", "r", "R", "s", "u" };
        var dateTimeStyles = new[]
        {
            DateTimeStyles.None,
            DateTimeStyles.AllowInnerWhite
            | DateTimeStyles.AllowLeadingWhite
            | DateTimeStyles.AllowTrailingWhite
            | DateTimeStyles.AllowWhiteSpaces,
            DateTimeStyles.AssumeLocal,
            DateTimeStyles.AssumeUniversal,
            DateTimeStyles.AdjustToUniversal,
        };

        Test(
            (dateTimeOffset, format, formatProvider) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider),
            (dateTimeOffset, format, formatProvider)
                => System.DateTimeOffset.ParseExact(dateTimeOffset.ToString(format, formatProvider), format, null),
            testValues,
            invariantFormats,
            FormatProviders);
        Test(
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider,
                styles),
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                null,
                styles),
            testValues,
            invariantFormats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, formatProvider) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider,
                DateTimeStyles.None),
            (dateTimeOffset, format, formatProvider) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider),
            testValues,
            formats,
            FormatProviders);
        Test(
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                [format],
                formatProvider,
                styles),
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider,
                styles),
            testValues,
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                formatProvider,
                styles),
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                formatProvider,
                styles),
            testValues,
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                invariantFormats,
                formatProvider,
                styles),
            (dateTimeOffset, format, formatProvider, styles) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                invariantFormats,
                null,
                styles),
            testValues,
            invariantFormats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, formatProvider, styles) => MissingDateTimeOffsetMembers.ParseExact(
                dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                formatProvider,
                styles),
            (dateTimeOffset, format, formatProvider, styles) => MissingDateTimeOffsetMembers.ParseExact(
                dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                formatProvider,
                styles),
            testValues,
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, formatProvider, styles) => MissingDateTimeOffsetMembers.ParseExact(
                dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                invariantFormats,
                formatProvider,
                styles),
            (dateTimeOffset, format, formatProvider, styles) => MissingDateTimeOffsetMembers.ParseExact(
                dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                invariantFormats,
                null,
                styles),
            testValues,
            invariantFormats,
            FormatProviders,
            dateTimeStyles);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestSubtract()
    {
        var dateTimeOffsetValue = new System.DateTimeOffset(2021, 7, 21, 13, 08, 52, System.TimeSpan.FromHours(2));
        var timeSpanValue = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(
            dateTimeOffset => dateTimeOffset.Subtract(dateTimeOffsetValue),
            dateTimeOffset => dateTimeOffset - dateTimeOffsetValue,
            [..TestValues.Except([System.DateTimeOffset.MinValue])]);
        Test(
            dateTimeOffset => dateTimeOffset.Subtract(timeSpanValue),
            dateTimeOffset => dateTimeOffset - timeSpanValue,
            [..TestValues.Except([System.DateTimeOffset.MinValue])]);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTryParse()
    {
        Test(
            (System.DateTimeOffset dateTimeOffset, out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}", null, out result),
            (System.DateTimeOffset dateTimeOffset, out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParse($"{dateTimeOffset}", out result));
        Test(
            (System.DateTimeOffset dateTimeOffset, out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), null, out result),
            (System.DateTimeOffset dateTimeOffset, out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), out result));

        Test(
            (System.DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParse($"{dateTimeOffset}", formatProvider, DateTimeStyles.None, out result),
            (System.DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}", formatProvider, out result),
            TestValues,
            FormatProviders);
        Test(
            (System.DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), formatProvider, DateTimeStyles.None, out result),
            (System.DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), formatProvider, out result),
            TestValues,
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    public void TestTryParseExact()
    {
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" };
        var invariantFormats = new[] { "o", "O", "r", "R", "s", "u" };
        var dateTimeStyles = new[]
        {
            DateTimeStyles.None,
            DateTimeStyles.AllowInnerWhite
            | DateTimeStyles.AllowLeadingWhite
            | DateTimeStyles.AllowTrailingWhite
            | DateTimeStyles.AllowWhiteSpaces,
            DateTimeStyles.AssumeLocal,
            DateTimeStyles.AssumeUniversal,
            DateTimeStyles.AdjustToUniversal,
        };

        Test(
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(dateTimeOffset.ToString(format, formatProvider), format, formatProvider, styles, out result),
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(dateTimeOffset.ToString(format, formatProvider), format, null, styles, out result),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            invariantFormats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(dateTimeOffset.ToString(format, formatProvider), [format], formatProvider, styles, out result),
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(dateTimeOffset.ToString(format, formatProvider), format, formatProvider, styles, out result),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(
                    dateTimeOffset.ToString(format, formatProvider),
                    ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                    formatProvider,
                    styles,
                    out result),
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(
                    dateTimeOffset.ToString(format, formatProvider),
                    ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                    formatProvider,
                    styles,
                    out result),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(
                    dateTimeOffset.ToString(format, formatProvider),
                    invariantFormats,
                    formatProvider,
                    styles,
                    out result),
            (System.DateTimeOffset dateTime, string format, IFormatProvider? formatProvider, DateTimeStyles styles, out System.DateTimeOffset result)
                => System.DateTimeOffset.TryParseExact(dateTime.ToString(format, formatProvider), invariantFormats, null, styles, out result),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParseExact(
                    dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                    ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                    formatProvider,
                    styles,
                    out result),
            (
                System.DateTimeOffset dateTimeOffset,
                string format,
                IFormatProvider? formatProvider,
                DateTimeStyles styles,
                out System.DateTimeOffset result) => MissingDateTimeOffsetMembers.TryParseExact(
                dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                formatProvider,
                styles,
                out result),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            FormatProviders,
            dateTimeStyles);
        Test(
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParseExact(
                    dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                    invariantFormats,
                    formatProvider,
                    styles,
                    out result),
            (
                    System.DateTimeOffset dateTimeOffset,
                    string format,
                    IFormatProvider? formatProvider,
                    DateTimeStyles styles,
                    out System.DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParseExact(
                    dateTimeOffset.ToString(format, formatProvider).AsSpan(),
                    invariantFormats,
                    null,
                    styles,
                    out result),
            [..TestValues.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            FormatProviders,
            dateTimeStyles);

        DoNamedTest2();
    }
}