using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int16;

[TestFixture]
public sealed class Int16AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int16";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or NotResolvedError;

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<short, R> expected, Func<short, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(short.MinValue), actual(short.MinValue));
        Assert.AreEqual(expected(short.MaxValue), actual(short.MaxValue));
    }

    static void Test<R>(Func<short, short, R> expected, Func<short, short, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, short.MaxValue), actual(0, short.MaxValue));
        Assert.AreEqual(expected(short.MinValue, 0), actual(short.MinValue, 0));
        Assert.AreEqual(expected(short.MinValue, short.MinValue), actual(short.MinValue, short.MinValue));
        Assert.AreEqual(expected(short.MaxValue, short.MaxValue), actual(short.MaxValue, short.MaxValue));
        Assert.AreEqual(expected(short.MinValue, short.MaxValue), actual(short.MinValue, short.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<short, short, bool> expected, FuncWithOut<short, short, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(short.MaxValue, out expectedResult), actual(short.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(short.MinValue, out expectedResult), actual(short.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingInt16Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingInt16Methods.Clamp(number, short.MinValue, short.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, (short)1, (short)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, short.MinValue, short.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingInt16Methods.DivRem(0, 10), () => (0, 0));
        Test(number => MissingInt16Methods.DivRem(number, 1), number => (number, 0));

        Test(() => MissingMathMethods.DivRem((short)0, (short)10), () => (0, 0));
        Test(number => MissingMathMethods.DivRem(number, (short)1), number => (number, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.Int16);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingInt16Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }
}