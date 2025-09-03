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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.IntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class IntPtrAnalyzerTests : BaseTypeAnalyzerTests<nint>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\IntPtr";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
                or UseBinaryOperatorSuggestion
                or RedundantArgumentHint
                or SuspiciousFormatSpecifierWarning
                or RedundantFormatPrecisionSpecifierHint
            || highlighting.IsError();

    protected override nint[] TestValues { get; } = [0, 1, 2, -1, -2];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingIntPtrMethods.Clamp(number, 1, 1), _ => 1);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingIntPtrMethods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((nint)0, 10), () => (0, 0));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => MissingIntPtrMethods.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => MissingIntPtrMethods.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingIntPtrMethods.Max(n, n), n => n);
        Test(n => MissingMathMethods.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMaxMagnitude()
    {
        Test(n => MissingIntPtrMethods.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingIntPtrMethods.Min(n, n), n => n);
        Test(n => MissingMathMethods.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMinMagnitude()
    {
        Test(n => MissingIntPtrMethods.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.Integer), n => MissingIntPtrMethods.Parse($"{n}"));
        Test(n => MissingIntPtrMethods.Parse($"{n}", null), n => MissingIntPtrMethods.Parse($"{n}"));
        Test(
            (n, provider) => MissingIntPtrMethods.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => MissingIntPtrMethods.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(
            n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.AllowLeadingSign, null),
            n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingIntPtrMethods.Parse($"{n}".AsSpan(), null), n => MissingIntPtrMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingIntPtrMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingIntPtrMethods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public void TestToString()
    {
        var formatsRedundant = new[] { null, "", "G", "G0", "g", "g0" };
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
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", out result));

        Test(
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (nint n, IFormatProvider? provider, out nint result)
                => MissingIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}