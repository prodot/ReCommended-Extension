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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class DecimalAnalyzerTests : BaseTypeAnalyzerTests<decimal>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Decimal";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or UseUnaryOperatorSuggestion
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override decimal[] TestValues { get; } = [0, -0.0m, 1, 2, -1, decimal.MinValue, decimal.MaxValue];

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestAdd()
    {
        Test((d1, d2) => decimal.Add(d1, d2), (d1, d2) => d1 + d2, [0, 1, -1, -0.0m], [0, 1, -1, -0.0m]);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingDecimalMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingDecimalMethods.Clamp(number, decimal.MinValue, decimal.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, decimal.MinValue, decimal.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestDivide()
    {
        Test((d1, d2) => decimal.Divide(d1, d2), (d1, d2) => d1 / d2, [0, 1, -1, -0.0m], [1, -1]);

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Decimal);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingDecimalMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingDecimalMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestMultiply()
    {
        Test((d1, d2) => decimal.Multiply(d1, d2), (d1, d2) => d1 * d2, [0, 1, -1, -0.0m], [0, 1, -1, -0.0m]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestNegate()
    {
        Test(d => decimal.Negate(d), d => -d);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => decimal.Parse($"{n}", NumberStyles.Number), n => decimal.Parse($"{n}"));
        Test(n => decimal.Parse($"{n}", null), n => decimal.Parse($"{n}"));
        Test(
            n => decimal.Parse($"{n}", NumberStyles.Number, NumberFormatInfo.InvariantInfo),
            n => decimal.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => decimal.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, null),
            n => decimal.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint));

        Test(n => MissingDecimalMethods.Parse($"{n}".AsSpan(), null), n => MissingDecimalMethods.Parse($"{n}".AsSpan()));

        Test(
            n => MissingDecimalMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingDecimalMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRemainder()
    {
        Test((d1, d2) => decimal.Remainder(d1, d2), (d1, d2) => d1 % d2, [0, 1, -1, -0.0m], [1, -1]);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore20]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        Test(n => decimal.Round(n, 0), n => decimal.Round(n));
        Test(n => decimal.Round(n, MidpointRounding.ToEven), n => decimal.Round(n));
        Test((n, mode) => decimal.Round(n, 0, mode), (n, mode) => decimal.Round(n, mode), TestValues, roundings);
        Test(
            (n, decimals) => decimal.Round(n, decimals, MidpointRounding.ToEven),
            (n, decimals) => decimal.Round(n, decimals),
            TestValues,
            digitsValues);

        Test(n => Math.Round(n, 0), n => Math.Round(n));
        Test(n => Math.Round(n, MidpointRounding.ToEven), n => Math.Round(n));
        Test((n, mode) => Math.Round(n, 0, mode), (n, mode) => Math.Round(n, mode), TestValues, roundings);
        Test((n, decimals) => Math.Round(n, decimals, MidpointRounding.ToEven), (n, decimals) => Math.Round(n, decimals), TestValues, digitsValues);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestSubtract()
    {
        Test((d1, d2) => decimal.Subtract(d1, d2), (d1, d2) => d1 - d2, [0, 1, -1, -0.0m], [0, 1, -1, -0.0m]);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString("G"), n => n.ToString());
        Test(n => n.ToString("g"), n => n.ToString());
        Test(n => n.ToString("E6"), n => n.ToString("E"));
        Test(n => n.ToString("e6"), n => n.ToString("e"));

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("C", null), n => n.ToString("C"));
        Test(n => n.ToString("G", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("E6", NumberFormatInfo.InvariantInfo), n => n.ToString("E", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("e6", NumberFormatInfo.InvariantInfo), n => n.ToString("e", NumberFormatInfo.InvariantInfo));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (decimal n, out decimal result) => decimal.TryParse($"{n}", NumberStyles.Number, NumberFormatInfo.InvariantInfo, out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}", null, out result),
            (decimal n, out decimal result) => decimal.TryParse($"{n}", out result));

        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Number,
                NumberFormatInfo.InvariantInfo,
                out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsSpan(), null, out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Number,
                NumberFormatInfo.InvariantInfo,
                out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}