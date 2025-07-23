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
            (n, provider) => ushort.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => ushort.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
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
        var formatsRedundant = new[] { null, "", "G", "G0", "G5", "G6", "g", "g0", "g5", "g6" };
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
            (ushort n, IFormatProvider? provider, out ushort result) => ushort.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}", null, out result),
            (ushort n, out ushort result) => ushort.TryParse($"{n}", out result));

        Test(
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (ushort n, IFormatProvider? provider, out ushort result)
                => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}