using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int32;

[TestFixture]
public sealed class Int32AnalyzerTests : BaseTypeAnalyzerTests<int>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int32";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override int[] TestValues { get; } = [0, 1, 2, -1, int.MinValue, int.MaxValue];

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
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

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
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => MissingInt32Methods.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => MissingInt32Methods.IsPositive(number), number => number >= 0);

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
    public void TestMaxMagnitude()
    {
        Test(n => MissingInt32Methods.MaxMagnitude(n, n), n => n);

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
    [TestNet70]
    public void TestMinMagnitude()
    {
        Test(n => MissingInt32Methods.MinMagnitude(n, n), n => n);

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

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString("G"), n => n.ToString());
        Test(n => n.ToString("G0"), n => n.ToString());
        Test(n => n.ToString("G10"), n => n.ToString());
        Test(n => n.ToString("G11"), n => n.ToString());
        Test(n => n.ToString("g"), n => n.ToString());
        Test(n => n.ToString("g0"), n => n.ToString());
        Test(n => n.ToString("g10"), n => n.ToString());
        Test(n => n.ToString("g11"), n => n.ToString());
        Test(n => n.ToString("E6"), n => n.ToString("E"));
        Test(n => n.ToString("e6"), n => n.ToString("e"));
        Test(n => n.ToString("D0"), n => n.ToString("D"));
        Test(n => n.ToString("D1"), n => n.ToString("D"));
        Test(n => n.ToString("d0"), n => n.ToString("d"));
        Test(n => n.ToString("d1"), n => n.ToString("d"));
        Test(n => n.ToString("X0"), n => n.ToString("X"));
        Test(n => n.ToString("X1"), n => n.ToString("X"));
        Test(n => n.ToString("x0"), n => n.ToString("x"));
        Test(n => n.ToString("x1"), n => n.ToString("x"));

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D", null), n => n.ToString("D"));
        Test(n => n.ToString("G", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G10", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G11", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g10", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g11", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("E6", NumberFormatInfo.InvariantInfo), n => n.ToString("E", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("e6", NumberFormatInfo.InvariantInfo), n => n.ToString("e", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D0", NumberFormatInfo.InvariantInfo), n => n.ToString("D", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("D1", NumberFormatInfo.InvariantInfo), n => n.ToString("D", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("d0", NumberFormatInfo.InvariantInfo), n => n.ToString("d", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("d1", NumberFormatInfo.InvariantInfo), n => n.ToString("d", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("X0", NumberFormatInfo.InvariantInfo), n => n.ToString("X"));
        Test(n => n.ToString("X1", NumberFormatInfo.InvariantInfo), n => n.ToString("X"));
        Test(n => n.ToString("X2", NumberFormatInfo.InvariantInfo), n => n.ToString("X2"));
        Test(n => n.ToString("x0", NumberFormatInfo.InvariantInfo), n => n.ToString("x"));
        Test(n => n.ToString("x1", NumberFormatInfo.InvariantInfo), n => n.ToString("x"));
        Test(n => n.ToString("x2", NumberFormatInfo.InvariantInfo), n => n.ToString("x2"));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (int n, out int result) => int.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}", null, out result),
            (int n, out int result) => int.TryParse($"{n}", out result));

        Test(
            (int n, out int result) => MissingInt32Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), null, out result),
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (int n, out int result) => MissingInt32Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (int n, out int result) => MissingInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (int n, out int result) => MissingInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}