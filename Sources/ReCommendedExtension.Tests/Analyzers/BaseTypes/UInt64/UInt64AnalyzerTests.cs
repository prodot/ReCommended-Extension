using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt64;

[TestFixture]
public sealed class UInt64AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt64";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or NotResolvedError;

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
        Test(number => MissingUInt64Methods.DivRem(number, 1), number => (number, 0ul));

        Test(() => MissingMathMethods.DivRem(0uL, 10uL), () => (0ul, 0ul));
        Test(number => MissingMathMethods.DivRem(number, 1uL), number => (number, 0ul));

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
}