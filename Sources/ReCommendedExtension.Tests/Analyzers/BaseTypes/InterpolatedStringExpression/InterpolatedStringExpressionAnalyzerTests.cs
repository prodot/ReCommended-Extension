using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.InterpolatedStringExpression;

using int128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Int128;
using uint128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.UInt128;
using half = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Half;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class InterpolatedStringExpressionAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\InterpolatedStringExpression";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantFormatSpecifierHint
                or RedundantFormatPrecisionSpecifierHint
                or PassOtherFormatSpecifierSuggestion
                or SuspiciousFormatSpecifierWarning
                or ReplaceTypeCastWithFormatSpecifierSuggestion
            || highlighting.IsError();

    static void Test<T>(Func<T, string> expected, Func<T, string> actual, T[] values)
    {
        foreach (var value in values)
        {
            Assert.AreEqual(expected(value), actual(value), $"with values: {value}");
        }
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestInt128()
    {
        var values = new[] { 0, 1, 2, -1, -2, int128.MaxValue, int128.MinValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestUInt128()
    {
        var values = new[] { 0, 1, 2, uint128.MaxValue };

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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "InterpolatedStringExpressionIsNotIFormattable")] // todo: remove when IntPtr implements IFormattable
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

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "InterpolatedStringExpressionIsNotIFormattable")] // todo: remove when UIntPtr implements IFormattable
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

        DoNamedTest2();
    }

    [Test]
    public void TestDecimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:g}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);

        DoNamedTest2();
    }

    [Test]
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

        DoNamedTest2();
    }

    [Test]
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

        DoNamedTest2();
    }

    [Test]
    [TestNet50]
    public void TestHalf()
    {
        var values = new[]
        {
            (sbyte)0,
            (sbyte)1,
            (sbyte)2,
            (sbyte)-1,
            (sbyte)-2,
            (half)(-0f),
            (half)1.2f,
            (half)(-1.2f),
            half.MaxValue,
            half.MinValue,
            half.Epsilon,
            half.NaN,
            half.PositiveInfinity,
            half.NegativeInfinity,
        };

        Test(n => $"{n:G}", n => $"{n}", values);
        Test(n => $"{n:G0}", n => $"{n}", values);
        Test(n => $"{n:E6}", n => $"{n:E}", values);
        Test(n => $"{n:e6}", n => $"{n:e}", values);

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
    public void TestEnum()
    {
        var enumValues = new[] { SampleEnum.Red, (SampleEnum)1, (SampleEnum)10 };
        var flagValues = new[] { SampleFlags.Red, SampleFlags.Red | SampleFlags.Blue, (SampleFlags)3, (SampleFlags)0, (SampleFlags)9 };

        Test(e => $"{e:G}", e => $"{e}", enumValues);
        Test(e => $"{e:g}", e => $"{e}", enumValues);

        Test(e => $"{e:G}", e => $"{e}", flagValues);
        Test(e => $"{e:g}", e => $"{e}", flagValues);

        DoNamedTest2();
    }

    [Test]
    public void TestGuid()
    {
        var values = new[] { System.Guid.Empty, new System.Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]) };

        Test(guid => $"{guid:D}", guid => $"{guid}", values);
        Test(guid => $"{guid:d}", guid => $"{guid}", values);

        DoNamedTest2();
    }

    [Test]
    public void TestTimeSpan()
    {
        var values = new[]
        {
            System.TimeSpan.Zero,
            System.TimeSpan.MinValue,
            System.TimeSpan.MaxValue,
            new(0, 0, 1),
            new(0, 1, 0),
            new(1, 0, 0),
            new(1, 0, 0, 0, 0),
            new(0, 0, 0, 0, 1),
            new(1, 2, 3, 4),
            new(-1, 2, 3, 4),
        };

        Test(timeSpan => $"{timeSpan:c}", timeSpan => $"{timeSpan}", values);
        Test(timeSpan => $"{timeSpan:t}", timeSpan => $"{timeSpan}", values);
        Test(timeSpan => $"{timeSpan:T}", timeSpan => $"{timeSpan}", values);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestDateOnly()
    {
        var values = new[] { Missing.DateOnly.MinValue, Missing.DateOnly.MaxValue, new(2025, 7, 15) };

        Test(dateOnly => $"{dateOnly:d}", dateOnly => $"{dateOnly}", values);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestTimeOnly()
    {
        var values = new[] { Missing.TimeOnly.MinValue, Missing.TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5) };

        Test(timeOnly => $"{timeOnly:t}", timeOnly => $"{timeOnly}", values);

        DoNamedTest2();
    }

    [Test]
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
    [TestNetFramework46]
    public void TestStringFormatters() => DoNamedTest2();
}