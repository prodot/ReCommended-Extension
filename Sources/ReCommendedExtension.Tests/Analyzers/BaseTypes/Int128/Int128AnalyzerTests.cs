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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int128;

[TestFixture]
[TestNet70]
public sealed class Int128AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<Int128Analyzer.Int128, R> expected, Func<Int128Analyzer.Int128, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(Int128Analyzer.Int128.MinValue), actual(Int128Analyzer.Int128.MinValue));
        Assert.AreEqual(expected(Int128Analyzer.Int128.MaxValue), actual(Int128Analyzer.Int128.MaxValue));
    }

    static void Test<R>(Func<Int128Analyzer.Int128, Int128Analyzer.Int128, R> expected, Func<Int128Analyzer.Int128, Int128Analyzer.Int128, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, Int128Analyzer.Int128.MaxValue), actual(0, Int128Analyzer.Int128.MaxValue));
        Assert.AreEqual(expected(Int128Analyzer.Int128.MinValue, 0), actual(Int128Analyzer.Int128.MinValue, 0));
        Assert.AreEqual(
            expected(Int128Analyzer.Int128.MinValue, Int128Analyzer.Int128.MinValue),
            actual(Int128Analyzer.Int128.MinValue, Int128Analyzer.Int128.MinValue));
        Assert.AreEqual(
            expected(Int128Analyzer.Int128.MaxValue, Int128Analyzer.Int128.MaxValue),
            actual(Int128Analyzer.Int128.MaxValue, Int128Analyzer.Int128.MaxValue));
        Assert.AreEqual(
            expected(Int128Analyzer.Int128.MinValue, Int128Analyzer.Int128.MaxValue),
            actual(Int128Analyzer.Int128.MinValue, Int128Analyzer.Int128.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(
        FuncWithOut<Int128Analyzer.Int128, Int128Analyzer.Int128, bool> expected,
        FuncWithOut<Int128Analyzer.Int128, Int128Analyzer.Int128, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(Int128Analyzer.Int128.MaxValue, out expectedResult), actual(Int128Analyzer.Int128.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(Int128Analyzer.Int128.MinValue, out expectedResult), actual(Int128Analyzer.Int128.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void TestClamp()
    {
        Test(number => Int128Analyzer.Int128.Clamp(number, 1, 1), _ => 1);
        Test(number => Int128Analyzer.Int128.Clamp(number, Int128Analyzer.Int128.MinValue, Int128Analyzer.Int128.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    public void TestDivRem()
    {
        Test(() => Int128Analyzer.Int128.DivRem(0, 10), () => (0, 0));

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => Int128Analyzer.Int128.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => Int128Analyzer.Int128.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    public void TestMax()
    {
        Test(n => Int128Analyzer.Int128.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMaxMagnitude()
    {
        Test(n => Int128Analyzer.Int128.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMin()
    {
        Test(n => Int128Analyzer.Int128.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMinMagnitude()
    {
        Test(n => Int128Analyzer.Int128.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => Int128Analyzer.Int128.Parse($"{n}", NumberStyles.Integer), n => Int128Analyzer.Int128.Parse($"{n}"));
        Test(n => Int128Analyzer.Int128.Parse($"{n}", null), n => Int128Analyzer.Int128.Parse($"{n}"));
        Test(
            n => Int128Analyzer.Int128.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => Int128Analyzer.Int128.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => Int128Analyzer.Int128.Parse($"{n}", NumberStyles.AllowLeadingSign, null),
            n => Int128Analyzer.Int128.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingInt128Methods.Parse($"{n}".AsSpan(), null), n => MissingInt128Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    public void TestRotateLeft()
    {
        Test(n => Int128Analyzer.Int128.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestRotateRight()
    {
        Test(n => Int128Analyzer.Int128.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D", null), n => n.ToString("D"));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => Int128Analyzer.Int128.TryParse(
                $"{n}",
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result)
                => Int128Analyzer.Int128.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => Int128Analyzer.Int128.TryParse($"{n}", null, out result),
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => Int128Analyzer.Int128.TryParse($"{n}", out result));

        Test(
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse(
                $"{n}".AsSpan(),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), null, out result),
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result)
                => MissingInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (Int128Analyzer.Int128 n, out Int128Analyzer.Int128 result) => MissingInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}