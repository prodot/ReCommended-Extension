using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateOnly;

[TestFixture]
[TestNet60]
public sealed class DateOnlyAnalyzerTests : BaseTypeAnalyzerTests<Missing.DateOnly>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateOnly";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantMethodInvocationHint or UseBinaryOperatorSuggestion or UseExpressionResultSuggestion or UseOtherArgumentSuggestion
            || highlighting.IsError();

    protected override Missing.DateOnly[] TestValues { get; } = [Missing.DateOnly.MinValue, Missing.DateOnly.MaxValue, new(2025, 7, 15)];

    [Test]
    public void TestAddDays()
    {
        Test(dateOnly => dateOnly.AddDays(0), dateOnly => dateOnly);

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((dateOnly, value) => dateOnly.Equals(value), (dateOnly, value) => dateOnly == value, TestValues, TestValues);

        Test(dateOnly => dateOnly.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestParseExact()
    {
        var formats = new[] { "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" };
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
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(dateOnly.ToString(format, provider), format, provider, style),
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(dateOnly.ToString(format, provider), format, null, style),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (dateOnly, format) => Missing.DateOnly.ParseExact(dateOnly.ToString(format), [format]),
            (dateOnly, format) => Missing.DateOnly.ParseExact(dateOnly.ToString(format), format),
            TestValues,
            formats);

        Test(
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(dateOnly.ToString(format, provider), [format], provider, style),
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(dateOnly.ToString(format, provider), format, provider, style),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(
                dateOnly.ToString(format, provider),
                invariantFormats,
                provider,
                style),
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(dateOnly.ToString(format, provider), invariantFormats, null, style),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                invariantFormats,
                provider,
                style),
            (dateOnly, format, provider, style) => Missing.DateOnly.ParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
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
        var formats = new[] { "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" };
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
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format), format, provider, style, out result),
            (Missing.DateOnly dateOnly, string format, IFormatProvider? _, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format), format, null, style, out result),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (Missing.DateOnly dateOnly, string format, out Missing.DateOnly result) => Missing.DateOnly.TryParseExact(
                dateOnly.ToString(format),
                [format],
                out result),
            (Missing.DateOnly dateOnly, string format, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format), format, out result),
            TestValues,
            formats);

        Test(
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format, provider), [format], provider, style, out result),
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format, provider), format, provider, style, out result),
            TestValues,
            formats,
            FormatProviders,
            styles);
        Test(
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format, provider), invariantFormats, provider, style, out result),
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format, provider), invariantFormats, null, style, out result),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        Test(
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format, provider).AsSpan(), invariantFormats, provider, style, out result),
            (Missing.DateOnly dateOnly, string format, IFormatProvider? provider, DateTimeStyles style, out Missing.DateOnly result)
                => Missing.DateOnly.TryParseExact(dateOnly.ToString(format, provider).AsSpan(), invariantFormats, null, style, out result),
            TestValues,
            invariantFormats,
            FormatProviders,
            styles);

        DoNamedTest2();
    }
}