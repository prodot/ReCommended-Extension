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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt64;

[TestFixture]
public sealed class UInt64AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt64";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<ulong, R> expected, Func<ulong, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(ulong.MaxValue), actual(ulong.MaxValue));
    }

    static void Test<R>(Func<ulong, ulong, R> expected, Func<ulong, ulong, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, ulong.MaxValue), actual(0, ulong.MaxValue));
        Assert.AreEqual(expected(ulong.MaxValue, ulong.MaxValue), actual(ulong.MaxValue, ulong.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<ulong, ulong, bool> expected, FuncWithOut<ulong, ulong, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(ulong.MaxValue, out expectedResult), actual(ulong.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUInt64Methods.Clamp(number, 1, 1), _ => 1uL);
        Test(number => MissingUInt64Methods.Clamp(number, ulong.MinValue, ulong.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1uL, 1uL), _ => 1uL);
        Test(number => MissingMathMethods.Clamp(number, ulong.MinValue, ulong.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUInt64Methods.DivRem(0, 10), () => (0ul, 0ul));

        Test(() => MissingMathMethods.DivRem(0uL, 10uL), () => (0ul, 0ul));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt64);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt64Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt64Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => ulong.Parse($"{n}", NumberStyles.Integer), n => ulong.Parse($"{n}"));
        Test(n => ulong.Parse($"{n}", null), n => ulong.Parse($"{n}"));
        Test(
            n => ulong.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => ulong.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => ulong.Parse($"{n}", NumberStyles.None, null), n => ulong.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt64Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt64Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingUInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingUInt64Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUInt64Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUInt64Methods.RotateRight(n, 0), n => n);

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
            (ulong n, out ulong result) => ulong.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}", null, out result),
            (ulong n, out ulong result) => ulong.TryParse($"{n}", out result));

        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}