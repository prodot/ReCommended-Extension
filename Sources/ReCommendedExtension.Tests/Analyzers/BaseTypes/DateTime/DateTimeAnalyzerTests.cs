using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
public sealed class DateTimeAnalyzerTests : BaseTypeAnalyzerTests<System.DateTime>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or RedundantArgumentRangeHint
                or UseDateTimePropertySuggestion
                or UseBinaryOperatorSuggestion
                or UseOtherArgumentSuggestion
                or RedundantElementHint
                or RedundantMethodInvocationHint
            || highlighting.IsError();

    protected override System.DateTime[] TestValues { get; } =
    [
        System.DateTime.MinValue,
        System.DateTime.MaxValue,
        new(2025, 7, 15, 21, 33, 0, 123),
        new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
        new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet80]
    public void Test_Constructors()
    {
        var calendars = new Calendar[] { new GregorianCalendar(), new JulianCalendar(), new JapaneseCalendar() };

        Test(() => new System.DateTime(0), () => System.DateTime.MinValue);

        Test(() => new System.DateTime(0, DateTimeKind.Unspecified), () => new System.DateTime(0));
        Test(() => new System.DateTime(1, DateTimeKind.Unspecified), () => new System.DateTime(1));
        Test(() => new System.DateTime(638_882_119_800_000_000, DateTimeKind.Unspecified), () => new System.DateTime(638_882_119_800_000_000));

        Test(
            dateTime => MissingDateTimeMembers._Ctor(
                Missing.DateOnly.FromDateTime(dateTime),
                TimeOnly.FromDateTime(dateTime),
                DateTimeKind.Unspecified),
            dateTime => MissingDateTimeMembers._Ctor(Missing.DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime)));

        Test(
            dateTime => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0),
            dateTime => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day));

        Test(
            dateTime => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                DateTimeKind.Unspecified),
            dateTime => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second));

        Test(
            (dateTime, calendar) => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, calendar),
            (dateTime, calendar) => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, calendar),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        Test(
            dateTime => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0),
            dateTime => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second));

        Test(
            dateTime => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                DateTimeKind.Unspecified),
            dateTime => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond));
        Test(
            dateTime => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                0,
                dateTime.Kind),
            dateTime => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Kind));

        Test(
            (dateTime, calendar) => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                0,
                calendar),
            (dateTime, calendar) => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                calendar),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        Test(
            (dateTime, calendar) => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar,
                DateTimeKind.Unspecified),
            (dateTime, calendar) => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        Test(
            dateTime => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond(),
                DateTimeKind.Unspecified),
            dateTime => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond()));
        Test(
            dateTime => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                0,
                dateTime.Kind),
            dateTime => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Kind));

        Test(
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                0,
                calendar),
            (dateTime, calendar) => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        Test(
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond(),
                calendar,
                DateTimeKind.Unspecified),
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond(),
                calendar),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);
        Test(
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                0,
                calendar,
                dateTime.Kind),
            (dateTime, calendar) => new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar,
                dateTime.Kind),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
    public void TestDate() => DoNamedTest2();

    [Test]
    public void TestAdd()
    {
        var value = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(dateTime => dateTime.Add(value), dateTime => dateTime + value, [..TestValues.Except([System.DateTime.MaxValue])]);

        DoNamedTest2();
    }

    [Test]
    public void TestAddTicks()
    {
        Test(dateTime => dateTime.AddTicks(0), dateTime => dateTime);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestEquals()
    {
        Test((dateTime, value) => dateTime.Equals(value), (dateTime, value) => dateTime == value, TestValues, TestValues);
        Test(dateTime => dateTime.Equals(null), _ => false);
        Test((t1, t2) => System.DateTime.Equals(t1, t2), (t1, t2) => t1 == t2, TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    public void TestGetDateTimeFormats()
    {
        Test(dateTime => dateTime.GetDateTimeFormats(null), dateTime => dateTime.GetDateTimeFormats());
        Test(
            (dateTime, format) => dateTime.GetDateTimeFormats(format, null),
            (dateTime, format) => dateTime.GetDateTimeFormats(format),
            TestValues,
            ['d', 'D', 'f', 'F', 'g', 'G', 'm', 'M', 'o', 'O', 'r', 'R', 's', 't', 'T', 'u', 'U', 'y', 'Y']);

        DoNamedTest2();
    }

    [Test]
    public void TestGetTypeCode()
    {
        Test(dateTime => dateTime.GetTypeCode(), _ => TypeCode.DateTime);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestParse()
    {
        Test(dateTime => System.DateTime.Parse($"{dateTime}", null), dateTime => System.DateTime.Parse($"{dateTime}"));
        Test(
            (dateTime, provider) => System.DateTime.Parse(dateTime.ToString(provider), provider, DateTimeStyles.None),
            (dateTime, provider) => System.DateTime.Parse(dateTime.ToString(provider), provider),
            TestValues,
            FormatProviders);
        Test(
            dateTime => MissingDateTimeMembers.Parse($"{dateTime}".AsSpan(), null),
            dateTime => MissingDateTimeMembers.Parse($"{dateTime}".AsSpan()));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestParseExact()
    {
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" };
        var invariantFormats = new[] { "o", "O", "r", "R", "s", "u" };
        var styles = new[]
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
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider),
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, null),
            TestValues,
            invariantFormats,
            FormatProviders);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, null, style),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            invariantFormats,
            FormatProviders,
            styles);
        Test(
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, DateTimeStyles.None),
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider),
            TestValues,
            formats,
            FormatProviders);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), [format], provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, style),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider),
                ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                provider,
                style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                provider,
                style),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), invariantFormats, provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), invariantFormats, null, style),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            invariantFormats,
            FormatProviders,
            styles);
        Test(
            (dateTime, format, provider, style) => MissingDateTimeMembers.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                provider,
                style),
            (dateTime, format, provider, style) => MissingDateTimeMembers.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                provider,
                style),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (dateTime, format, provider, style) => MissingDateTimeMembers.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                invariantFormats,
                provider,
                style),
            (dateTime, format, provider, style) => MissingDateTimeMembers.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                invariantFormats,
                null,
                style),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            invariantFormats,
            FormatProviders,
            styles);

        DoNamedTest2();
    }

    [Test]
    public void TestSubtract()
    {
        var dateTimeValue = new System.DateTime(2021, 7, 21);
        var timeSpanValue = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(dateTime => dateTime.Subtract(dateTimeValue), dateTime => dateTime - dateTimeValue, [..TestValues.Except([System.DateTime.MinValue])]);
        Test(dateTime => dateTime.Subtract(timeSpanValue), dateTime => dateTime - timeSpanValue, [..TestValues.Except([System.DateTime.MinValue])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public void TestToString()
    {
        Test((dateTime, format) => dateTime.ToString(format), (dateTime, _) => dateTime.ToString(), TestValues, [null, ""]);

        Test(dateTime => dateTime.ToString(null as IFormatProvider), dateTime => dateTime.ToString());

        Test(
            (dateTime, format, provider) => dateTime.ToString(format, provider),
            (dateTime, _, provider) => dateTime.ToString(provider),
            TestValues,
            [null, ""],
            FormatProviders);
        Test(
            (dateTime, format) => dateTime.ToString(format, null),
            (dateTime, format) => dateTime.ToString(format),
            TestValues,
            ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"]);
        Test(
            (dateTime, format, provider) => dateTime.ToString(format, provider),
            (dateTime, format, _) => dateTime.ToString(format),
            TestValues,
            ["o", "O", "r", "R", "s", "u"],
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTryParse()
    {
        Test(
            (System.DateTime dateTime, out System.DateTime result) => MissingDateTimeMembers.TryParse($"{dateTime}", null, out result),
            (System.DateTime dateTime, out System.DateTime result) => System.DateTime.TryParse($"{dateTime}", out result));
        Test(
            (System.DateTime dateTime, out System.DateTime result) => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), null, out result),
            (System.DateTime dateTime, out System.DateTime result) => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), out result));

        Test(
            (System.DateTime dateTime, IFormatProvider? provider, out System.DateTime result)
                => System.DateTime.TryParse($"{dateTime}", provider, DateTimeStyles.None, out result),
            (System.DateTime dateTime, IFormatProvider? provider, out System.DateTime result)
                => MissingDateTimeMembers.TryParse($"{dateTime}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (System.DateTime dateTime, IFormatProvider? provider, out System.DateTime result)
                => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), provider, DateTimeStyles.None, out result),
            (System.DateTime dateTime, IFormatProvider? provider, out System.DateTime result)
                => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    public void TestTryParseExact()
    {
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" };
        var invariantFormats = new[] { "o", "O", "r", "R", "s", "u" };
        var styles = new[]
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
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), format, provider, style, out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), format, null, style, out result),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            invariantFormats,
            FormatProviders,
            styles);
        Test(
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), [format], provider, style, out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), format, provider, style, out result),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(
                    dateTime.ToString(format, provider),
                    ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                    provider,
                    style,
                    out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(
                    dateTime.ToString(format, provider),
                    ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                    provider,
                    style,
                    out result),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), invariantFormats, provider, style, out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), invariantFormats, null, style, out result),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => MissingDateTimeMembers.TryParseExact(
                    dateTime.ToString(format, provider).AsSpan(),
                    ["d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                    provider,
                    style,
                    out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => MissingDateTimeMembers.TryParseExact(
                    dateTime.ToString(format, provider).AsSpan(),
                    ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                    provider,
                    style,
                    out result),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);
        Test(
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => MissingDateTimeMembers.TryParseExact(dateTime.ToString(format, provider).AsSpan(), invariantFormats, provider, style, out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => MissingDateTimeMembers.TryParseExact(dateTime.ToString(format, provider).AsSpan(), invariantFormats, null, style, out result),
            [..TestValues.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            FormatProviders,
            styles);

        DoNamedTest2();
    }
}