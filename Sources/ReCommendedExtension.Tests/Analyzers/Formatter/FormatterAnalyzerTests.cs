using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Formatter;
using ReCommendedExtension.Extensions.NumberInfos;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.Formatter;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class FormatterAnalyzerTests : CSharpHighlightingTestBase
{
    readonly IFormatProvider?[] formatProviders = [null, CultureInfo.InvariantCulture, new CultureInfo("en-US"), new CultureInfo("de-DE")];

    protected override string RelativeTestDataPath => @"Analyzers\Formatter";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantFormatSpecifierHint
            or RedundantFormatProviderHint
            or RedundantFormatPrecisionSpecifierHint
            or PassOtherFormatSpecifierSuggestion
            or SuspiciousFormatSpecifierWarning
            or ReplaceTypeCastWithFormatSpecifierSuggestion
            or { IsError: true };

    static void Test<T>(Func<T, string?> expected, Func<T, string?> actual, T[] values)
    {
        foreach (var value in values)
        {
            Assert.AreEqual(expected(value), actual(value), $"with value: {value}");
        }
    }

    static void Test<T, U>(Func<T, U, string?> expected, Func<T, U, string?> actual, T[] xValues, U[] yValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                Assert.AreEqual(expected(x, y), actual(x, y), $"with values: {x}, {y}");
            }
        }
    }

    static void Test<T, U, V>(Func<T, U, V, string?> expected, Func<T, U, V, string?> actual, T[] xValues, U[] yValues, V[] zValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                foreach (var z in zValues)
                {
                    Assert.AreEqual(expected(x, y, z), actual(x, y, z), $"with values: {x}, {y}, {z}");
                }
            }
        }
    }

    void Test<T>(T[] values, bool providerAlwaysIgnored)
    {
        // value.ToString(provider)
        Test(value => value?.ToString(null as IFormatProvider), value => value?.ToString(), values);

        if (providerAlwaysIgnored)
        {
            Test((value, provider) => value?.ToString(provider), (value, _) => value?.ToString(), values, formatProviders);
        }
    }

    void Test<T>(
        T[] values,
        bool providerAlwaysIgnored,
        string?[] formatsRedundant,
        string[] formatsRedundantPrecision,
        string[] formatsRedundantProvider,
        string[] formatsRedundantPrecisionAndProvider) where T : struct
    {
        // value.ToString(provider)
        Test(values, providerAlwaysIgnored);

        // value.ToString(format)
        Test((value, format) => value.ToString(format), (value, _) => value.ToString(), values, formatsRedundant);
        Test(
            (value, format) => value.ToString(format),
            (value, format) => value.ToString($"{format[0]}"),
            values,
            [..formatsRedundantPrecision, ..formatsRedundantPrecisionAndProvider]);

        // value.ToString(format, provider)
        Test(
            (value, format) => value.ToString(format, null),
            (value, format) => value.ToString(format),
            values,
            [..formatsRedundant, ..formatsRedundantPrecision, ..formatsRedundantProvider, ..formatsRedundantPrecisionAndProvider]);
        Test(
            (value, format, provider) => value.ToString(format, provider),
            (value, _, provider) => value.ToString(provider),
            values,
            formatsRedundant,
            formatProviders);
        Test(
            (value, format, provider) => value.ToString(format, provider),
            (value, format, provider) => value.ToString($"{format[0]}", provider),
            values,
            formatsRedundantPrecision,
            formatProviders);
        Test(
            (value, format, provider) => value.ToString(format, provider),
            (value, format, _) => value.ToString($"{format[0]}"),
            values,
            formatsRedundantPrecisionAndProvider,
            formatProviders);
        Test(
            (value, format, provider) => value.ToString(format, provider),
            (value, format, _) => value.ToString(format),
            values,
            formatsRedundantProvider,
            formatProviders);

        if (providerAlwaysIgnored)
        {
            Test(
                (value, format, provider) => value.ToString(format, provider),
                (value, format, _) => value.ToString(format),
                values,
                [..formatsRedundant, ..formatsRedundantPrecision, ..formatsRedundantProvider, ..formatsRedundantPrecisionAndProvider],
                formatProviders);
        }
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestByte()
    {
        var values = new byte[] { 0, 1, 2, byte.MaxValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G3}", n => $"{n}", values);
        Test(n => $"{n:G4}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g3}", n => $"{n}", values);
        Test(n => $"{n:g4}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G3}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G4}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g3}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g4}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G3", "G4", "g", "g0", "g3", "g4"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestSByte()
    {
        var values = new sbyte[] { 0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G3}", n => $"{n}", values);
        Test(n => $"{n:G4}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g3}", n => $"{n}", values);
        Test(n => $"{n:g4}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G3}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G4}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g3}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g4}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G3", "G4", "g", "g0", "g3", "g4"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt16()
    {
        var values = new short[] { 0, 1, 2, -1, -2, short.MaxValue, short.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G5}", n => $"{n}", values);
        Test(n => $"{n:G6}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g5}", n => $"{n}", values);
        Test(n => $"{n:g6}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G5}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G6}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g5}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g6}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G5", "G6", "g", "g0", "g5", "g6"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt16()
    {
        var values = new ushort[] { 0, 1, 2, ushort.MaxValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G5}", n => $"{n}", values);
        Test(n => $"{n:G6}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g5}", n => $"{n}", values);
        Test(n => $"{n:g6}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G5}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G6}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g5}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g6}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G5", "G6", "g", "g0", "g5", "g6"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt32()
    {
        var values = new[] { 0, 1, 2, -1, -2, int.MaxValue, int.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G10}", n => $"{n}", values);
        Test(n => $"{n:G11}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g10}", n => $"{n}", values);
        Test(n => $"{n:g11}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G10}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G11}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g10}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g11}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G10", "G11", "g", "g0", "g10", "g11"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt32()
    {
        var values = new uint[] { 0, 1, 2, uint.MaxValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G10}", n => $"{n}", values);
        Test(n => $"{n:G11}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g10}", n => $"{n}", values);
        Test(n => $"{n:g11}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G10}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G11}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g10}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g11}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G10", "G11", "g", "g0", "g10", "g11"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt64()
    {
        var values = new[] { 0, 1, 2, -1, -2, long.MaxValue, long.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G19}", n => $"{n}", values);
        Test(n => $"{n:G20}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g19}", n => $"{n}", values);
        Test(n => $"{n:g20}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G19}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G20}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g19}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g20}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G19", "G20", "g", "g0", "g19", "g20"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt64()
    {
        var values = new ulong[] { 0, 1, 2, ulong.MaxValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G20}", n => $"{n}", values);
        Test(n => $"{n:G21}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g20}", n => $"{n}", values);
        Test(n => $"{n:g21}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G20}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G21}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g20}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g21}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G20", "G21", "g", "g0", "g20", "g21"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt128()
    {
        var values = new[] { 0, 1, 2, -1, -2, Int128.MaxValue, Int128.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G39}", n => $"{n}", values);
        Test(n => $"{n:G40}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g39}", n => $"{n}", values);
        Test(n => $"{n:g40}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G39}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G40}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g39}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g40}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G39", "G40", "g", "g0", "g39", "g40"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt128()
    {
        var values = new[] { 0, 1, 2, UInt128.MaxValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:G39}", n => $"{n}", values);
        Test(n => $"{n:G40}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:g36}", n => $"{n}", values);
        Test(n => $"{n:g40}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G39}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G40}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g39}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g40}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(
            values,
            false,
            [null, "", "G", "G0", "G39", "G40", "g", "g0", "g39", "g40"],
            ["E6", "e6", "D0", "D1", "d0", "d1"],
            ["X2", "x2"],
            ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "InterpolatedStringExpressionIsNotIFormattable")] // todo: remove when IntPtr implements IFormattable
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    [SuppressMessage("ReSharper", "FormatStringProblem")] // todo: remove when IntPtr implements IFormattable
    public void TestIntPtr()
    {
        var values = new[] { (nint)0, 1, 2, -1, -2 };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(values, false, [null, "", "G", "G0", "g", "g0"], ["E6", "e6", "D0", "D1", "d0", "d1"], ["X2", "x2"], ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "InterpolatedStringExpressionIsNotIFormattable")] // todo: remove when UIntPtr implements IFormattable
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    [SuppressMessage("ReSharper", "FormatStringProblem")] // todo: remove when UIntPtr implements IFormattable
    public void TestUIntPtr()
    {
        var values = new nuint[] { 0, 1, 2 };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:g0}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);
        Test(n => $"{n:D0}", n => $"{n:D}", values);
        Test(n => $"{n:D1}", n => $"{n:D}", values);
        Test(n => $"{n:d0}", n => $"{n:d}", values);
        Test(n => $"{n:d1}", n => $"{n:d}", values);
        Test(n => $"{n:X0}", n => $"{n:X}", values);
        Test(n => $"{n:X1}", n => $"{n:X}", values);
        Test(n => $"{n:x0}", n => $"{n:x}", values);
        Test(n => $"{n:x1}", n => $"{n:x}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);
        Test(n => string.Format("{0:D0}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:D1}", n), n => string.Format("{0:D}", n), values);
        Test(n => string.Format("{0:d0}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:d1}", n), n => string.Format("{0:d}", n), values);
        Test(n => string.Format("{0:X0}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:X1}", n), n => string.Format("{0:X}", n), values);
        Test(n => string.Format("{0:x0}", n), n => string.Format("{0:x}", n), values);
        Test(n => string.Format("{0:x1}", n), n => string.Format("{0:x}", n), values);

        Test(values, false, [null, "", "G", "G0", "g", "g0"], ["E6", "e6", "D0", "D1", "d0", "d1"], ["X2", "x2"], ["X0", "X1", "x0", "x1"]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestDecimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        Test(values, false, [null, "", "G", "g"], ["E6", "e6"], [], []);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestDouble()
    {
        var values = new[]
        {
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
        };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        Test(values, false, [null, "", "G", "G0"], ["E6", "e6"], [], []);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "RedundantFormatPrecisionSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestSingle()
    {
        var values = new[]
        {
            0,
            -0f,
            1,
            2,
            -1,
            -2,
            1.2f,
            -1.2f,
            float.MaxValue,
            float.MinValue,
            float.Epsilon,
            float.NaN,
            float.PositiveInfinity,
            float.NegativeInfinity,
        };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        Test(values, false, [null, "", "G", "G0"], ["E6", "e6"], [], []);

        DoNamedTest2();
    }

    [Test]
    [TestNet50]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestHalf()
    {
        var values = new[]
        {
            (sbyte)0,
            (sbyte)1,
            (sbyte)2,
            (sbyte)-1,
            (sbyte)-2,
            (Half)(-0f),
            (Half)1.2f,
            (Half)(-1.2f),
            Half.MaxValue,
            Half.MinValue,
            Half.Epsilon,
            Half.NaN,
            Half.PositiveInfinity,
            Half.NegativeInfinity,
        };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        Test(values, false, [null, "", "G", "G0"], ["E6", "e6"], [], []);

        DoNamedTest2();
    }

    [Test]
    public void TestBoolean()
    {
        var values = new[] { true, false };

        Test(values, true);

        DoNamedTest2();
    }

    [Test]
    public void TestChar()
    {
        var values = new[] { 'a', 'A', '1', ' ', 'ä', 'ß', '€', char.MinValue, char.MaxValue };

        Test(values, true);

        DoNamedTest2();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    enum SampleEnum
    {
        Red,
        Green,
        Blue,
    }

    [Flags]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    enum SampleFlags
    {
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestEnum()
    {
        var enumValues = new[] { SampleEnum.Red, (SampleEnum)1, (SampleEnum)10 };
        var flagValues = new[] { SampleFlags.Red, SampleFlags.Red | SampleFlags.Blue, (SampleFlags)3, (SampleFlags)0, (SampleFlags)9 };

        Test(e => $"{e:G}", e => $"{e}", enumValues);
        Test(e => $"{e:g}", e => $"{e}", enumValues);

        Test(e => $"{e:G}", e => $"{e}", flagValues);
        Test(e => $"{e:g}", e => $"{e}", flagValues);

        Test(e => string.Format("{0:G}", e), e => string.Format("{0}", e), enumValues);
        Test(e => string.Format("{0:g}", e), e => string.Format("{0}", e), enumValues);

        Test(e => string.Format("{0:G}", e), e => string.Format("{0}", e), flagValues);
        Test(e => string.Format("{0:g}", e), e => string.Format("{0}", e), flagValues);

        Test(enumValues, true, [null, "", "G", "g"], [], [], []);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestGuid()
    {
        var values = new[] { Guid.Empty, new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]) };

        Test(guid => $"{guid:D}", guid => $"{guid}", values);
        Test(guid => $"{guid:d}", guid => $"{guid}", values);

        Test(guid => string.Format("{0:D}", guid), guid => string.Format("{0}", guid), values);
        Test(guid => string.Format("{0:d}", guid), guid => string.Format("{0}", guid), values);

        Test(values, true, [null, "", "D", "d"], [], ["N", "B", "P", "X"], []);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantFormatSpecifier")]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestTimeSpan()
    {
        var values = new[]
        {
            TimeSpan.Zero,
            TimeSpan.MinValue,
            TimeSpan.MaxValue,
            new(0, 0, 1),
            new(0, 1, 0),
            new(1, 0, 0),
            new(1, 0, 0, 0, 1),
            new(0, 0, 0, 0, 1),
            new(1, 2, 3, 4),
            new(-1, 2, 3, 4),
        };

        Test(timeSpan => $"{timeSpan:c}", timeSpan => $"{timeSpan}", values);
        Test(timeSpan => $"{timeSpan:t}", timeSpan => $"{timeSpan}", values);
        Test(timeSpan => $"{timeSpan:T}", timeSpan => $"{timeSpan}", values);

        Test(timeSpan => string.Format("{0:c}", timeSpan), timeSpan => string.Format("{0}", timeSpan), values);
        Test(timeSpan => string.Format("{0:t}", timeSpan), timeSpan => string.Format("{0}", timeSpan), values);
        Test(timeSpan => string.Format("{0:T}", timeSpan), timeSpan => string.Format("{0}", timeSpan), values);

        Test(values, false, [null, "", "c", "t", "T"], [], ["c", "t", "T"], []);

        DoNamedTest2();
    }

    [Test]
    public void TestDateTime()
    {
        var values = new[]
        {
            DateTime.MinValue,
            DateTime.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
        };

        Test(values, false, [null, ""], [], ["o", "O", "r", "R", "s", "u"], []);

        DoNamedTest2();
    }

    [Test]
    public void TestDateTimeOffset()
    {
        var values = new[]
        {
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.Zero),
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.FromHours(2)),
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.FromHours(-6)),
        };

        Test(values, false, [null, ""], [], ["o", "O", "r", "R", "s", "u"], []);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestDateOnly()
    {
        var values = new[] { DateOnly.MinValue, DateOnly.MaxValue, new(2025, 7, 15) };

        Test(dateOnly => $"{dateOnly:d}", dateOnly => $"{dateOnly}", values);

        Test(dateOnly => string.Format("{0:d}", dateOnly), dateOnly => string.Format("{0}", dateOnly), values);

        Test(values, false, [null, "", "d"], [], ["o", "O", "r", "R"], []);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestTimeOnly()
    {
        var values = new[] { TimeOnly.MinValue, TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5) };

        Test(timeOnly => $"{timeOnly:t}", timeOnly => $"{timeOnly}", values);

        Test(timeOnly => string.Format("{0:t}", timeOnly), timeOnly => string.Format("{0}", timeOnly), values);

        Test(values, false, [null, "", "t"], [], ["o", "O", "r", "R"], []);

        DoNamedTest2();
    }

    [Test]
    public void TestString()
    {
        var values = new[] { null, "", "abcde" };

        Test(values, true);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ReplaceTypeCastWithFormatSpecifier")]
    public void TestCastEnum()
    {
        var values = new[] { SampleEnum.Red, (SampleEnum)1, (SampleEnum)10 };

        Test(e => $"{(int)e}", e => $"{e:D}", values);
        Test(e => $"{(int?)e}", e => $"{e:D}", values);
        Test<SampleEnum?>(e => $"{(int?)e}", e => $"{e:D}", [..values, null]);

        DoNamedTest2();
    }

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void TestInterpolatedStringHandler() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestStringFormatters() => DoNamedTest2();
}