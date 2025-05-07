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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt128;

using uint128 = ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos.UInt128;

[TestFixture]
[TestNet70]
public sealed class UInt128AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<uint128, R> expected, Func<uint128, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(uint128.MaxValue), actual(uint128.MaxValue));
    }

    static void Test<R>(Func<uint128, uint128, R> expected, Func<uint128, uint128, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, uint128.MaxValue), actual(0, uint128.MaxValue));
        Assert.AreEqual(expected(uint128.MaxValue, uint128.MaxValue), actual(uint128.MaxValue, uint128.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<uint128, uint128, bool> expected, FuncWithOut<uint128, uint128, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(uint128.MaxValue, out expectedResult), actual(uint128.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void TestClamp()
    {
        Test(number => uint128.Clamp(number, 1, 1), _ => 1u);
        Test(number => uint128.Clamp(number, uint128.MinValue, uint128.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    public void TestDivRem()
    {
        Test(() => uint128.DivRem(0, 10), () => (0, 0));

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
    public void TestMax()
    {
        Test(n => uint128.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMin()
    {
        Test(n => uint128.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => uint128.Parse($"{n}", NumberStyles.Integer), n => uint128.Parse($"{n}"));
        Test(n => uint128.Parse($"{n}", null), n => uint128.Parse($"{n}"));
        Test(
            n => uint128.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => uint128.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => uint128.Parse($"{n}", NumberStyles.None, null), n => uint128.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt128Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt128Methods.Parse($"{n}".AsSpan()));

        Test(
            n => MissingUInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingUInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    public void TestRotateLeft()
    {
        Test(n => uint128.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestRotateRight()
    {
        Test(n => uint128.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString("G"), n => n.ToString());
        Test(n => n.ToString("G0"), n => n.ToString());
        Test(n => n.ToString("G39"), n => n.ToString());
        Test(n => n.ToString("g"), n => n.ToString());
        Test(n => n.ToString("g0"), n => n.ToString());
        Test(n => n.ToString("g39"), n => n.ToString());
        Test(n => n.ToString("E6"), n => n.ToString("E"));
        Test(n => n.ToString("e6"), n => n.ToString("e"));
        Test(n => n.ToString("D0"), n => n.ToString("D"));
        Test(n => n.ToString("D1"), n => n.ToString("D"));
        Test(n => n.ToString("d0"), n => n.ToString("d"));
        Test(n => n.ToString("d1"), n => n.ToString("d"));
        Test(n => n.ToString("X0"), n => n.ToString("X"));
        Test(n => n.ToString("X1"), n => n.ToString("X"));
        Test(n => n.ToString("x0"), n => n.ToString("x"));
        Test(n => n.ToString("x1"), n => n.ToString("x"));

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D", null), n => n.ToString("D"));
        Test(n => n.ToString("G", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G39", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g39", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("E6", NumberFormatInfo.InvariantInfo), n => n.ToString("E", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("e6", NumberFormatInfo.InvariantInfo), n => n.ToString("e", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D0", NumberFormatInfo.InvariantInfo), n => n.ToString("D", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D1", NumberFormatInfo.InvariantInfo), n => n.ToString("D", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("d0", NumberFormatInfo.InvariantInfo), n => n.ToString("d", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("d1", NumberFormatInfo.InvariantInfo), n => n.ToString("d", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("X0", NumberFormatInfo.InvariantInfo), n => n.ToString("X"));
        Test(n => n.ToString("X1", NumberFormatInfo.InvariantInfo), n => n.ToString("X"));
        Test(n => n.ToString("X2", NumberFormatInfo.InvariantInfo), n => n.ToString("X2"));
        Test(n => n.ToString("x0", NumberFormatInfo.InvariantInfo), n => n.ToString("x"));
        Test(n => n.ToString("x1", NumberFormatInfo.InvariantInfo), n => n.ToString("x"));
        Test(n => n.ToString("x2", NumberFormatInfo.InvariantInfo), n => n.ToString("x2"));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", null, out result),
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", out result));

        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), null, out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}