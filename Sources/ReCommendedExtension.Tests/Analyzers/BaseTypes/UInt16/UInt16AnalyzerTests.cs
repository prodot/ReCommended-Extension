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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt16;

[TestFixture]
public sealed class UInt16AnalyzerTests : BaseTypeAnalyzerTests<ushort>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt16";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override ushort[] TestValues { get; } = [0, 1, 2, ushort.MaxValue];

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

        Test(() => MissingMathMethods.DivRem((ushort)0, (ushort)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt16);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt16Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt16Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => ushort.Parse($"{n}", NumberStyles.Integer), n => ushort.Parse($"{n}"));
        Test(n => ushort.Parse($"{n}", null), n => ushort.Parse($"{n}"));
        Test(
            n => ushort.Parse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo),
            n => ushort.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(n => ushort.Parse($"{n}", NumberStyles.None, null), n => ushort.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt16Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt16Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingUInt16Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingUInt16Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUInt16Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUInt16Methods.RotateRight(n, 0), n => n);

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
        Test(n => n.ToString("G5"), n => n.ToString());
        Test(n => n.ToString("G6"), n => n.ToString());
        Test(n => n.ToString("g"), n => n.ToString());
        Test(n => n.ToString("g0"), n => n.ToString());
        Test(n => n.ToString("g5"), n => n.ToString());
        Test(n => n.ToString("g6"), n => n.ToString());
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
        Test(n => n.ToString("G5", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G6", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g5", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("g6", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
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
            (ushort n, out ushort result) => ushort.TryParse($"{n}", NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}", null, out result),
            (ushort n, out ushort result) => ushort.TryParse($"{n}", out result));

        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}