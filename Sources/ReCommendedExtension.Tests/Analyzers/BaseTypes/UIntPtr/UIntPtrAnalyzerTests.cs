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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UIntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UIntPtrAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UIntPtr";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<nuint, R> expected, Func<nuint, R> actual) => Assert.AreEqual(expected(0), actual(0));

    static void Test<R>(Func<nuint, nuint, R> expected, Func<nuint, nuint, R> actual) => Assert.AreEqual(expected(0, 0), actual(0, 0));

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<nuint, nuint, bool> expected, FuncWithOut<nuint, nuint, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUIntPtrMethods.Clamp(number, 1, 1), _ => 1u);

        Test(number => MissingMathMethods.Clamp(number, (nuint)1, (nuint)1), _ => 1u);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUIntPtrMethods.DivRem(0, 10), () => (0u, 0u));

        Test(() => MissingMathMethods.DivRem((nuint)0, (nuint)10), () => (0u, 0u));

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
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUIntPtrMethods.Max(n, n), n => n);
        Test(n => MissingMathMethods.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUIntPtrMethods.Min(n, n), n => n);
        Test(n => MissingMathMethods.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.Integer), n => MissingUIntPtrMethods.Parse($"{n}"));
        Test(n => MissingUIntPtrMethods.Parse($"{n}", null), n => MissingUIntPtrMethods.Parse($"{n}"));
        Test(
            n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => MissingUIntPtrMethods.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.None, null), n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUIntPtrMethods.Parse($"{n}".AsSpan(), null), n => MissingUIntPtrMethods.Parse($"{n}".AsSpan()));

        Test(
            n => MissingUIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingUIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUIntPtrMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUIntPtrMethods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", out result));

        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberFormatInfo.InvariantInfo,
                out result));
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}