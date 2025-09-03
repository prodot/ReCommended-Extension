using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int64;

[TestFixture]
public sealed class Int64AnalyzerTests : BaseTypeAnalyzerTests<long>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int64";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override long[] TestValues { get; } = [0, 1, 2, -1, -2, long.MinValue, long.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingInt64Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingInt64Methods.Clamp(number, long.MinValue, long.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, long.MinValue, long.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingInt64Methods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem(0L, 10L), () => (0, 0));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Int64);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => MissingInt64Methods.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => MissingInt64Methods.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingInt64Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMaxMagnitude()
    {
        Test(n => MissingInt64Methods.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingInt64Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMinMagnitude()
    {
        Test(n => MissingInt64Methods.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => long.Parse($"{n}", NumberStyles.Integer), n => long.Parse($"{n}"));
        Test(n => long.Parse($"{n}", null), n => long.Parse($"{n}"));
        Test(
            (n, provider) => long.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => long.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => long.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => long.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingInt64Methods.Parse($"{n}".AsSpan(), null), n => MissingInt64Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingInt64Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingInt64Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestToString()
    {
        var formatsRedundant = new[] { null, "", "G", "G0", "G19", "G20", "g", "g0", "g19", "g20" };
        var formatsRedundantSpecifier = new[] { "E6", "e6", "D0", "D1", "d0", "d1" };
        var formatsRedundantProvider = new[] { "X2", "x2" };
        var formatsRedundantSpecifierAndProvider = new[] { "X0", "X1", "x0", "x1" };

        Test((n, format) => n.ToString(format), (n, _) => n.ToString(), TestValues, formatsRedundant);
        Test(
            (n, format) => n.ToString(format),
            (n, format) => n.ToString($"{format[0]}"),
            TestValues,
            [..formatsRedundantSpecifier, ..formatsRedundantSpecifierAndProvider]);

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());

        Test(
            (n, format) => n.ToString(format, null),
            (n, format) => n.ToString(format),
            TestValues,
            [..formatsRedundant, ..formatsRedundantSpecifier, ..formatsRedundantProvider, ..formatsRedundantSpecifierAndProvider]);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, _, provider) => n.ToString(provider),
            TestValues,
            formatsRedundant,
            FormatProviders);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, format, provider) => n.ToString($"{format[0]}", provider),
            TestValues,
            formatsRedundantSpecifier,
            FormatProviders);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, format, _) => n.ToString($"{format[0]}"),
            TestValues,
            formatsRedundantSpecifierAndProvider,
            FormatProviders);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, format, _) => n.ToString(format),
            TestValues,
            formatsRedundantProvider,
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (long n, IFormatProvider? provider, out long result) => long.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}", null, out result),
            (long n, out long result) => long.TryParse($"{n}", out result));

        Test(
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), null, out result),
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (long n, IFormatProvider? provider, out long result)
                => MissingInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (long n, out long result) => MissingInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}