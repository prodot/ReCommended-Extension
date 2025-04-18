using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt16;

[TestFixture]
public sealed class UInt16AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt16";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or NotResolvedError;

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
        Test(number => MissingUInt16Methods.DivRem(number, 1), number => (number, 0));

        Test(() => MissingMathMethods.DivRem((ushort)0, (ushort)10), () => (0, 0));
        Test(number => MissingMathMethods.DivRem(number, (ushort)1), number => (number, 0));

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
}