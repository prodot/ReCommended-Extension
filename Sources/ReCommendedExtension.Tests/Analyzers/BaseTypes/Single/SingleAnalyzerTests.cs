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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Single;

[TestFixture]
public sealed class SingleAnalyzerTests : BaseTypeAnalyzerTests<float>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Single";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or RedundantArgumentHint
                or UseFloatingPointPatternSuggestion
                or PassOtherFormatSpecifierSuggestion
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override float[] TestValues { get; } =
    [
        0,
        -0f,
        1,
        2,
        -1,
        -2,
        1.2f,
        -1.2f,
        float.MinValue,
        float.MaxValue,
        float.Epsilon,
        float.NaN,
        float.PositiveInfinity,
        float.NegativeInfinity,
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
        Test(number => number.GetTypeCode(), _ => TypeCode.Single);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseFloatingPointPattern")]
    public void TestIsNaN()
    {
        Test(number => float.IsNaN(number), number => number is float.NaN);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        var values = TestValues.Except([float.MinValue, float.MaxValue]).ToArray();

        Test(n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => float.Parse($"{n}"), values);
        Test(n => float.Parse($"{n}", null), n => float.Parse($"{n}"), values);
        Test(
            (n, provider) => float.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => float.Parse(n.ToString(provider), provider),
            values,
            FormatProviders);
        Test(
            n => float.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, null),
            n => float.Parse($"{n}", NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint),
            values);

        Test(n => MissingSingleMethods.Parse($"{n}".AsSpan(), null), n => MissingSingleMethods.Parse($"{n}".AsSpan()), values);

        Test(
            n => MissingSingleMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingSingleMethods.Parse(Encoding.UTF8.GetBytes($"{n}")),
            values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestRound()
    {
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        Test(n => MissingSingleMethods.Round(n, 0), n => MissingSingleMethods.Round(n));
        Test(n => MissingSingleMethods.Round(n, MidpointRounding.ToEven), n => MissingSingleMethods.Round(n));
        Test((n, mode) => MissingSingleMethods.Round(n, 0, mode), (n, mode) => MissingSingleMethods.Round(n, mode), TestValues, roundings);
        Test(
            (n, digits) => MissingSingleMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingSingleMethods.Round(n, digits),
            TestValues,
            digitsValues);

        Test(n => MissingMathFMethods.Round(n, 0), n => MissingMathFMethods.Round(n));
        Test(n => MissingMathFMethods.Round(n, MidpointRounding.ToEven), n => MissingMathFMethods.Round(n));
        Test((n, mode) => MissingMathFMethods.Round(n, 0, mode), (n, mode) => MissingMathFMethods.Round(n, mode), TestValues, roundings);
        Test(
            (n, digits) => MissingMathFMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingMathFMethods.Round(n, digits),
            TestValues,
            digitsValues);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestToString()
    {
        var formatsRedundant = new[] { null, "", "G", "G0" };
        var formatsRedundantSpecifier = new[] { "E6", "e6" };

        Test((n, format) => n.ToString(format), (n, _) => n.ToString(), TestValues, formatsRedundant);
        Test((n, format) => n.ToString(format), (n, format) => n.ToString($"{format[0]}"), TestValues, formatsRedundantSpecifier);

        Test(
            (n, format) => n.ToString(format, null),
            (n, format) => n.ToString(format),
            TestValues,
            [..formatsRedundant, ..formatsRedundantSpecifier]);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, _, provider) => n.ToString(provider),
            TestValues,
            formatsRedundant,
            FormatProviders);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, format, provider) => n.ToString($"{format[0]}", provider),
            TestValues,
            formatsRedundantSpecifier,
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (float n, IFormatProvider? provider, out float result) => float.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}", null, out result),
            (float n, out float result) => float.TryParse($"{n}", out result));

        Test(
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (float n, IFormatProvider? provider, out float result)
                => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}