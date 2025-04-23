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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class DoubleAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<double, R> expected, Func<double, R> actual, double minValue = double.MinValue, double maxValue = double.MaxValue)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(minValue), actual(minValue));
        Assert.AreEqual(expected(maxValue), actual(maxValue));
        Assert.AreEqual(expected(double.Epsilon), actual(double.Epsilon));
        Assert.AreEqual(expected(double.NaN), actual(double.NaN));
        Assert.AreEqual(expected(double.PositiveInfinity), actual(double.PositiveInfinity));
        Assert.AreEqual(expected(double.NegativeInfinity), actual(double.NegativeInfinity));
    }

    static void Test<R>(Func<double, double, R> expected, Func<double, double, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, double.MaxValue), actual(0, double.MaxValue));
        Assert.AreEqual(expected(double.MinValue, 0), actual(double.MinValue, 0));
        Assert.AreEqual(expected(double.MinValue, double.MinValue), actual(double.MinValue, double.MinValue));
        Assert.AreEqual(expected(double.MaxValue, double.MaxValue), actual(double.MaxValue, double.MaxValue));
        Assert.AreEqual(expected(double.MinValue, double.MaxValue), actual(double.MinValue, double.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<double, double, bool> expected, FuncWithOut<double, double, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(double.MaxValue, out expectedResult), actual(double.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(double.MinValue, out expectedResult), actual(double.MinValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Double);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => double.Parse($"{n}"), float.MinValue, float.MaxValue);
        Test(n => double.Parse($"{n}", null), n => double.Parse($"{n}"), float.MinValue, float.MaxValue);
        Test(
            n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en")),
            n => double.Parse($"{n}", new CultureInfo("en")),
            float.MinValue,
            float.MaxValue);
        Test(
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint),
            float.MinValue,
            float.MaxValue);

        Test(
            n => MissingDoubleMethods.Parse($"{n}".AsSpan(), null),
            n => MissingDoubleMethods.Parse($"{n}".AsSpan()),
            float.MinValue,
            float.MaxValue);

        Test(
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")),
            float.MinValue,
            float.MaxValue);

        DoNamedTest2();
    }
}