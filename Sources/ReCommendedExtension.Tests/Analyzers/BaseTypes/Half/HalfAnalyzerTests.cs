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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Half;

using half = ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos.Half;

[TestFixture]
[TestNet50]
public sealed class HalfAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Half";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or RedundantFormatPrecisionSpecifierHint || highlighting.IsError();

    static void Test<R>(Func<half, R> expected, Func<half, R> actual)
    {
        Assert.AreEqual(expected((sbyte)0), actual((sbyte)0));
        Assert.AreEqual(expected((sbyte)1), actual((sbyte)1));
        Assert.AreEqual(expected((sbyte)2), actual((sbyte)2));
        Assert.AreEqual(expected((sbyte)-1), actual((sbyte)-1));
        Assert.AreEqual(expected((half)(-0f)), actual((half)(-0f)));
        Assert.AreEqual(expected(half.MinValue), actual(half.MinValue));
        Assert.AreEqual(expected(half.MaxValue), actual(half.MaxValue));
        Assert.AreEqual(expected(half.Epsilon), actual(half.Epsilon));
        Assert.AreEqual(expected(half.NaN), actual(half.NaN));
        Assert.AreEqual(expected(half.PositiveInfinity), actual(half.PositiveInfinity));
        Assert.AreEqual(expected(half.NegativeInfinity), actual(half.NegativeInfinity));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<half, half, bool> expected, FuncWithOut<half, half, bool> actual)
    {
        Assert.AreEqual(expected((byte)0, out var expectedResult), actual((byte)0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(half.MaxValue, out expectedResult), actual(half.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(half.MinValue, out expectedResult), actual(half.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    static void Test(Func<half, MidpointRounding, half> expected, Func<half, MidpointRounding, half> actual)
    {
        Assert.AreEqual(expected((byte)0, MidpointRounding.ToEven), actual((byte)0, MidpointRounding.ToEven));
        Assert.AreEqual(expected((byte)0, MidpointRounding.AwayFromZero), actual((byte)0, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected((half)(-0f), MidpointRounding.ToEven), actual((half)(-0f), MidpointRounding.ToEven));
        Assert.AreEqual(expected((half)(-0f), MidpointRounding.AwayFromZero), actual((half)(-0f), MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(half.MaxValue, MidpointRounding.ToEven), actual(half.MaxValue, MidpointRounding.ToEven));
        Assert.AreEqual(expected(half.MaxValue, MidpointRounding.AwayFromZero), actual(half.MaxValue, MidpointRounding.AwayFromZero));

        Assert.AreEqual(expected(half.MinValue, MidpointRounding.ToEven), actual(half.MinValue, MidpointRounding.ToEven));
        Assert.AreEqual(expected(half.MinValue, MidpointRounding.AwayFromZero), actual(half.MinValue, MidpointRounding.AwayFromZero));
    }

    static void Test(Func<half, int, half> expected, Func<half, int, half> actual)
    {
        Assert.AreEqual(expected((byte)0, 0), actual((byte)0, 0));
        Assert.AreEqual(expected((byte)0, 1), actual((byte)0, 1));
        Assert.AreEqual(expected((byte)0, 2), actual((byte)0, 2));

        Assert.AreEqual(expected((half)(-0f), 0), actual((half)(-0f), 0));
        Assert.AreEqual(expected((half)(-0f), 1), actual((half)(-0f), 1));
        Assert.AreEqual(expected((half)(-0f), 2), actual((half)(-0f), 2));

        Assert.AreEqual(expected(half.MaxValue, 0), actual(half.MaxValue, 0));
        Assert.AreEqual(expected(half.MaxValue, 1), actual(half.MaxValue, 1));
        Assert.AreEqual(expected(half.MaxValue, 2), actual(half.MaxValue, 2));

        Assert.AreEqual(expected(half.MinValue, 0), actual(half.MinValue, 0));
        Assert.AreEqual(expected(half.MinValue, 1), actual(half.MinValue, 1));
        Assert.AreEqual(expected(half.MinValue, 2), actual(half.MinValue, 2));
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
        Test(n => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => half.Parse($"{n}"));
        Test(n => half.Parse($"{n}", null), n => half.Parse($"{n}"));
        Test(
            n => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo),
            n => half.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));

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
            (half n, out half result) => half.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (half n, out half result) => half.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test((half n, out half result) => half.TryParse($"{n}", null, out result), (half n, out half result) => half.TryParse($"{n}", out result));

        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}