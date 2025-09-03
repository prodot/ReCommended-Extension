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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt64;

[TestFixture]
public sealed class UInt64AnalyzerTests : BaseTypeAnalyzerTests<ulong>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt64";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override ulong[] TestValues { get; } = [0, 1, 2, ulong.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUInt64Methods.Clamp(number, 1, 1), _ => 1uL);
        Test(number => MissingUInt64Methods.Clamp(number, ulong.MinValue, ulong.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1uL, 1uL), _ => 1uL);
        Test(number => MissingMathMethods.Clamp(number, ulong.MinValue, ulong.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUInt64Methods.DivRem(0, 10), () => (0ul, 0ul));

        Test(() => MissingMathMethods.DivRem(0uL, 10uL), () => (0ul, 0ul));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt64);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt64Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt64Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => ulong.Parse($"{n}", NumberStyles.Integer), n => ulong.Parse($"{n}"));
        Test(n => ulong.Parse($"{n}", null), n => ulong.Parse($"{n}"));
        Test(
            (n, provider) => ulong.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => ulong.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => ulong.Parse($"{n}", NumberStyles.None, null), n => ulong.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt64Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt64Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingUInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingUInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUInt64Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUInt64Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestToString()
    {
        var formatsRedundant = new[] { null, "", "G", "G0", "G20", "G21", "g", "g0", "g20", "g21" };
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
            (ulong n, IFormatProvider? provider, out ulong result) => ulong.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}", null, out result),
            (ulong n, out ulong result) => ulong.TryParse($"{n}", out result));

        Test(
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (ulong n, IFormatProvider? provider, out ulong result)
                => MissingUInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}