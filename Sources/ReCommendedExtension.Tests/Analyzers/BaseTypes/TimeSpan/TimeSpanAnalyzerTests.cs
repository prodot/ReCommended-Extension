using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class TimeSpanAnalyzerTests : BaseTypeAnalyzerTests<System.TimeSpan>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or UseBinaryOperatorSuggestion
                or UseUnaryOperatorSuggestion
                or UseOtherArgumentSuggestion
                or RedundantElementHint
            || highlighting.IsError();

    protected override System.TimeSpan[] TestValues { get; } =
    [
        System.TimeSpan.Zero,
        System.TimeSpan.MinValue,
        System.TimeSpan.MaxValue,
        new(0, 0, 1),
        new(0, 1, 0),
        new(1, 0, 0),
        new(1, 0, 0, 0, 1),
        new(0, 0, 0, 0, 1),
        new(1, 2, 3, 4),
        new(-1, 2, 3, 4),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Test_Constructors()
    {
        Test(() => new System.TimeSpan(0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers._Ctor(0, 0, 0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 1, 2, 3), () => new System.TimeSpan(1, 2, 3));
        Test(() => new System.TimeSpan(1, 2, 3, 4, 0), () => new System.TimeSpan(1, 2, 3, 4));
        Test(() => MissingTimeSpanMembers._Ctor(1, 2, 3, 4, 5, 0), () => new System.TimeSpan(1, 2, 3, 4, 5));
        Test(() => new System.TimeSpan(long.MinValue), () => System.TimeSpan.MinValue);
        Test(() => new System.TimeSpan(long.MaxValue), () => System.TimeSpan.MaxValue);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestAdd()
    {
        var values = TestValues.Except([System.TimeSpan.MinValue, System.TimeSpan.MaxValue]).ToArray();

        Test((timeSpan, ts) => timeSpan.Add(ts), (timeSpan, ts) => timeSpan + ts, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore20]
    public void TestDivide()
    {
        Test(
            (timeSpan, divisor) => timeSpan.Divide(divisor),
            (timeSpan, divisor) => MissingTimeSpanMembers.op_Division(timeSpan, divisor),
            [..TestValues.Except([System.TimeSpan.MinValue, System.TimeSpan.MaxValue])],
            [1d, 2d, -1d, double.MaxValue, double.MinValue, double.PositiveInfinity, double.NegativeInfinity]);
        Test((timeSpan, ts) => timeSpan.Divide(ts), (timeSpan, ts) => MissingTimeSpanMembers.op_Division(timeSpan, ts), TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((timeSpan, obj) => timeSpan.Equals(obj), (timeSpan, obj) => timeSpan == obj, TestValues, TestValues);
        Test(timeSpan => timeSpan.Equals(null), _ => false);
        Test((t1, t2) => System.TimeSpan.Equals(t1, t2), (t1, t2) => t1 == t2, TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromDays()
    {
        Test(_ => MissingTimeSpanMembers.FromDays(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromDays(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromHours()
    {
        Test(_ => MissingTimeSpanMembers.FromHours(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromHours(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromMicroseconds()
    {
        Test(_ => MissingTimeSpanMembers.FromMicroseconds(0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromMilliseconds()
    {
        Test(_ => MissingTimeSpanMembers.FromMilliseconds(0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromMinutes()
    {
        Test(_ => MissingTimeSpanMembers.FromMinutes(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromMinutes(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromSeconds()
    {
        Test(_ => MissingTimeSpanMembers.FromSeconds(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromSeconds(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestFromTicks()
    {
        Test(_ => System.TimeSpan.FromTicks(0), _ => System.TimeSpan.Zero);
        Test(_ => System.TimeSpan.FromTicks(long.MinValue), _ => System.TimeSpan.MinValue);
        Test(_ => System.TimeSpan.FromTicks(long.MaxValue), _ => System.TimeSpan.MaxValue);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore20]
    public void TestMultiply()
    {
        Test(
            (timeSpan, factor) => timeSpan.Multiply(factor),
            (timeSpan, factor) => MissingTimeSpanMembers.op_Multiply(timeSpan, factor),
            [..TestValues.Except([System.TimeSpan.MinValue, System.TimeSpan.MaxValue])],
            [0d, -0d, 1d, 2d, double.Epsilon]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseUnaryOperator")]
    public void TestNegate()
    {
        Test(timeSpan => timeSpan.Negate(), timeSpan => -timeSpan, [..TestValues.Except([System.TimeSpan.MinValue])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(timeSpan => System.TimeSpan.Parse($"{timeSpan}", null), timeSpan => System.TimeSpan.Parse($"{timeSpan}"));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParseExact()
    {
        var constantFormatSpecifiers = new[] { "c", "t", "T" };
        var formatSpecifiers = new[] { "c", "t", "T", "g", "G" };
        var timeSpanStyles = new[] { TimeSpanStyles.None, TimeSpanStyles.AssumeNegative };

        Test(
            (timeSpan, format, formatProvider) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, formatProvider),
            (timeSpan, format, _) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, null),
            TestValues,
            constantFormatSpecifiers,
            FormatProviders);

        Test(
            (timeSpan, format, formatProvider) => System.TimeSpan.ParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                TimeSpanStyles.None),
            (timeSpan, format, formatProvider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, formatProvider), format, formatProvider),
            TestValues,
            formatSpecifiers,
            FormatProviders);
        Test(
            (timeSpan, format, formatProvider, styles) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, formatProvider, styles),
            (timeSpan, format, _, styles) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, null, styles),
            TestValues,
            constantFormatSpecifiers,
            FormatProviders,
            timeSpanStyles);

        Test(
            (timeSpan, format, formatProvider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, formatProvider), [format], formatProvider),
            (timeSpan, format, formatProvider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, formatProvider), format, formatProvider),
            TestValues,
            formatSpecifiers,
            FormatProviders);
        Test(
            (timeSpan, formatProvider) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "t", "T", "g", "G"], formatProvider),
            (timeSpan, formatProvider) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "g", "G"], formatProvider),
            TestValues,
            FormatProviders);

        Test(
            (timeSpan, formatProvider) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "g", "G"], formatProvider, TimeSpanStyles.None),
            (timeSpan, formatProvider) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "g", "G"], formatProvider),
            TestValues,
            FormatProviders);
        Test(
            (timeSpan, format, formatProvider, styles) => System.TimeSpan.ParseExact(
                timeSpan.ToString(format, formatProvider),
                [format],
                formatProvider,
                styles),
            (timeSpan, format, formatProvider, styles) => System.TimeSpan.ParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                styles),
            TestValues,
            formatSpecifiers,
            FormatProviders,
            timeSpanStyles);
        Test(
            (timeSpan, formatProvider, styles) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "t", "T", "g", "G"], formatProvider, styles),
            (timeSpan, formatProvider, styles) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "g", "G"], formatProvider, styles),
            TestValues,
            FormatProviders,
            timeSpanStyles);

        Test(
            (timeSpan, formatProvider, styles) => MissingTimeSpanMembers.ParseExact(
                $"{timeSpan}".AsSpan(),
                ["c", "t", "T", "g", "G"],
                formatProvider,
                styles),
            (timeSpan, formatProvider, styles) => MissingTimeSpanMembers.ParseExact($"{timeSpan}".AsSpan(), ["c", "g", "G"], formatProvider, styles),
            TestValues,
            FormatProviders,
            timeSpanStyles);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestSubtract()
    {
        var values = TestValues.Except([System.TimeSpan.MinValue, System.TimeSpan.MaxValue]).ToArray();

        Test((timeSpan, ts) => timeSpan.Subtract(ts), (timeSpan, ts) => timeSpan - ts, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestTryParse()
    {
        Test(
            (System.TimeSpan timeSpan, out System.TimeSpan result) => System.TimeSpan.TryParse($"{timeSpan}", null, out result),
            (System.TimeSpan timeSpan, out System.TimeSpan result) => System.TimeSpan.TryParse($"{timeSpan}", out result));
        Test(
            (System.TimeSpan timeSpan, out System.TimeSpan result) => MissingTimeSpanMembers.TryParse($"{timeSpan}".AsSpan(), null, out result),
            (System.TimeSpan timeSpan, out System.TimeSpan result) => MissingTimeSpanMembers.TryParse($"{timeSpan}".AsSpan(), out result));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    public void TestTryParseExact()
    {
        var constantFormatSpecifiers = new[] { "c", "t", "T" };
        var formatSpecifiers = new[] { "c", "t", "T", "g", "G" };
        var timeSpanStyles = new[] { TimeSpanStyles.None, TimeSpanStyles.AssumeNegative };

        Test(
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", format, formatProvider, out result),
            (System.TimeSpan timeSpan, string format, IFormatProvider? _, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", format, null, out result),
            TestValues,
            constantFormatSpecifiers,
            FormatProviders);

        Test(
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out System.TimeSpan result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out System.TimeSpan result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                out result),
            TestValues,
            formatSpecifiers,
            FormatProviders);
        Test(
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", format, formatProvider, styles, out result),
            (System.TimeSpan timeSpan, string format, IFormatProvider? _, TimeSpanStyles styles, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", format, null, styles, out result),
            TestValues,
            constantFormatSpecifiers,
            FormatProviders,
            timeSpanStyles);

        Test(
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out System.TimeSpan result)
                => MissingTimeSpanMembers.TryParseExact(
                    timeSpan.ToString(format, formatProvider).AsSpan(),
                    format.AsSpan(),
                    formatProvider,
                    TimeSpanStyles.None,
                    out result),
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out System.TimeSpan result)
                => MissingTimeSpanMembers.TryParseExact(
                    timeSpan.ToString(format, formatProvider).AsSpan(),
                    format.AsSpan(),
                    formatProvider,
                    out result),
            TestValues,
            formatSpecifiers,
            FormatProviders);

        Test(
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", [format], formatProvider, out result),
            (System.TimeSpan timeSpan, string format, IFormatProvider? _, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", format, null, out result),
            TestValues,
            constantFormatSpecifiers,
            FormatProviders);
        Test(
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", formatProvider),
                ["c", "t", "T", "g", "G"],
                formatProvider,
                out result),
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", formatProvider),
                ["c", "g", "G"],
                formatProvider,
                out result),
            TestValues,
            FormatProviders);

        Test(
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => System.TimeSpan.TryParseExact(
                $"{timeSpan}",
                ["c", "g", "G"],
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact($"{timeSpan}", ["c", "g", "G"], formatProvider, out result),
            TestValues,
            FormatProviders);
        Test(
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact(timeSpan.ToString(format, formatProvider), [format], formatProvider, styles, out result),
            (System.TimeSpan timeSpan, string format, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact(timeSpan.ToString(format, formatProvider), format, formatProvider, styles, out result),
            TestValues,
            formatSpecifiers,
            FormatProviders,
            timeSpanStyles);
        Test(
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact(
                    timeSpan.ToString("c", formatProvider),
                    ["c", "t", "T", "g", "G"],
                    formatProvider,
                    styles,
                    out result),
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => System.TimeSpan.TryParseExact(timeSpan.ToString("c", formatProvider), ["c", "g", "G"], formatProvider, styles, out result),
            TestValues,
            FormatProviders,
            timeSpanStyles);

        Test(
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString("c", formatProvider).AsSpan(),
                ["c", "t", "T", "g", "G"],
                formatProvider,
                out result),
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString("c", formatProvider).AsSpan(),
                ["c", "g", "G"],
                formatProvider,
                out result),
            TestValues,
            FormatProviders);

        Test(
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString("c", formatProvider).AsSpan(),
                ["c", "t", "T", "g", "G"],
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, out System.TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString("c", formatProvider).AsSpan(),
                ["c", "t", "T", "g", "G"],
                formatProvider,
                out result),
            TestValues,
            FormatProviders);
        Test(
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => MissingTimeSpanMembers.TryParseExact(
                    timeSpan.ToString("c", formatProvider).AsSpan(),
                    ["c", "t", "T", "g", "G"],
                    formatProvider,
                    styles,
                    out result),
            (System.TimeSpan timeSpan, IFormatProvider? formatProvider, TimeSpanStyles styles, out System.TimeSpan result)
                => MissingTimeSpanMembers.TryParseExact(
                    timeSpan.ToString("c", formatProvider).AsSpan(),
                    ["c", "g", "G"],
                    formatProvider,
                    styles,
                    out result),
            TestValues,
            FormatProviders,
            timeSpanStyles);

        DoNamedTest2();
    }
}