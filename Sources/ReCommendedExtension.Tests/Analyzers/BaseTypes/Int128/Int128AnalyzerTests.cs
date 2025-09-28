using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int128;

using int128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Int128;

[TestFixture]
[TestNet70]
public sealed class Int128AnalyzerTests : BaseTypeAnalyzerTests<int128>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override int128[] TestValues { get; } = [0, 1, 2, -1, -2, int128.MinValue, int128.MaxValue];

    [Test]
    public void TestClamp()
    {
        Test(number => int128.Clamp(number, 1, 1), _ => 1);
        Test(number => int128.Clamp(number, int128.MinValue, int128.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    public void TestDivRem()
    {
        Test(() => int128.DivRem(0, 10), () => (0, 0));

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
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => int128.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => int128.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    public void TestMax()
    {
        Test(n => int128.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMaxMagnitude()
    {
        Test(n => int128.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMin()
    {
        Test(n => int128.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMinMagnitude()
    {
        Test(n => int128.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => int128.Parse($"{n}", NumberStyles.Integer), n => int128.Parse($"{n}"));
        Test(n => int128.Parse($"{n}", null), n => int128.Parse($"{n}"));
        Test(
            (n, provider) => int128.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => int128.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => int128.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => int128.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingInt128Methods.Parse($"{n}".AsSpan(), null), n => MissingInt128Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    public void TestRotateLeft()
    {
        Test(n => int128.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestRotateRight()
    {
        Test(n => int128.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (int128 n, IFormatProvider? provider, out int128 result) => int128.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (int128 n, IFormatProvider? provider, out int128 result) => int128.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (int128 n, out int128 result) => int128.TryParse($"{n}", null, out result),
            (int128 n, out int128 result) => int128.TryParse($"{n}", out result));

        Test(
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (int128 n, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), null, out result),
            (int128 n, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                provider,
                out result),
            TestValues,
            FormatProviders);
        Test(
            (int128 n, out int128 result) => MissingInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (int128 n, out int128 result) => MissingInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}