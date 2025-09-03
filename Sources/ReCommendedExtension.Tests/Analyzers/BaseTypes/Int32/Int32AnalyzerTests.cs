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

    protected override int[] TestValues { get; } = [0, 1, 2, -1, -2, int.MinValue, int.MaxValue];

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
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
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
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => int.Parse($"{n}", NumberStyles.Integer), n => int.Parse($"{n}"));
        Test(n => int.Parse($"{n}", null), n => int.Parse($"{n}"));
        Test(
            (n, provider) => int.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => int.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
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
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestToString()
    {
        var formatsRedundant = new[] { null, "", "G", "G0", "G10", "G11", "g", "g0", "g10", "g11" };
        var formatsRedundantSpecifier = new[] { "E6", "e6", "D0", "D1", "d0", "d1" };
        var formatsRedundantProvider = new[] { "X2", "x2" };
        var formatsRedundantSpecifierAndProvider = new[] { "X0", "X1", "x0", "x1" };

        Test((n, format) => n.ToString(format), (n, _) => n.ToString(), TestValues, formatsRedundant);
        Test(
            (n, format) => n.ToString(format),
            (n, format) => n.ToString($"{format[0]}"),
            TestValues,
            [..formatsRedundantSpecifier, ..formatsRedundantSpecifierAndProvider]);

        Test(n => n.ToString(null as IFormatProvider), n => n.ToString());

        Test(
            (n, format) => n.ToString(format, null),
            (n, format) => n.ToString(format),
            TestValues,
            [..formatsRedundant, ..formatsRedundantSpecifier, ..formatsRedundantProvider, ..formatsRedundantSpecifierAndProvider]);
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
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, format, _) => n.ToString($"{format[0]}"),
            TestValues,
            formatsRedundantSpecifierAndProvider,
            FormatProviders);
        Test(
            (n, format, provider) => n.ToString(format, provider),
            (n, format, _) => n.ToString(format),
            TestValues,
            formatsRedundantProvider,
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (int n, IFormatProvider? provider, out int result) => int.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}", null, out result),
            (int n, out int result) => int.TryParse($"{n}", out result));

        Test(
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), null, out result),
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (int n, out int result) => MissingInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}