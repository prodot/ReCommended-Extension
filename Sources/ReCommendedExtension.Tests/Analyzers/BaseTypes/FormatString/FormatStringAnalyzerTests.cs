using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.FormatString;

using int128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Int128;
using uint128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.UInt128;
using half = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Half;

[TestFixture]
public sealed class FormatStringAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\FormatString";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantFormatSpecifierHint
                or RedundantFormatPrecisionSpecifierHint
                or PassOtherFormatSpecifierSuggestion
                or SuspiciousFormatSpecifierWarning
            || highlighting.IsError();

    static void Test<T>(Func<T, string> expected, Func<T, string> actual, T[] values)
    {
        foreach (var value in values)
        {
            Assert.AreEqual(expected(value), actual(value));
        }
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestByte()
    {
        var values = new[] { (byte)0, (byte)1, (byte)2, byte.MaxValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestSByte()
    {
        var values = new[] { (sbyte)0, (sbyte)1, (sbyte)2, (sbyte)-1, sbyte.MaxValue, sbyte.MinValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt16()
    {
        var values = new[] { (short)0, (short)1, (short)2, (short)-1, short.MaxValue, short.MinValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt16()
    {
        var values = new[] { (ushort)0, (ushort)1, (ushort)2, ushort.MaxValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt32()
    {
        var values = new[] { 0, 1, 2, -1, int.MaxValue, int.MinValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt32()
    {
        var values = new[] { 0u, 1u, 2u, uint.MaxValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt64()
    {
        var values = new[] { 0, 1, 2, -1, long.MaxValue, long.MinValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt64()
    {
        var values = new[] { 0ul, 1ul, 2ul, ulong.MaxValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestInt128()
    {
        var values = new[] { 0, 1, 2, -1, int128.MaxValue, int128.MinValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestUInt128()
    {
        var values = new[] { 0, 1, 2, uint128.MaxValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    [SuppressMessage("ReSharper", "FormatStringProblem")] // todo: remove when IntPtr implements IFormattable
    public void TestIntPtr()
    {
        var values = new[] { (nint)0, 1, 2, -1 };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    [SuppressMessage("ReSharper", "FormatStringProblem")] // todo: remove when IntPtr implements IFormattable
    public void TestUIntPtr()
    {
        var values = new[] { (nuint)0, (nuint)1, (nuint)2 };

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

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestDecimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, decimal.MaxValue, decimal.MinValue };

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:g}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestDouble()
    {
        var values = new[]
        {
            0, -0d, 1, 2, -1, double.MaxValue, double.MinValue, double.Epsilon, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
        };

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public void TestSingle()
    {
        var values = new[]
        {
            0, -0f, 1, 2, -1, float.MaxValue, float.MinValue, float.Epsilon, float.NaN, float.PositiveInfinity, float.NegativeInfinity,
        };

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

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
            (half)(-0f),
            half.MaxValue,
            half.MinValue,
            half.Epsilon,
            half.NaN,
            half.PositiveInfinity,
            half.NegativeInfinity,
        };

        Test(n => string.Format("{0:G}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:G0}", n), n => string.Format("{0}", n), values);
        Test(n => string.Format("{0:E6}", n), n => string.Format("{0:E}", n), values);
        Test(n => string.Format("{0:e6}", n), n => string.Format("{0:e}", n), values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestStringFormatters() => DoNamedTest2();
}