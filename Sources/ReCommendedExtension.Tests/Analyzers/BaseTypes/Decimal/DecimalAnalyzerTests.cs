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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class DecimalAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Decimal";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperationSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<decimal, R> expected, Func<decimal, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(decimal.MinValue), actual(decimal.MinValue));
        Assert.AreEqual(expected(decimal.MaxValue), actual(decimal.MaxValue));
    }

    static void Test<R>(Func<decimal, decimal, R> expected, Func<decimal, decimal, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, decimal.MaxValue), actual(0, decimal.MaxValue));
        Assert.AreEqual(expected(decimal.MinValue, 0), actual(decimal.MinValue, 0));
        Assert.AreEqual(expected(decimal.MinValue, decimal.MinValue), actual(decimal.MinValue, decimal.MinValue));
        Assert.AreEqual(expected(decimal.MaxValue, decimal.MaxValue), actual(decimal.MaxValue, decimal.MaxValue));
        Assert.AreEqual(expected(decimal.MinValue, decimal.MaxValue), actual(decimal.MinValue, decimal.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<decimal, decimal, bool> expected, FuncWithOut<decimal, decimal, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(decimal.MaxValue, out expectedResult), actual(decimal.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(decimal.MinValue, out expectedResult), actual(decimal.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingDecimalMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingDecimalMethods.Clamp(number, decimal.MinValue, decimal.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, decimal.MinValue, decimal.MaxValue), number => number);

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
        Test(number => number.GetTypeCode(), _ => TypeCode.Decimal);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingDecimalMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingDecimalMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => decimal.Parse($"{n}", NumberStyles.Number), n => decimal.Parse($"{n}"));
        Test(n => decimal.Parse($"{n}", null), n => decimal.Parse($"{n}"));
        Test(
            n => decimal.Parse($"{n}", NumberStyles.Number, NumberFormatInfo.InvariantInfo),
            n => decimal.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => decimal.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => decimal.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingDecimalMethods.Parse($"{n}".AsSpan(), null), n => MissingDecimalMethods.Parse($"{n}".AsSpan()));

        Test(
            n => MissingDecimalMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingDecimalMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }
}