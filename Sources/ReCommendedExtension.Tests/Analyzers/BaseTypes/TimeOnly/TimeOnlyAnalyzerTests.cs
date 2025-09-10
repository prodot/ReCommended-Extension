using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeOnly;

[TestFixture]
[TestNet60]
public sealed class TimeOnlyAnalyzerTests : BaseTypeAnalyzerTests<Missing.TimeOnly>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeOnly";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or UseBinaryOperatorSuggestion
                or RedundantArgumentRangeHint
                or UseOtherArgumentSuggestion
                or RedundantElementHint
            || highlighting.IsError();

    protected override Missing.TimeOnly[] TestValues { get; } =
    [
        Missing.TimeOnly.MinValue, Missing.TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    public void Test_Constructors()
    {
        Test(() => new Missing.TimeOnly(0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0, 0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0, 0, 0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0, 0, 0, 0), () => Missing.TimeOnly.MinValue);

        DoNamedTest2();
    }

    [Test]
    public void TestAdd()
    {
        Test(
            (timeOnly, value) => timeOnly.Add(value, out _),
            (timeOnly, value) => timeOnly.Add(value),
            TestValues,
            [System.TimeSpan.FromDays(2), System.TimeSpan.FromDays(-2), System.TimeSpan.FromHours(23), System.TimeSpan.FromHours(-23)]);

        DoNamedTest2();
    }

    [Test]
    public void TestAddHours()
    {
        Test((timeOnly, value) => timeOnly.AddHours(value, out _), (timeOnly, value) => timeOnly.AddHours(value), TestValues, [48, -48, 23, -23]);

        DoNamedTest2();
    }

    [Test]
    public void TestAddMinutes()
    {
        Test(
            (timeOnly, value) => timeOnly.AddMinutes(value, out _),
            (timeOnly, value) => timeOnly.AddMinutes(value),
            TestValues,
            [48 * 60, -48 * 60, 23 * 60, -23 * 60]);

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((timeOnly, value) => timeOnly.Equals(value), (timeSpan, value) => timeSpan == value, TestValues, TestValues);
        Test(timeOnly => timeOnly.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestParse()
    {
        Test(
            dateOnly => Missing.TimeOnly.Parse(dateOnly.ToString(), null, DateTimeStyles.None),
            dateOnly => Missing.TimeOnly.Parse(dateOnly.ToString()));
        Test(dateOnly => Missing.TimeOnly.Parse(dateOnly.ToString(), null), dateOnly => Missing.TimeOnly.Parse(dateOnly.ToString()));
        Test(
            dateOnly => Missing.TimeOnly.Parse(dateOnly.ToString().AsSpan(), null),
            dateOnly => Missing.TimeOnly.Parse(dateOnly.ToString().AsSpan()));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestParseExact()
    {
        var formats = new[] { "t", "T", "o", "O", "r", "R" };
        var invariantFormats = new[] { "o", "O", "r", "R" };
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
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), format, null, DateTimeStyles.None),
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), format),
            TestValues,
            formats);
        Test(
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), format, provider, style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), format, null, style),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), [format]),
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), format),
            TestValues,
            formats);
        Test(
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), ["t", "t", "T", "o", "O", "r", "R"]),
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), ["t", "T", "o", "r"]),
            TestValues,
            formats);

        Test(
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), formats, null),
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format), formats),
            TestValues,
            formats);
        Test(
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), [format], provider, style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), format, provider, style),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider),
                ["t", "t", "T", "o", "O", "r", "R"],
                provider,
                style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider),
                ["t", "T", "o", "r"],
                provider,
                style),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider),
                invariantFormats,
                provider,
                style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), invariantFormats, null, style),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), ["t", "t", "T", "o", "O", "r", "R"]),
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), ["t", "T", "o", "r"]),
            TestValues,
            formats);

        Test(
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), formats, null),
            (timeOnly, format) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), formats),
            TestValues,
            formats);
        Test(
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                ["t", "t", "T", "o", "O", "r", "R"],
                provider,
                style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                ["t", "T", "o", "r"],
                provider,
                style),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                invariantFormats,
                provider,
                style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                invariantFormats,
                null,
                style),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        Test((timeOnly, format) => timeOnly.ToString(format), (timeOnly, _) => timeOnly.ToString(), TestValues, [null, "", "t"]);

        Test(timeOnly => timeOnly.ToString(null as IFormatProvider), timeOnly => timeOnly.ToString(), TestValues);

        Test(
            (timeOnly, format) => timeOnly.ToString(format, null),
            (timeOnly, format) => timeOnly.ToString(format),
            TestValues,
            [null, "", "t", "T", "o", "O", "r", "R"]);
        Test(
            (timeOnly, provider) => timeOnly.ToString(null, provider),
            (timeOnly, provider) => timeOnly.ToString(provider),
            TestValues,
            FormatProviders);
        Test(
            (timeOnly, format, provider) => timeOnly.ToString(format, provider),
            (timeOnly, _, provider) => timeOnly.ToString(provider),
            TestValues,
            [null, "", "t"],
            FormatProviders);
        Test(
            (timeOnly, format, provider) => timeOnly.ToString(format, provider),
            (timeOnly, format, _) => timeOnly.ToString(format),
            TestValues,
            ["o", "O", "r", "R"],
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTryParse()
    {
        Test(
            (Missing.TimeOnly timeOnly, IFormatProvider? provider, out Missing.TimeOnly result) => Missing.TimeOnly.TryParse(
                timeOnly.ToString(),
                provider,
                DateTimeStyles.None,
                out result),
            (Missing.TimeOnly timeOnly, IFormatProvider? provider, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParse(timeOnly.ToString(), provider, out result),
            TestValues,
            FormatProviders);

        Test(
            (Missing.TimeOnly timeOnly, IFormatProvider? provider, out Missing.TimeOnly result) => Missing.TimeOnly.TryParse(
                timeOnly.ToString().AsSpan(),
                provider,
                DateTimeStyles.None,
                out result),
            (Missing.TimeOnly timeOnly, IFormatProvider? provider, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParse(timeOnly.ToString().AsSpan(), provider, out result),
            TestValues,
            FormatProviders);

        Test(
            (Missing.TimeOnly timeOnly, out Missing.TimeOnly result) => Missing.TimeOnly.TryParse(timeOnly.ToString(), null, out result),
            (Missing.TimeOnly timeOnly, out Missing.TimeOnly result) => Missing.TimeOnly.TryParse(timeOnly.ToString(), out result));

        Test(
            (Missing.TimeOnly timeOnly, out Missing.TimeOnly result) => Missing.TimeOnly.TryParse(timeOnly.ToString().AsSpan(), null, out result),
            (Missing.TimeOnly timeOnly, out Missing.TimeOnly result) => Missing.TimeOnly.TryParse(timeOnly.ToString().AsSpan(), out result));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestTryParseExact()
    {
        var formats = new[] { "t", "T", "o", "O", "r", "R" };
        var invariantFormats = new[] { "o", "O", "r", "R" };
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
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                format,
                null,
                DateTimeStyles.None,
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
            TestValues,
            formats);
        Test(
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format), format, provider, style, out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? _, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format), format, null, style, out result),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                format.AsSpan(),
                null,
                DateTimeStyles.None,
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                format.AsSpan(),
                out result),
            TestValues,
            formats);

        Test(
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                [format],
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
            TestValues,
            formats);
        Test(
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                ["t", "t", "T", "o", "O", "r", "R"],
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                ["t", "T", "o", "r"],
                out result),
            TestValues,
            formats);

        Test(
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                ["t", "t", "T", "o", "O", "r", "R"],
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                ["t", "T", "o", "r"],
                out result),
            TestValues,
            formats);

        Test(
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), [format], provider, style, out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), format, provider, style, out result),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(
                    timeOnly.ToString(format, provider),
                    ["t", "t", "T", "o", "O", "r", "R"],
                    provider,
                    style,
                    out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), ["t", "T", "o", "r"], provider, style, out result),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), invariantFormats, provider, style, out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), invariantFormats, null, style, out result),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);
        Test(
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
            TestValues,
            formats);

        Test(
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(
                    timeOnly.ToString(format, provider).AsSpan(),
                    ["t", "t", "T", "o", "O", "r", "R"],
                    provider,
                    style,
                    out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider).AsSpan(), ["t", "T", "o", "r"], provider, style, out result),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider).AsSpan(), invariantFormats, provider, style, out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider).AsSpan(), invariantFormats, null, style, out result),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);
        Test(
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result) => Missing.TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format).AsSpan(), formats, out result),
            TestValues,
            formats);

        DoNamedTest2();
    }
}