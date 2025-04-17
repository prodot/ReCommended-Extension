using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int128;

[TestFixture]
[TestNet70]
public sealed class Int128AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or NotResolvedError;

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
        Test(number => Int128Analyzer.Int128.DivRem(number, 1), number => (number, 0));

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }
}