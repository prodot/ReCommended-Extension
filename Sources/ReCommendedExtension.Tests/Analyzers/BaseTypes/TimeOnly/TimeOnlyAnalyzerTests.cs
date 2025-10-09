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
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or UseOtherArgumentSuggestion || highlighting.IsError();

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
    public void TestEquals()
    {
        Test((timeOnly, value) => timeOnly.Equals(value), (timeSpan, value) => timeSpan == value, TestValues, TestValues);
        Test(timeOnly => timeOnly.Equals(null), _ => false);

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
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), [format], provider, style),
            (timeOnly, format, provider, style) => Missing.TimeOnly.ParseExact(timeOnly.ToString(format, provider), format, provider, style),
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
                timeOnly.ToString(format),
                [format],
                out result),
            (Missing.TimeOnly timeOnly, string format, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
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
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), invariantFormats, provider, style, out result),
            (Missing.TimeOnly timeOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.TimeOnly result)
                => Missing.TimeOnly.TryParseExact(timeOnly.ToString(format, provider), invariantFormats, null, style, out result),
            TestValues,
            invariantFormats,
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

        DoNamedTest2();
    }
}