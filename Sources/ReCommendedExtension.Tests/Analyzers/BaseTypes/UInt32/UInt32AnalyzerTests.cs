using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt32;

[TestFixture]
public sealed class UInt32AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt32";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or NotResolvedError;

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<uint, R> expected, Func<uint, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(uint.MaxValue), actual(uint.MaxValue));
    }

    static void Test<R>(Func<uint, uint, R> expected, Func<uint, uint, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, uint.MaxValue), actual(0, uint.MaxValue));
        Assert.AreEqual(expected(uint.MaxValue, uint.MaxValue), actual(uint.MaxValue, uint.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<uint, uint, bool> expected, FuncWithOut<uint, uint, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(uint.MaxValue, out expectedResult), actual(uint.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUInt32Methods.Clamp(number, 1, 1), _ => 1u);
        Test(number => MissingUInt32Methods.Clamp(number, uint.MinValue, uint.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1u, 1u), _ => 1u);
        Test(number => MissingMathMethods.Clamp(number, uint.MinValue, uint.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUInt32Methods.DivRem(0, 10), () => (0u, 0u));
        Test(number => MissingUInt32Methods.DivRem(number, 1), number => (number, 0u));

        Test(() => MissingMathMethods.DivRem(0u, 10u), () => (0u, 0u));
        Test(number => MissingMathMethods.DivRem(number, 1u), number => (number, 0u));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt32);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt32Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt32Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }
}