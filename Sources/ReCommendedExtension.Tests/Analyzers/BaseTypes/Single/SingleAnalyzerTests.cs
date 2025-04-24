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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Single;

[TestFixture]
public sealed class SingleAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Single";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<R>(Func<float, R> expected, Func<float, R> actual)
    {
        Assert.AreEqual(expected(0), actual(0));
        Assert.AreEqual(expected(float.MinValue), actual(float.MinValue));
        Assert.AreEqual(expected(float.MaxValue), actual(float.MaxValue));
        Assert.AreEqual(expected(float.Epsilon), actual(float.Epsilon));
        Assert.AreEqual(expected(float.NaN), actual(float.NaN));
        Assert.AreEqual(expected(float.PositiveInfinity), actual(float.PositiveInfinity));
        Assert.AreEqual(expected(float.NegativeInfinity), actual(float.NegativeInfinity));
    }

    static void Test<R>(Func<float, float, R> expected, Func<float, float, R> actual)
    {
        Assert.AreEqual(expected(0, 0), actual(0, 0));
        Assert.AreEqual(expected(0, float.MaxValue), actual(0, float.MaxValue));
        Assert.AreEqual(expected(float.MinValue, 0), actual(float.MinValue, 0));
        Assert.AreEqual(expected(float.MinValue, float.MinValue), actual(float.MinValue, float.MinValue));
        Assert.AreEqual(expected(float.MaxValue, float.MaxValue), actual(float.MaxValue, float.MaxValue));
        Assert.AreEqual(expected(float.MinValue, float.MaxValue), actual(float.MinValue, float.MaxValue));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test(FuncWithOut<float, float, bool> expected, FuncWithOut<float, float, bool> actual)
    {
        Assert.AreEqual(expected(0, out var expectedResult), actual(0, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(float.MaxValue, out expectedResult), actual(float.MaxValue, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        Assert.AreEqual(expected(float.MinValue, out expectedResult), actual(float.MinValue, out actualResult));
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
        Test(number => number.GetTypeCode(), _ => TypeCode.Single);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => float.Parse($"{n}"));
        Test(n => float.Parse($"{n}", null), n => float.Parse($"{n}"));
        Test(
            n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("en")),
            n => float.Parse($"{n}", new CultureInfo("en")));
        Test(
            n => float.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => float.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));

        Test(n => MissingSingleMethods.Parse($"{n}".AsSpan(), null), n => MissingSingleMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingSingleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingSingleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("F", null), n => n.ToString("F"));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (float n, out float result) => float.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}", new CultureInfo("en"), out result));
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}", null, out result),
            (float n, out float result) => float.TryParse($"{n}", out result));

        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), new CultureInfo("en"), out result));
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                new CultureInfo("en"),
                out result),
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), new CultureInfo("en"), out result));
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}