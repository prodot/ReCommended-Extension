using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class DoubleAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or UseFloatingPointPatternSuggestion || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<double, R> expected, Func<double, R> actual, double minValue = double.MinValue, double maxValue = double.MaxValue)
    {
        Assert.AreEqual(expected(0d), actual(0d));
        Assert.AreEqual(expected(-0d), actual(-0d));
        Assert.AreEqual(expected(minValue), actual(minValue));
        Assert.AreEqual(expected(maxValue), actual(maxValue));
        Assert.AreEqual(expected(double.Epsilon), actual(double.Epsilon));
        Assert.AreEqual(expected(double.NaN), actual(double.NaN));
        Assert.AreEqual(expected(double.PositiveInfinity), actual(double.PositiveInfinity));
        Assert.AreEqual(expected(double.NegativeInfinity), actual(double.NegativeInfinity));
    }

    static void Test<R>(Func<double, double, R> expected, Func<double, double, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, double.MaxValue), actual(0, double.MaxValue));
        Assert.AreEqual(expected(double.MinValue, 0), actual(double.MinValue, 0));
        Assert.AreEqual(expected(double.MinValue, double.MinValue), actual(double.MinValue, double.MinValue));
        Assert.AreEqual(expected(double.MaxValue, double.MaxValue), actual(double.MaxValue, double.MaxValue));
        Assert.AreEqual(expected(double.MinValue, double.MaxValue), actual(double.MinValue, double.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(
        FuncWithOut<double, double, bool> expected,
        FuncWithOut<double, double, bool> actual,
        double minValue = double.MinValue,
        double maxValue = double.MaxValue)
    {
        Assert.AreEqual(expected(0d, out var expectedResult), actual(0d, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(-0d, out expectedResult), actual(-0d, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(maxValue, out expectedResult), actual(maxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(minValue, out expectedResult), actual(minValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    static void Test(Func<double, MidpointRounding, double> expected, Func<double, MidpointRounding, double> actual)
    {
        Assert.AreEqual(expected(0, MidpointRounding.ToEven), actual(0, MidpointRounding.ToEven));
        Assert.AreEqual(expected(0, MidpointRounding.AwayFromZero), actual(0, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(-0d, MidpointRounding.ToEven), actual(-0d, MidpointRounding.ToEven));
        Assert.AreEqual(expected(-0d, MidpointRounding.AwayFromZero), actual(-0d, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(double.MaxValue, MidpointRounding.ToEven), actual(double.MaxValue, MidpointRounding.ToEven));
        Assert.AreEqual(expected(double.MaxValue, MidpointRounding.AwayFromZero), actual(double.MaxValue, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(double.MinValue, MidpointRounding.ToEven), actual(double.MinValue, MidpointRounding.ToEven));
        Assert.AreEqual(expected(double.MinValue, MidpointRounding.AwayFromZero), actual(double.MinValue, MidpointRounding.AwayFromZero));
    }

    static void Test(Func<double, int, double> expected, Func<double, int, double> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, 1), actual(0, 1));
        Assert.AreEqual(expected(0, 2), actual(0, 2));

        Assert.AreEqual(expected(-0d, 0), actual(-0d, 0));
        Assert.AreEqual(expected(-0d, 1), actual(-0d, 1));
        Assert.AreEqual(expected(-0d, 2), actual(-0d, 2));

        Assert.AreEqual(expected(double.MaxValue, 0), actual(double.MaxValue, 0));
        Assert.AreEqual(expected(double.MaxValue, 1), actual(double.MaxValue, 1));
        Assert.AreEqual(expected(double.MaxValue, 2), actual(double.MaxValue, 2));

        Assert.AreEqual(expected(double.MinValue, 0), actual(double.MinValue, 0));
        Assert.AreEqual(expected(double.MinValue, 1), actual(double.MinValue, 1));
        Assert.AreEqual(expected(double.MinValue, 2), actual(double.MinValue, 2));
    }

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
        Test(n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => double.Parse($"{n}"), float.MinValue, float.MaxValue);
        Test(n => double.Parse($"{n}", null), n => double.Parse($"{n}"), float.MinValue, float.MaxValue);
        Test(
            n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en")),
            n => double.Parse($"{n}", new CultureInfo("en")),
            float.MinValue,
            float.MaxValue);
        Test(
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint),
            float.MinValue,
            float.MaxValue);

        Test(
            n => MissingDoubleMethods.Parse($"{n}".AsSpan(), null),
            n => MissingDoubleMethods.Parse($"{n}".AsSpan()),
            float.MinValue,
            float.MaxValue);

        Test(
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")),
            float.MinValue,
            float.MaxValue);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        Test(n => MissingDoubleMethods.Round(n, 0), n => MissingDoubleMethods.Round(n));
        Test(n => MissingDoubleMethods.Round(n, MidpointRounding.ToEven), n => MissingDoubleMethods.Round(n));
        Test((n, mode) => MissingDoubleMethods.Round(n, 0, mode), (n, mode) => MissingDoubleMethods.Round(n, mode));
        Test((n, digits) => MissingDoubleMethods.Round(n, digits, MidpointRounding.ToEven), (n, digits) => MissingDoubleMethods.Round(n, digits));

        Test(n => Math.Round(n, 0), n => Math.Round(n));
        Test(n => Math.Round(n, MidpointRounding.ToEven), n => Math.Round(n));
        Test((n, mode) => Math.Round(n, 0, mode), (n, mode) => Math.Round(n, mode));
        Test((n, digits) => Math.Round(n, digits, MidpointRounding.ToEven), (n, digits) => Math.Round(n, digits));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("F", null), n => n.ToString("F"));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (double n, out double result) => double.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}", new CultureInfo("en"), out result),
            float.MinValue,
            float.MaxValue);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}", null, out result),
            (double n, out double result) => double.TryParse($"{n}", out result),
            float.MinValue,
            float.MaxValue);

        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), new CultureInfo("en"), out result),
            float.MinValue,
            float.MaxValue);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), out result),
            float.MinValue,
            float.MaxValue);

        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), new CultureInfo("en"), out result),
            float.MinValue,
            float.MaxValue);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result),
            float.MinValue,
            float.MaxValue);

        DoNamedTest2();
    }
}