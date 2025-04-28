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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Byte;

[TestFixture]
public sealed class ByteAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Byte";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<byte, R> expected, Func<byte, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(byte.MaxValue), actual(byte.MaxValue));
    }

    static void Test<R>(Func<byte, byte, R> expected, Func<byte, byte, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, byte.MaxValue), actual(0, byte.MaxValue));
        Assert.AreEqual(expected(byte.MaxValue, byte.MaxValue), actual(byte.MaxValue, byte.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<byte, byte, bool> expected, FuncWithOut<byte, byte, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(byte.MaxValue, out expectedResult), actual(byte.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingByteMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingByteMethods.Clamp(number, 0, 255), number => number);

        Test(number => MissingMathMethods.Clamp(number, (byte)1, (byte)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, byte.MinValue, byte.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingByteMethods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((byte)0, (byte)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.Byte);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingByteMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingByteMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => byte.Parse($"{n}", NumberStyles.Integer), n => byte.Parse($"{n}"));
        Test(n => byte.Parse($"{n}", null), n => byte.Parse($"{n}"));
        Test(n => byte.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo), n => byte.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => byte.Parse($"{n}", NumberStyles.None, null), n => byte.Parse($"{n}", NumberStyles.None));

        Test(n => MissingByteMethods.Parse($"{n}".AsSpan(), null), n => MissingByteMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingByteMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingByteMethods.RotateRight(n, 0), n => n);

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
            (byte n, out byte result) => byte.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}", null, out result),
            (byte n, out byte result) => byte.TryParse($"{n}", out result));

        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), null, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (byte n, out byte result) => MissingByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}