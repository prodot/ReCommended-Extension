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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Half;

using half = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Half;

[TestFixture]
[TestNet50]
public sealed class HalfAnalyzerTests : BaseTypeAnalyzerTests<half>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Half";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or RedundantFormatPrecisionSpecifierHint || highlighting.IsError();

    protected override half[] TestValues { get; } =
    [
        (sbyte)0, (sbyte)1, (sbyte)2, -1, (half)(-0f), half.MaxValue, half.Epsilon, half.NaN, half.PositiveInfinity, half.NegativeInfinity,
    ];

    [Test]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => half.Parse($"{n}"));
        Test(n => half.Parse($"{n}", null), n => half.Parse($"{n}"));
        Test(
            n => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo),
            n => half.Parse($"{n}", NumberFormatInfo.InvariantInfo));
        Test(
            n => half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => half.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));

        Test(n => MissingHalfMethods.Parse($"{n}".AsSpan(), null), n => MissingHalfMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingHalfMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingHalfMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        var xValues = new[] { (byte)0, (half)(-0f), half.MaxValue, half.MinValue };
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        Test(n => MissingHalfMethods.Round(n, 0), n => MissingHalfMethods.Round(n));
        Test(n => MissingHalfMethods.Round(n, MidpointRounding.ToEven), n => MissingHalfMethods.Round(n));
        Test((n, mode) => MissingHalfMethods.Round(n, 0, mode), (n, mode) => MissingHalfMethods.Round(n, mode), xValues, roundings);
        Test(
            (n, digits) => MissingHalfMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingHalfMethods.Round(n, digits),
            xValues,
            digitsValues);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public void TestToString()
    {
        Test(n => n.ToString(null as string), n => n.ToString());
        Test(n => n.ToString(""), n => n.ToString());
        Test(n => n.ToString("G"), n => n.ToString());
        Test(n => n.ToString("G0"), n => n.ToString());
        Test(n => n.ToString("E6"), n => n.ToString("E"));
        Test(n => n.ToString("e6"), n => n.ToString("e"));

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());
        Test(n => n.ToString(null, NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("F", null), n => n.ToString("F"));
        Test(n => n.ToString("G", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("G0", NumberFormatInfo.InvariantInfo), n => n.ToString(NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("E6", NumberFormatInfo.InvariantInfo), n => n.ToString("E", NumberFormatInfo.InvariantInfo));
        Test(n => n.ToString("e6", NumberFormatInfo.InvariantInfo), n => n.ToString("e", NumberFormatInfo.InvariantInfo));

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (half n, out half result) => half.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (half n, out half result) => half.TryParse($"{n}", NumberFormatInfo.InvariantInfo, out result));
        Test((half n, out half result) => half.TryParse($"{n}", null, out result), (half n, out half result) => half.TryParse($"{n}", out result));

        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                NumberFormatInfo.InvariantInfo,
                out result),
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), NumberFormatInfo.InvariantInfo, out result));
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}