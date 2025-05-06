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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Single;

[TestFixture]
public sealed class SingleAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Single";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or UseFloatingPointPatternSuggestion
                or PassOtherFormatSpecifierSuggestion
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    static void Test<R>(Func<float, R> expected, Func<float, R> actual)
    {
        Assert.AreEqual(expected(0f), actual(0f));
        Assert.AreEqual(expected(-0f), actual(-0f));
        Assert.AreEqual(expected(float.MinValue), actual(float.MinValue));
        Assert.AreEqual(expected(float.MaxValue), actual(float.MaxValue));
        Assert.AreEqual(expected(float.Epsilon), actual(float.Epsilon));
        Assert.AreEqual(expected(float.NaN), actual(float.NaN));
        Assert.AreEqual(expected(float.PositiveInfinity), actual(float.PositiveInfinity));
        Assert.AreEqual(expected(float.NegativeInfinity), actual(float.NegativeInfinity));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<float, float, bool> expected, FuncWithOut<float, float, bool> actual)
    {
        Assert.AreEqual(expected(0f, out var expectedResult), actual(0f, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(-0f, out expectedResult), actual(-0f, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(float.MaxValue, out expectedResult), actual(float.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(float.MinValue, out expectedResult), actual(float.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    static void Test(Func<float, MidpointRounding, float> expected, Func<float, MidpointRounding, float> actual)
    {
        Assert.AreEqual(expected(0, MidpointRounding.ToEven), actual(0, MidpointRounding.ToEven));
        Assert.AreEqual(expected(0, MidpointRounding.AwayFromZero), actual(0, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(-0f, MidpointRounding.ToEven), actual(-0f, MidpointRounding.ToEven));
        Assert.AreEqual(expected(-0f, MidpointRounding.AwayFromZero), actual(-0f, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(float.MaxValue, MidpointRounding.ToEven), actual(float.MaxValue, MidpointRounding.ToEven));
        Assert.AreEqual(expected(float.MaxValue, MidpointRounding.AwayFromZero), actual(float.MaxValue, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(float.MinValue, MidpointRounding.ToEven), actual(float.MinValue, MidpointRounding.ToEven));
        Assert.AreEqual(expected(float.MinValue, MidpointRounding.AwayFromZero), actual(float.MinValue, MidpointRounding.AwayFromZero));
    }

    static void Test(Func<float, int, float> expected, Func<float, int, float> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, 1), actual(0, 1));
        Assert.AreEqual(expected(0, 2), actual(0, 2));

        Assert.AreEqual(expected(-0f, 0), actual(-0f, 0));
        Assert.AreEqual(expected(-0f, 1), actual(-0f, 1));
        Assert.AreEqual(expected(-0f, 2), actual(-0f, 2));

        Assert.AreEqual(expected(float.MaxValue, 0), actual(float.MaxValue, 0));
        Assert.AreEqual(expected(float.MaxValue, 1), actual(float.MaxValue, 1));
        Assert.AreEqual(expected(float.MaxValue, 2), actual(float.MaxValue, 2));

        Assert.AreEqual(expected(float.MinValue, 0), actual(float.MinValue, 0));
        Assert.AreEqual(expected(float.MinValue, 1), actual(float.MinValue, 1));
        Assert.AreEqual(expected(float.MinValue, 2), actual(float.MinValue, 2));
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
        Test(number => number.GetTypeCode(), _ => TypeCode.Single);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNaN()
    {
        Test(number => float.IsNaN(number), number => number is float.NaN);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => float.Parse($"{n}"));
        Test(n => float.Parse($"{n}", null), n => float.Parse($"{n}"));
        Test(
            n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en")),
            n => float.Parse($"{n}", new CultureInfo("en")));
        Test(
            n => float.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => float.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));

        Test(n => MissingSingleMethods.Parse($"{n}".AsSpan(), null), n => MissingSingleMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingSingleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingSingleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        Test(n => MissingSingleMethods.Round(n, 0), n => MissingSingleMethods.Round(n));
        Test(n => MissingSingleMethods.Round(n, MidpointRounding.ToEven), n => MissingSingleMethods.Round(n));
        Test((n, mode) => MissingSingleMethods.Round(n, 0, mode), (n, mode) => MissingSingleMethods.Round(n, mode));
        Test((n, digits) => MissingSingleMethods.Round(n, digits, MidpointRounding.ToEven), (n, digits) => MissingSingleMethods.Round(n, digits));

        Test(n => MissingMathFMethods.Round(n, 0), n => MissingMathFMethods.Round(n));
        Test(n => MissingMathFMethods.Round(n, MidpointRounding.ToEven), n => MissingMathFMethods.Round(n));
        Test((n, mode) => MissingMathFMethods.Round(n, 0, mode), (n, mode) => MissingMathFMethods.Round(n, mode));
        Test((n, digits) => MissingMathFMethods.Round(n, digits, MidpointRounding.ToEven), (n, digits) => MissingMathFMethods.Round(n, digits));

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
        Test(
            (float n, out float result) => float.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}", new CultureInfo("en"), out result));
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}", null, out result),
            (float n, out float result) => float.TryParse($"{n}", out result));

        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), new CultureInfo("en"), out result));
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), new CultureInfo("en"), out result));
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}