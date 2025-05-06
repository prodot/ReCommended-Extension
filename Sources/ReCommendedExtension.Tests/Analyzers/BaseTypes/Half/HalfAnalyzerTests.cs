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
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Half;

[TestFixture]
[TestNet50]
public sealed class HalfAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Half";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or RedundantFormatPrecisionSpecifierHint || highlighting.IsError();

    static void Test<R>(Func<HalfAnalyzer.Half, R> expected, Func<HalfAnalyzer.Half, R> actual)
    {
        Assert.AreEqual(expected((byte)0), actual((byte)0));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue), actual(HalfAnalyzer.Half.MinValue));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MaxValue), actual(HalfAnalyzer.Half.MaxValue));
        Assert.AreEqual(expected(HalfAnalyzer.Half.Epsilon), actual(HalfAnalyzer.Half.Epsilon));
        Assert.AreEqual(expected(HalfAnalyzer.Half.NaN), actual(HalfAnalyzer.Half.NaN));
        Assert.AreEqual(expected(HalfAnalyzer.Half.PositiveInfinity), actual(HalfAnalyzer.Half.PositiveInfinity));
        Assert.AreEqual(expected(HalfAnalyzer.Half.NegativeInfinity), actual(HalfAnalyzer.Half.NegativeInfinity));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<HalfAnalyzer.Half, HalfAnalyzer.Half, bool> expected, FuncWithOut<HalfAnalyzer.Half, HalfAnalyzer.Half, bool> actual)
    {
        Assert.AreEqual(expected((byte)0, out var expectedResult), actual((byte)0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(HalfAnalyzer.Half.MaxValue, out expectedResult), actual(HalfAnalyzer.Half.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue, out expectedResult), actual(HalfAnalyzer.Half.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    static void Test(
        Func<HalfAnalyzer.Half, MidpointRounding, HalfAnalyzer.Half> expected,
        Func<HalfAnalyzer.Half, MidpointRounding, HalfAnalyzer.Half> actual)
    {
        Assert.AreEqual(expected((byte)0, MidpointRounding.ToEven), actual((byte)0, MidpointRounding.ToEven));
        Assert.AreEqual(expected((byte)0, MidpointRounding.AwayFromZero), actual((byte)0, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected((HalfAnalyzer.Half)(-0f), MidpointRounding.ToEven), actual((HalfAnalyzer.Half)(-0f), MidpointRounding.ToEven));
        Assert.AreEqual(
            expected((HalfAnalyzer.Half)(-0f), MidpointRounding.AwayFromZero),
            actual((HalfAnalyzer.Half)(-0f), MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(HalfAnalyzer.Half.MaxValue, MidpointRounding.ToEven), actual(HalfAnalyzer.Half.MaxValue, MidpointRounding.ToEven));
        Assert.AreEqual(
            expected(HalfAnalyzer.Half.MaxValue, MidpointRounding.AwayFromZero),
            actual(HalfAnalyzer.Half.MaxValue, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue, MidpointRounding.ToEven), actual(HalfAnalyzer.Half.MinValue, MidpointRounding.ToEven));
        Assert.AreEqual(
            expected(HalfAnalyzer.Half.MinValue, MidpointRounding.AwayFromZero),
            actual(HalfAnalyzer.Half.MinValue, MidpointRounding.AwayFromZero));
    }

    static void Test(Func<HalfAnalyzer.Half, int, HalfAnalyzer.Half> expected, Func<HalfAnalyzer.Half, int, HalfAnalyzer.Half> actual)
    {
        Assert.AreEqual(expected((byte)0, 0), actual((byte)0, 0));
        Assert.AreEqual(expected((byte)0, 1), actual((byte)0, 1));
        Assert.AreEqual(expected((byte)0, 2), actual((byte)0, 2));

        Assert.AreEqual(expected((HalfAnalyzer.Half)(-0f), 0), actual((HalfAnalyzer.Half)(-0f), 0));
        Assert.AreEqual(expected((HalfAnalyzer.Half)(-0f), 1), actual((HalfAnalyzer.Half)(-0f), 1));
        Assert.AreEqual(expected((HalfAnalyzer.Half)(-0f), 2), actual((HalfAnalyzer.Half)(-0f), 2));

        Assert.AreEqual(expected(HalfAnalyzer.Half.MaxValue, 0), actual(HalfAnalyzer.Half.MaxValue, 0));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MaxValue, 1), actual(HalfAnalyzer.Half.MaxValue, 1));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MaxValue, 2), actual(HalfAnalyzer.Half.MaxValue, 2));

        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue, 0), actual(HalfAnalyzer.Half.MinValue, 0));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue, 1), actual(HalfAnalyzer.Half.MinValue, 1));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue, 2), actual(HalfAnalyzer.Half.MinValue, 2));
    }

    [Test]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => HalfAnalyzer.Half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => HalfAnalyzer.Half.Parse($"{n}"));
        Test(n => HalfAnalyzer.Half.Parse($"{n}", null), n => HalfAnalyzer.Half.Parse($"{n}"));
        Test(
            n => HalfAnalyzer.Half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo),
            n => HalfAnalyzer.Half.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => HalfAnalyzer.Half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => HalfAnalyzer.Half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));

        Test(n => MissingHalfMethods.Parse($"{n}".AsSpan(), null), n => MissingHalfMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingHalfMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingHalfMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        Test(n => MissingHalfMethods.Round(n, 0), n => MissingHalfMethods.Round(n));
        Test(n => MissingHalfMethods.Round(n, MidpointRounding.ToEven), n => MissingHalfMethods.Round(n));
        Test((n, mode) => MissingHalfMethods.Round(n, 0, mode), (n, mode) => MissingHalfMethods.Round(n, mode));
        Test((n, digits) => MissingHalfMethods.Round(n, digits, MidpointRounding.ToEven), (n, digits) => MissingHalfMethods.Round(n, digits));

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
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => HalfAnalyzer.Half.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => HalfAnalyzer.Half.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => HalfAnalyzer.Half.TryParse($"{n}", null, out result),
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => HalfAnalyzer.Half.TryParse($"{n}", out result));

        Test(
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result)
                => MissingHalfMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), null, out result),
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (HalfAnalyzer.Half n, out HalfAnalyzer.Half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}