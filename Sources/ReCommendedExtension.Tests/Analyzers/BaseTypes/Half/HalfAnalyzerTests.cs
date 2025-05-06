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
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Half;

[TestFixture]
[TestNet50]
public sealed class HalfAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Half";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or RedundantFormatPrecisionSpecifierHint || highlighting.IsError();

    static void Test<R>(Func<HalfInfo.Half, R> expected, Func<HalfInfo.Half, R> actual)
    {
        Assert.AreEqual(expected((byte)0), actual((byte)0));
        Assert.AreEqual(expected(HalfInfo.Half.MinValue), actual(HalfInfo.Half.MinValue));
        Assert.AreEqual(expected(HalfInfo.Half.MaxValue), actual(HalfInfo.Half.MaxValue));
        Assert.AreEqual(expected(HalfInfo.Half.Epsilon), actual(HalfInfo.Half.Epsilon));
        Assert.AreEqual(expected(HalfInfo.Half.NaN), actual(HalfInfo.Half.NaN));
        Assert.AreEqual(expected(HalfInfo.Half.PositiveInfinity), actual(HalfInfo.Half.PositiveInfinity));
        Assert.AreEqual(expected(HalfInfo.Half.NegativeInfinity), actual(HalfInfo.Half.NegativeInfinity));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<HalfInfo.Half, HalfInfo.Half, bool> expected, FuncWithOut<HalfInfo.Half, HalfInfo.Half, bool> actual)
    {
        Assert.AreEqual(expected((byte)0, out var expectedResult), actual((byte)0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(HalfInfo.Half.MaxValue, out expectedResult), actual(HalfInfo.Half.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(HalfInfo.Half.MinValue, out expectedResult), actual(HalfInfo.Half.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    static void Test(Func<HalfInfo.Half, MidpointRounding, HalfInfo.Half> expected, Func<HalfInfo.Half, MidpointRounding, HalfInfo.Half> actual)
    {
        Assert.AreEqual(expected((byte)0, MidpointRounding.ToEven), actual((byte)0, MidpointRounding.ToEven));
        Assert.AreEqual(expected((byte)0, MidpointRounding.AwayFromZero), actual((byte)0, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected((HalfInfo.Half)(-0f), MidpointRounding.ToEven), actual((HalfInfo.Half)(-0f), MidpointRounding.ToEven));
        Assert.AreEqual(expected((HalfInfo.Half)(-0f), MidpointRounding.AwayFromZero), actual((HalfInfo.Half)(-0f), MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(HalfInfo.Half.MaxValue, MidpointRounding.ToEven), actual(HalfInfo.Half.MaxValue, MidpointRounding.ToEven));
        Assert.AreEqual(
            expected(HalfInfo.Half.MaxValue, MidpointRounding.AwayFromZero),
            actual(HalfInfo.Half.MaxValue, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(HalfInfo.Half.MinValue, MidpointRounding.ToEven), actual(HalfInfo.Half.MinValue, MidpointRounding.ToEven));
        Assert.AreEqual(
            expected(HalfInfo.Half.MinValue, MidpointRounding.AwayFromZero),
            actual(HalfInfo.Half.MinValue, MidpointRounding.AwayFromZero));
    }

    static void Test(Func<HalfInfo.Half, int, HalfInfo.Half> expected, Func<HalfInfo.Half, int, HalfInfo.Half> actual)
    {
        Assert.AreEqual(expected((byte)0, 0), actual((byte)0, 0));
        Assert.AreEqual(expected((byte)0, 1), actual((byte)0, 1));
        Assert.AreEqual(expected((byte)0, 2), actual((byte)0, 2));

        Assert.AreEqual(expected((HalfInfo.Half)(-0f), 0), actual((HalfInfo.Half)(-0f), 0));
        Assert.AreEqual(expected((HalfInfo.Half)(-0f), 1), actual((HalfInfo.Half)(-0f), 1));
        Assert.AreEqual(expected((HalfInfo.Half)(-0f), 2), actual((HalfInfo.Half)(-0f), 2));

        Assert.AreEqual(expected(HalfInfo.Half.MaxValue, 0), actual(HalfInfo.Half.MaxValue, 0));
        Assert.AreEqual(expected(HalfInfo.Half.MaxValue, 1), actual(HalfInfo.Half.MaxValue, 1));
        Assert.AreEqual(expected(HalfInfo.Half.MaxValue, 2), actual(HalfInfo.Half.MaxValue, 2));

        Assert.AreEqual(expected(HalfInfo.Half.MinValue, 0), actual(HalfInfo.Half.MinValue, 0));
        Assert.AreEqual(expected(HalfInfo.Half.MinValue, 1), actual(HalfInfo.Half.MinValue, 1));
        Assert.AreEqual(expected(HalfInfo.Half.MinValue, 2), actual(HalfInfo.Half.MinValue, 2));
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
        Test(n => HalfInfo.Half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => HalfInfo.Half.Parse($"{n}"));
        Test(n => HalfInfo.Half.Parse($"{n}", null), n => HalfInfo.Half.Parse($"{n}"));
        Test(
            n => HalfInfo.Half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo),
            n => HalfInfo.Half.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => HalfInfo.Half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => HalfInfo.Half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));

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
            (HalfInfo.Half n, out HalfInfo.Half result) => HalfInfo.Half.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (HalfInfo.Half n, out HalfInfo.Half result) => HalfInfo.Half.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (HalfInfo.Half n, out HalfInfo.Half result) => HalfInfo.Half.TryParse($"{n}", null, out result),
            (HalfInfo.Half n, out HalfInfo.Half result) => HalfInfo.Half.TryParse($"{n}", out result));

        Test(
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (HalfInfo.Half n, out HalfInfo.Half result)
                => MissingHalfMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), null, out result),
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (HalfInfo.Half n, out HalfInfo.Half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}