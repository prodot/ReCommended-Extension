using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
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
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override half[] TestValues { get; } =
    [
        (sbyte)0,
        (sbyte)1,
        (sbyte)2,
        -1,
        -2,
        (half)(-0f),
        (half)1.2f,
        (half)(-1.2f),
        half.MaxValue,
        half.Epsilon,
        half.NaN,
        half.PositiveInfinity,
        half.NegativeInfinity,
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
            (n, provider) => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => half.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
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
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        Test(n => MissingHalfMethods.Round(n, 0), n => MissingHalfMethods.Round(n));
        Test(n => MissingHalfMethods.Round(n, MidpointRounding.ToEven), n => MissingHalfMethods.Round(n));
        Test((n, mode) => MissingHalfMethods.Round(n, 0, mode), (n, mode) => MissingHalfMethods.Round(n, mode), TestValues, roundings);
        Test(
            (n, digits) => MissingHalfMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingHalfMethods.Round(n, digits),
            TestValues,
            digitsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (half n, IFormatProvider? provider, out half result) => half.TryParse(
                $"{n}",
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (half n, IFormatProvider? provider, out half result) => half.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test((half n, out half result) => half.TryParse($"{n}", null, out result), (half n, out half result) => half.TryParse($"{n}", out result));

        Test(
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}