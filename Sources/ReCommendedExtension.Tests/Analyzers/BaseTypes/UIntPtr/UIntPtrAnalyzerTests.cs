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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UIntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UIntPtrAnalyzerTests : BaseTypeAnalyzerTests<nuint>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UIntPtr";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override nuint[] TestValues { get; } = [0, 1, 2];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUIntPtrMethods.Clamp(number, 1, 1), _ => 1u);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1u);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUIntPtrMethods.DivRem(0, 10), () => (0u, 0u));

        Test(() => MissingMathMethods.DivRem((nuint)0, 10), () => (0u, 0u));

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
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString("G"), n => n.ToString());
        Test(n => n.ToString("G0"), n => n.ToString());
        Test(n => n.ToString("g"), n => n.ToString());
        Test(n => n.ToString("g0"), n => n.ToString());
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
        Test(n => n.ToString("g", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
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