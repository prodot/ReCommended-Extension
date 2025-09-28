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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class DoubleAnalyzerTests : BaseTypeAnalyzerTests<double>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or UseFloatingPointPatternSuggestion || highlighting.IsError();

    protected override double[] TestValues { get; } =
    [
        0,
        -0d,
        1,
        2,
        -1,
        -2,
        1.2,
        -1.2,
        double.MaxValue,
        double.MinValue,
        double.Epsilon,
        double.NaN,
        double.PositiveInfinity,
        double.NegativeInfinity,
    ];

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Double);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseFloatingPointPattern")]
    public void TestIsNaN()
    {
        Test(number => double.IsNaN(number), number => number is double.NaN);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        double[] values = [..TestValues.Except([double.MinValue, double.MaxValue]), float.MinValue, float.MaxValue];

        Test(n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => double.Parse($"{n}"), values);
        Test(n => double.Parse($"{n}", null), n => double.Parse($"{n}"), values);
        Test(
            (n, provider) => double.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => double.Parse(n.ToString(provider), provider),
            values,
            FormatProviders);
        Test(
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => double.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint),
            values);

        Test(n => MissingDoubleMethods.Parse($"{n}".AsSpan(), null), n => MissingDoubleMethods.Parse($"{n}".AsSpan()), values);

        Test(
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingDoubleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")),
            values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestRound()
    {
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        Test(n => MissingDoubleMethods.Round(n, 0), n => MissingDoubleMethods.Round(n));
        Test(n => MissingDoubleMethods.Round(n, MidpointRounding.ToEven), n => MissingDoubleMethods.Round(n));
        Test((n, mode) => MissingDoubleMethods.Round(n, 0, mode), (n, mode) => MissingDoubleMethods.Round(n, mode), TestValues, roundings);
        Test(
            (n, digits) => MissingDoubleMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingDoubleMethods.Round(n, digits),
            TestValues, 
            digitsValues);

        Test(n => Math.Round(n, 0), n => Math.Round(n));
        Test(n => Math.Round(n, MidpointRounding.ToEven), n => Math.Round(n));
        Test((n, mode) => Math.Round(n, 0, mode), (n, mode) => Math.Round(n, mode), TestValues, roundings);
        Test((n, digits) => Math.Round(n, digits, MidpointRounding.ToEven), (n, digits) => Math.Round(n, digits), TestValues, digitsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        double[] values = [..TestValues.Except([double.MinValue, double.MaxValue]), float.MinValue, float.MaxValue];

        Test(
            (double n, IFormatProvider? provider, out double result) => double.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse($"{n}", provider, out result),
            values,
            FormatProviders);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}", null, out result),
            (double n, out double result) => double.TryParse($"{n}", out result),
            values);

        Test(
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            FormatProviders);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), out result),
            values);

        Test(
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (double n, IFormatProvider? provider, out double result)
                => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            values,
            FormatProviders);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result),
            values);

        DoNamedTest2();
    }
}