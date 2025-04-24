using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
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
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

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

    static void Test<R>(Func<HalfAnalyzer.Half, HalfAnalyzer.Half, R> expected, Func<HalfAnalyzer.Half, HalfAnalyzer.Half, R> actual)
    {
        Assert.AreEqual(expected((byte)0, (byte)0), actual((byte)0, (byte)0));
        Assert.AreEqual(expected((byte)0, HalfAnalyzer.Half.MaxValue), actual((byte)0, HalfAnalyzer.Half.MaxValue));
        Assert.AreEqual(expected(HalfAnalyzer.Half.MinValue, (byte)0), actual(HalfAnalyzer.Half.MinValue, (byte)0));
        Assert.AreEqual(
            expected(HalfAnalyzer.Half.MinValue, HalfAnalyzer.Half.MinValue),
            actual(HalfAnalyzer.Half.MinValue, HalfAnalyzer.Half.MinValue));
        Assert.AreEqual(
            expected(HalfAnalyzer.Half.MaxValue, HalfAnalyzer.Half.MaxValue),
            actual(HalfAnalyzer.Half.MaxValue, HalfAnalyzer.Half.MaxValue));
        Assert.AreEqual(
            expected(HalfAnalyzer.Half.MinValue, HalfAnalyzer.Half.MaxValue),
            actual(HalfAnalyzer.Half.MinValue, HalfAnalyzer.Half.MaxValue));
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