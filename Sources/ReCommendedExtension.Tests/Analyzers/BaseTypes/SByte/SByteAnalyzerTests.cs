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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.SByte;

[TestFixture]
public sealed class SByteAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\SByte";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<sbyte, R> expected, Func<sbyte, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(sbyte.MinValue), actual(sbyte.MinValue));
        Assert.AreEqual(expected(sbyte.MaxValue), actual(sbyte.MaxValue));
    }

    static void Test<R>(Func<sbyte, sbyte, R> expected, Func<sbyte, sbyte, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, sbyte.MaxValue), actual(0, sbyte.MaxValue));
        Assert.AreEqual(expected(sbyte.MinValue, 0), actual(sbyte.MinValue, 0));
        Assert.AreEqual(expected(sbyte.MinValue, sbyte.MinValue), actual(sbyte.MinValue, sbyte.MinValue));
        Assert.AreEqual(expected(sbyte.MaxValue, sbyte.MaxValue), actual(sbyte.MaxValue, sbyte.MaxValue));
        Assert.AreEqual(expected(sbyte.MinValue, sbyte.MaxValue), actual(sbyte.MinValue, sbyte.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<sbyte, sbyte, bool> expected, FuncWithOut<sbyte, sbyte, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(sbyte.MaxValue, out expectedResult), actual(sbyte.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(sbyte.MinValue, out expectedResult), actual(sbyte.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingSByteMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingSByteMethods.Clamp(number, sbyte.MinValue, sbyte.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, (sbyte)1, (sbyte)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, sbyte.MinValue, sbyte.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingSByteMethods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((sbyte)0, (sbyte)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.SByte);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingSByteMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingSByteMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => sbyte.Parse($"{n}", NumberStyles.Integer), n => sbyte.Parse($"{n}"));
        Test(n => sbyte.Parse($"{n}", null), n => sbyte.Parse($"{n}"));
        Test(
            n => sbyte.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => sbyte.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => sbyte.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => sbyte.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingSByteMethods.Parse($"{n}".AsSpan(), null), n => MissingSByteMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingSByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingSByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingSByteMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingSByteMethods.RotateRight(n, 0), n => n);

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
            (sbyte n, out sbyte result) => sbyte.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}", null, out result),
            (sbyte n, out sbyte result) => sbyte.TryParse($"{n}", out result));

        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), null, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}