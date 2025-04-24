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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.IntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class IntPtrAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\IntPtr";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<nint, R> expected, Func<nint, R> actual) => Assert.AreEqual(expected(0), actual(0));

    static void Test<R>(Func<nint, nint, R> expected, Func<nint, nint, R> actual) => Assert.AreEqual(expected(0, 0), actual(0, 0));

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<nint, nint, bool> expected, FuncWithOut<nint, nint, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingIntPtrMethods.Clamp(number, 1, 1), _ => 1);

        Test(number => MissingMathMethods.Clamp(number, (nint)1, (nint)1), _ => 1);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingIntPtrMethods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((nint)0, (nint)10), () => (0, 0));

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
        Test(n => MissingIntPtrMethods.Max(n, n), n => n);
        Test(n => MissingMathMethods.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingIntPtrMethods.Min(n, n), n => n);
        Test(n => MissingMathMethods.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.Integer), n => MissingIntPtrMethods.Parse($"{n}"));
        Test(n => MissingIntPtrMethods.Parse($"{n}", null), n => MissingIntPtrMethods.Parse($"{n}"));
        Test(
            n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => MissingIntPtrMethods.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.AllowLeadingSign, null),
            n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingIntPtrMethods.Parse($"{n}".AsSpan(), null), n => MissingIntPtrMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingIntPtrMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingIntPtrMethods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", out result));

        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}