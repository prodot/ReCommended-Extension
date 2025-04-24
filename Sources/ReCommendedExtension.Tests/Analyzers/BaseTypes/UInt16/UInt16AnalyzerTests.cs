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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt16;

[TestFixture]
public sealed class UInt16AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt16";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<ushort, R> expected, Func<ushort, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(ushort.MaxValue), actual(ushort.MaxValue));
    }

    static void Test<R>(Func<ushort, ushort, R> expected, Func<ushort, ushort, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, ushort.MaxValue), actual(0, ushort.MaxValue));
        Assert.AreEqual(expected(ushort.MaxValue, ushort.MaxValue), actual(ushort.MaxValue, ushort.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<ushort, ushort, bool> expected, FuncWithOut<ushort, ushort, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(ushort.MaxValue, out expectedResult), actual(ushort.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUInt16Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingUInt16Methods.Clamp(number, ushort.MinValue, ushort.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, (ushort)1, (ushort)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, ushort.MinValue, ushort.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUInt16Methods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((ushort)0, (ushort)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt16);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt16Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt16Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => ushort.Parse($"{n}", NumberStyles.Integer), n => ushort.Parse($"{n}"));
        Test(n => ushort.Parse($"{n}", null), n => ushort.Parse($"{n}"));
        Test(
            n => ushort.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => ushort.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => ushort.Parse($"{n}", NumberStyles.None, null), n => ushort.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt16Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt16Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingUInt16Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingUInt16Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUInt16Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUInt16Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (ushort n, out ushort result) => ushort.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}", null, out result),
            (ushort n, out ushort result) => ushort.TryParse($"{n}", out result));

        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}