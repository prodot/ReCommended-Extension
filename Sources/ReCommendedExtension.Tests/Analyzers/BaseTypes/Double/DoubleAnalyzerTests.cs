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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class DoubleAnalyzerTests : BaseTypeAnalyzerTests<double>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or UseFloatingPointPatternSuggestion
                or PassOtherFormatSpecifierSuggestion
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override double[] TestValues { get; } =
    [
        0d, -0d, 1d, 2d, double.MaxValue, double.MinValue, double.Epsilon, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
    ];

    [Test]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Double);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNaN()
    {
        Test(number => double.IsNaN(number), number => number is double.NaN);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        var values = new[]
        {
            0d, -0d, 1d, 2d, float.MinValue, float.MaxValue, double.Epsilon, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
        };

        Test(n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => double.Parse($"{n}"), values);
        Test(n => double.Parse($"{n}", null), n => double.Parse($"{n}"), values);
        Test(
            n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en")),
            n => double.Parse($"{n}", new CultureInfo("en")),
            values);
        Test(
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint),
            values);

        Test(n => MissingDoubleMethods.Parse($"{n}".AsSpan(), null), n => MissingDoubleMethods.Parse($"{n}".AsSpan()), values);

        Test(
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")),
            values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        var xValues = new[] { 0, -0d, double.MaxValue, double.MinValue };
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        Test(n => MissingDoubleMethods.Round(n, 0), n => MissingDoubleMethods.Round(n));
        Test(n => MissingDoubleMethods.Round(n, MidpointRounding.ToEven), n => MissingDoubleMethods.Round(n));
        Test((n, mode) => MissingDoubleMethods.Round(n, 0, mode), (n, mode) => MissingDoubleMethods.Round(n, mode), xValues, roundings);
        Test(
            (n, digits) => MissingDoubleMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingDoubleMethods.Round(n, digits),
            xValues,
            digitsValues);

        Test(n => Math.Round(n, 0), n => Math.Round(n));
        Test(n => Math.Round(n, MidpointRounding.ToEven), n => Math.Round(n));
        Test((n, mode) => Math.Round(n, 0, mode), (n, mode) => Math.Round(n, mode), xValues, roundings);
        Test((n, digits) => Math.Round(n, digits, MidpointRounding.ToEven), (n, digits) => Math.Round(n, digits), xValues, digitsValues);

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
        Test(n => n.ToString("G0"), n => n.ToString());
        Test(n => n.ToString("E6"), n => n.ToString("E"));
        Test(n => n.ToString("e6"), n => n.ToString("e"));

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("F", null), n => n.ToString("F"));
        Test(n => n.ToString("G", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("E6", NumberFormatInfo.InvariantInfo), n => n.ToString("E", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("e6", NumberFormatInfo.InvariantInfo), n => n.ToString("e", NumberFormatInfo.InvariantInfo));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        var values = new[]
        {
            0d, -0d, 1d, 2d, float.MinValue, float.MaxValue, double.Epsilon, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
        };

        Test(
            (double n, out double result) => double.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}", new CultureInfo("en"), out result),
            values);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}", null, out result),
            (double n, out double result) => double.TryParse($"{n}", out result),
            values);

        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), new CultureInfo("en"), out result),
            values);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), out result),
            values);

        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), new CultureInfo("en"), out result),
            values);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result),
            values);

        DoNamedTest2();
    }
}