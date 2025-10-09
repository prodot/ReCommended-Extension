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
                or UseDateTimePropertySuggestion
                or UseBinaryOperatorSuggestion
                or UseOtherArgumentSuggestion
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
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantArgumentRange")]
    public void Test_Constructors()
    {
        Test(() => new System.DateTime(0), () => System.DateTime.MinValue);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
    public void TestDate() => DoNamedTest2();

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestAdd()
    {
        var value = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(dateTime => dateTime.Add(value), dateTime => dateTime + value, [..TestValues.Except([System.DateTime.MaxValue])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestAddTicks()
    {
        Test(dateTime => dateTime.AddTicks(0), dateTime => dateTime);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((dateTime, value) => dateTime.Equals(value), (dateTime, value) => dateTime == value, TestValues, TestValues);
        Test(dateTime => dateTime.Equals(null), _ => false);
        Test((t1, t2) => System.DateTime.Equals(t1, t2), (t1, t2) => t1 == t2, TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(dateTime => dateTime.GetTypeCode(), _ => TypeCode.DateTime);

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
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), [format], provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, style),
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
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestSubtract()
    {
        var dateTimeValue = new System.DateTime(2021, 7, 21);
        var timeSpanValue = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(dateTime => dateTime.Subtract(dateTimeValue), dateTime => dateTime - dateTimeValue, [..TestValues.Except([System.DateTime.MinValue])]);
        Test(dateTime => dateTime.Subtract(timeSpanValue), dateTime => dateTime - timeSpanValue, [..TestValues.Except([System.DateTime.MinValue])]);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
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
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), invariantFormats, provider, style, out result),
            (System.DateTime dateTime, string format, IFormatProvider? provider, DateTimeStyles style, out System.DateTime result)
                => System.DateTime.TryParseExact(dateTime.ToString(format, provider), invariantFormats, null, style, out result),
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