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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int32;

[TestFixture]
public sealed class Int32AnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int32";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<int, R> expected, Func<int, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(int.MinValue), actual(int.MinValue));
        Assert.AreEqual(expected(int.MaxValue), actual(int.MaxValue));
    }

    static void Test<R>(Func<int, int, R> expected, Func<int, int, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, int.MaxValue), actual(0, int.MaxValue));
        Assert.AreEqual(expected(int.MinValue, 0), actual(int.MinValue, 0));
        Assert.AreEqual(expected(int.MinValue, int.MinValue), actual(int.MinValue, int.MinValue));
        Assert.AreEqual(expected(int.MaxValue, int.MaxValue), actual(int.MaxValue, int.MaxValue));
        Assert.AreEqual(expected(int.MinValue, int.MaxValue), actual(int.MinValue, int.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<int, int, bool> expected, FuncWithOut<int, int, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(int.MaxValue, out expectedResult), actual(int.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(int.MinValue, out expectedResult), actual(int.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingInt32Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingInt32Methods.Clamp(number, int.MinValue, int.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, int.MinValue, int.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingInt32Methods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem(0, 10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.Int32);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingInt32Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingInt32Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => int.Parse($"{n}", NumberStyles.Integer), n => int.Parse($"{n}"));
        Test(n => int.Parse($"{n}", null), n => int.Parse($"{n}"));
        Test(n => int.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo), n => int.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => int.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => int.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingInt32Methods.Parse($"{n}".AsSpan(), null), n => MissingInt32Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingInt32Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingInt32Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingInt32Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingInt32Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }
}