using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int64;

[TestFixture]
public sealed class Int64AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int64";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<long, R> expected, Func<long, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(long.MinValue), actual(long.MinValue));
        Assert.AreEqual(expected(long.MaxValue), actual(long.MaxValue));
    }

    static void Test<R>(Func<long, long, R> expected, Func<long, long, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, long.MaxValue), actual(0, long.MaxValue));
        Assert.AreEqual(expected(long.MinValue, 0), actual(long.MinValue, 0));
        Assert.AreEqual(expected(long.MinValue, long.MinValue), actual(long.MinValue, long.MinValue));
        Assert.AreEqual(expected(long.MaxValue, long.MaxValue), actual(long.MaxValue, long.MaxValue));
        Assert.AreEqual(expected(long.MinValue, long.MaxValue), actual(long.MinValue, long.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<long, long, bool> expected, FuncWithOut<long, long, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(long.MaxValue, out expectedResult), actual(long.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(long.MinValue, out expectedResult), actual(long.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingInt64Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingInt64Methods.Clamp(number, long.MinValue, long.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, long.MinValue, long.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingInt64Methods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem(0L, 10L), () => (0, 0));

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
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Int64);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => MissingInt64Methods.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => MissingInt64Methods.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingInt64Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMaxMagnitude()
    {
        Test(n => MissingInt64Methods.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingInt64Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMinMagnitude()
    {
        Test(n => MissingInt64Methods.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => long.Parse($"{n}", NumberStyles.Integer), n => long.Parse($"{n}"));
        Test(n => long.Parse($"{n}", null), n => long.Parse($"{n}"));
        Test(n => long.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo), n => long.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => long.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => long.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingInt64Methods.Parse($"{n}".AsSpan(), null), n => MissingInt64Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingInt64Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingInt64Methods.RotateRight(n, 0), n => n);

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
            (long n, out long result) => long.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}", null, out result),
            (long n, out long result) => long.TryParse($"{n}", out result));

        Test(
            (long n, out long result) => MissingInt64Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), null, out result),
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (long n, out long result) => MissingInt64Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (long n, out long result) => MissingInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (long n, out long result) => MissingInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}