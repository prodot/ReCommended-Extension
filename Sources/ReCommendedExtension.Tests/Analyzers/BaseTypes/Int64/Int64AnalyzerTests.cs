using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int64;

[TestFixture]
public sealed class Int64AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int64";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or NotResolvedError;

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
        Test(number => MissingInt64Methods.DivRem(number, 1), number => (number, 0));

        Test(() => MissingMathMethods.DivRem(0L, 10L), () => (0, 0));
        Test(number => MissingMathMethods.DivRem(number, 1L), number => (number, 0));

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
}