using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ExpressionResult;
using ReCommendedExtension.Tests.Missing;

using int128 = ReCommendedExtension.Extensions.NumberInfos.Int128;
using uint128 = ReCommendedExtension.Extensions.NumberInfos.UInt128;
using half = ReCommendedExtension.Extensions.NumberInfos.Half;
using dateOnly = ReCommendedExtension.Tests.Missing.DateOnly;
using timeOnly = ReCommendedExtension.Tests.Missing.TimeOnly;

namespace ReCommendedExtension.Tests.Analyzers.ExpressionResult;

[TestFixture]
public sealed class ExpressionResultAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ExpressionResult";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is UseExpressionResultSuggestion;

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void TestRandom<R>(Func<Random, R> expected, Func<Random, R> actual) => Test(() => expected(new Random()), () => actual(new Random()));

    static void Test<T, R>(Func<T, R> expected, Func<T, R> actual, T[] args)
    {
        foreach (var a in args)
        {
            Assert.AreEqual(expected(a), actual(a), $"with value: {a}");
        }
    }

    static void Test<T, U, R>(Func<T, U, R> expected, Func<T, U, R> actual, T[] args1, U[] args2)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                Assert.AreEqual(expected(a, b), actual(a, b), $"with values: {a}, {b}");
            }
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Boolean()
    {
        bool[] values = [true, false];

        // expression result

        Test(flag => flag.Equals(false), flag => !flag, values);
        Test(flag => true.Equals(flag), flag => flag, values);
        Test(flag => false.Equals(flag), flag => !flag, values);
        Test(flag => flag.Equals(null), _ => false, values);

        Test(flag => flag.GetTypeCode(), _ => TypeCode.Boolean, values);

        DoNamedTest();
    }


    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Byte()
    {
        byte[] values = [0, 1, 2, byte.MaxValue];

        // expression result

        Test(right => byte.DivRem(0, right), _ => (0, 0), [..values.Except(new byte[1])]);

        Test(n => byte.RotateLeft(n, 0), n => n, values);
        Test(n => byte.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => byte.Clamp(n, 1, 1), _ => 1, values);
        Test(n => byte.Clamp(n, 0, byte.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Byte, values);

        Test(n => byte.Max(n, n), n => n, values);
        Test(n => byte.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void SByte()
    {
        sbyte[] values = [0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue];

        // expression result

        Test(right => sbyte.DivRem(0, right), _ => (0, 0), [..values.Except(new sbyte[1])]);

        Test(n => sbyte.RotateLeft(n, 0), n => n, values);
        Test(n => sbyte.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => sbyte.Clamp(n, 1, 1), _ => 1, values);
        Test(n => sbyte.Clamp(n, sbyte.MinValue, sbyte.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.SByte, values);

        Test(n => sbyte.Max(n, n), n => n, values);
        Test(n => sbyte.Min(n, n), n => n, values);

        Test(n => sbyte.MaxMagnitude(n, n), n => n, values);
        Test(n => sbyte.MinMagnitude(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Int16()
    {
        short[] values = [0, 1, 2, -1, -2, short.MaxValue, short.MinValue];

        // expression result

        Test(right => short.DivRem(0, right), _ => (0, 0), [..values.Except(new short[1])]);

        Test(n => short.RotateLeft(n, 0), n => n, values);
        Test(n => short.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => short.Clamp(n, 1, 1), _ => 1, values);
        Test(n => short.Clamp(n, short.MinValue, short.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Int16, values);

        Test(n => short.Max(n, n), n => n, values);
        Test(n => short.Min(n, n), n => n, values);

        Test(n => short.MaxMagnitude(n, n), n => n, values);
        Test(n => short.MinMagnitude(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void UInt16()
    {
        ushort[] values = [0, 1, 2, ushort.MaxValue];

        // expression result

        Test(right => ushort.DivRem(0, right), _ => (0, 0), [..values.Except(new ushort[1])]);

        Test(n => ushort.RotateLeft(n, 0), n => n, values);
        Test(n => ushort.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => ushort.Clamp(n, 1, 1), _ => 1, values);
        Test(n => ushort.Clamp(n, 0, ushort.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.UInt16, values);

        Test(n => ushort.Max(n, n), n => n, values);
        Test(n => ushort.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Int32()
    {
        int[] values = [0, 1, 2, -1, -2, int.MaxValue, int.MinValue];

        // expression result

        Test(right => int.DivRem(0, right), _ => (0, 0), [..values.Except(new int[1])]);

        Test(n => int.RotateLeft(n, 0), n => n, values);
        Test(n => int.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => int.Clamp(n, 1, 1), _ => 1, values);
        Test(n => int.Clamp(n, int.MinValue, int.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Int32, values);

        Test(n => int.Max(n, n), n => n, values);
        Test(n => int.Min(n, n), n => n, values);

        Test(n => int.MaxMagnitude(n, n), n => n, values);
        Test(n => int.MinMagnitude(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void UInt32()
    {
        uint[] values = [0, 1, 2, uint.MaxValue];

        // expression result

        Test(right => uint.DivRem(0, right), _ => (0u, 0u), [..values.Except(new uint[1])]);

        Test(n => uint.RotateLeft(n, 0), n => n, values);
        Test(n => uint.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => uint.Clamp(n, 1, 1), _ => 1u, values);
        Test(n => uint.Clamp(n, 0, uint.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.UInt32, values);

        Test(n => uint.Max(n, n), n => n, values);
        Test(n => uint.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Int64()
    {
        long[] values = [0, 1, 2, -1, -2, long.MaxValue, long.MinValue];

        // expression result

        Test(right => long.DivRem(0, right), _ => (0, 0), [..values.Except(new long[1])]);

        Test(n => long.RotateLeft(n, 0), n => n, values);
        Test(n => long.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => long.Clamp(n, 1, 1), _ => 1, values);
        Test(n => long.Clamp(n, long.MinValue, long.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Int64, values);

        Test(n => long.Max(n, n), n => n, values);
        Test(n => long.Min(n, n), n => n, values);

        Test(n => long.MaxMagnitude(n, n), n => n, values);
        Test(n => long.MinMagnitude(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void UInt64()
    {
        ulong[] values = [0, 1, 2, ulong.MaxValue];

        // expression result

        Test(right => ulong.DivRem(0, right), _ => (0ul, 0ul), [..values.Except(new ulong[1])]);

        Test(n => ulong.RotateLeft(n, 0), n => n, values);
        Test(n => ulong.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => ulong.Clamp(n, 1, 1), _ => 1ul, values);
        Test(n => ulong.Clamp(n, 0, ulong.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.UInt64, values);

        Test(n => ulong.Max(n, n), n => n, values);
        Test(n => ulong.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    public void Int128()
    {
        int128[] values = [0, 1, 2, -1, -2, int128.MaxValue, int128.MinValue];

        // expression result

        Test(right => int128.DivRem(0, right), _ => (0, 0), [..values.Except(new int128[1])]);

        Test(n => int128.RotateLeft(n, 0), n => n, values);
        Test(n => int128.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => int128.Clamp(n, 1, 1), _ => 1, values);
        Test(n => int128.Clamp(n, int128.MinValue, int128.MaxValue), n => n, values);

        Test(n => int128.Max(n, n), n => n, values);
        Test(n => int128.Min(n, n), n => n, values);

        Test(n => int128.MaxMagnitude(n, n), n => n, values);
        Test(n => int128.MinMagnitude(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    public void UInt128()
    {
        uint128[] values = [0, 1, 2, uint128.MaxValue];

        // expression result

        Test(right => uint128.DivRem(0, right), _ => (0, 0), [..values.Except(new uint128[1])]);

        Test(n => uint128.RotateLeft(n, 0), n => n, values);
        Test(n => uint128.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => uint128.Clamp(n, 1, 1), _ => 1, values);
        Test(n => uint128.Clamp(n, 0, uint128.MaxValue), n => n, values);

        Test(n => uint128.Max(n, n), n => n, values);
        Test(n => uint128.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void IntPtr()
    {
        nint[] values = [0, 1, 2, -1, -2];

        // expression result

        Test(right => nint.DivRem(0, right), _ => (0, 0), [..values.Except(new nint[1])]);

        Test(n => nint.RotateLeft(n, 0), n => n, values);
        Test(n => nint.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => nint.Clamp(n, 1, 1), _ => 1, values);

        Test(n => nint.Max(n, n), n => n, values);
        Test(n => nint.Min(n, n), n => n, values);

        Test(n => nint.MaxMagnitude(n, n), n => n, values);
        Test(n => nint.MinMagnitude(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void UIntPtr()
    {
        nuint[] values = [0, 1, 2];

        // expression result

        Test(right => nuint.DivRem(0, right), _ => (0u, 0u), [..values.Except(new nuint[1])]);

        Test(n => nuint.RotateLeft(n, 0), n => n, values);
        Test(n => nuint.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => nuint.Clamp(n, 1, 1), _ => 1u, values);

        Test(n => nuint.Max(n, n), n => n, values);
        Test(n => nuint.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Decimal()
    {
        decimal[] values = [0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue];

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        Test(n => decimal.Clamp(n, 1, 1), _ => 1, values);
        Test(n => decimal.Clamp(n, decimal.MinValue, decimal.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Decimal, values);

        Test(n => decimal.Max(n, n), n => n, values);
        Test(n => decimal.Min(n, n), n => n, values);

        DoNamedTest();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Double()
    {
        double[] values =
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

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Double, values);

        DoNamedTest();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Single()
    {
        float[] values =
        [
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
        ];

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Single, values);

        DoNamedTest();
    }

    [Test]
    [TestNet50]
    public void Half()
    {
        half[] values =
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
            half.MinValue,
            half.Epsilon,
            half.NaN,
            half.PositiveInfinity,
            half.NegativeInfinity,
        ];

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet60]
    public void Math()
    {
        byte[] byteValues = [0, 1, 2, byte.MaxValue];
        sbyte[] sbyteValues = [0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue];
        short[] int16Values = [0, 1, 2, -1, -2, short.MaxValue, short.MinValue];
        ushort[] uint16Values = [0, 1, 2, ushort.MaxValue];
        int[] int32Values = [0, 1, 2, -1, -2, int.MaxValue, int.MinValue];
        uint[] uint32Values = [0, 1, 2, uint.MaxValue];
        long[] int64Values = [0, 1, 2, -1, -2, long.MaxValue, long.MinValue];
        ulong[] uint64Values = [0, 1, 2, ulong.MaxValue];
        nint[] intPtrValues = [0, 1, 2, -1, -2];
        nuint[] uintPtrValues = [0, 1, 2];
        decimal[] decimalValues = [0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue];

        // expression result

        Test(right => System.Math.DivRem((byte)0, right), _ => (0, 0), [..byteValues.Except(new byte[1])]);
        Test(right => System.Math.DivRem((sbyte)0, right), _ => (0, 0), [..sbyteValues.Except(new sbyte[1])]);
        Test(right => System.Math.DivRem((short)0, right), _ => (0, 0), [..int16Values.Except(new short[1])]);
        Test(right => System.Math.DivRem((ushort)0, right), _ => (0, 0), [..uint16Values.Except(new ushort[1])]);
        Test(right => System.Math.DivRem(0, right), _ => (0, 0), [..int32Values.Except(new int[1])]);
        Test(right => System.Math.DivRem(0, right), _ => (0u, 0u), [..uint32Values.Except(new uint[1])]);
        Test(right => System.Math.DivRem(0, right), _ => (0, 0), [..int64Values.Except(new long[1])]);
        Test(right => System.Math.DivRem(0, right), _ => (0ul, 0ul), [..uint64Values.Except(new ulong[1])]);
        Test(right => System.Math.DivRem(0, right), _ => (0, 0), [..intPtrValues.Except(new nint[1])]);
        Test(right => System.Math.DivRem(0, right), _ => (0u, 0u), [..uintPtrValues.Except(new nuint[1])]);

        Test(value => System.Math.Clamp(value, (byte)1, (byte)1), _ => 1, byteValues);
        Test(value => System.Math.Clamp(value, (sbyte)1, (sbyte)1), _ => 1, sbyteValues);
        Test(value => System.Math.Clamp(value, (short)1, (short)1), _ => 1, int16Values);
        Test(value => System.Math.Clamp(value, (ushort)1, (ushort)1), _ => 1, uint16Values);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1, int32Values);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1u, uint32Values);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1, int64Values);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1ul, uint64Values);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1, intPtrValues);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1u, uintPtrValues);
        Test(value => System.Math.Clamp(value, 1, 1), _ => 1, decimalValues);

        Test(value => System.Math.Clamp(value, byte.MinValue, byte.MaxValue), value => value, byteValues);
        Test(value => System.Math.Clamp(value, sbyte.MinValue, sbyte.MaxValue), value => value, sbyteValues);
        Test(value => System.Math.Clamp(value, short.MinValue, short.MaxValue), value => value, int16Values);
        Test(value => System.Math.Clamp(value, ushort.MinValue, ushort.MaxValue), value => value, uint16Values);
        Test(value => System.Math.Clamp(value, int.MinValue, int.MaxValue), value => value, int32Values);
        Test(value => System.Math.Clamp(value, uint.MinValue, uint.MaxValue), value => value, uint32Values);
        Test(value => System.Math.Clamp(value, long.MinValue, long.MaxValue), value => value, int64Values);
        Test(value => System.Math.Clamp(value, ulong.MinValue, ulong.MaxValue), value => value, uint64Values);
        Test(value => System.Math.Clamp(value, decimal.MinValue, decimal.MaxValue), value => value, decimalValues);

        Test(n => System.Math.Max(n, n), n => n, byteValues);
        Test(n => System.Math.Max(n, n), n => n, sbyteValues);
        Test(n => System.Math.Max(n, n), n => n, int16Values);
        Test(n => System.Math.Max(n, n), n => n, uint16Values);
        Test(n => System.Math.Max(n, n), n => n, int32Values);
        Test(n => System.Math.Max(n, n), n => n, uint32Values);
        Test(n => System.Math.Max(n, n), n => n, int64Values);
        Test(n => System.Math.Max(n, n), n => n, uint64Values);
        Test(n => MissingMathMethods.Max(n, n), n => n, intPtrValues);
        Test(n => MissingMathMethods.Max(n, n), n => n, uintPtrValues);
        Test(n => System.Math.Max(n, n), n => n, decimalValues);

        Test(n => System.Math.Min(n, n), n => n, byteValues);
        Test(n => System.Math.Min(n, n), n => n, sbyteValues);
        Test(n => System.Math.Min(n, n), n => n, int16Values);
        Test(n => System.Math.Min(n, n), n => n, uint16Values);
        Test(n => System.Math.Min(n, n), n => n, int32Values);
        Test(n => System.Math.Min(n, n), n => n, uint32Values);
        Test(n => System.Math.Min(n, n), n => n, int64Values);
        Test(n => System.Math.Min(n, n), n => n, uint64Values);
        Test(n => MissingMathMethods.Min(n, n), n => n, intPtrValues);
        Test(n => MissingMathMethods.Min(n, n), n => n, uintPtrValues);
        Test(n => System.Math.Min(n, n), n => n, decimalValues);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void DateTime()
    {
        DateTime[] values =
        [
            System.DateTime.MinValue,
            System.DateTime.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
        ];

        // expression result

        Test(() => new DateTime(0), () => System.DateTime.MinValue);

        Test(dateTime => dateTime.Equals(null), _ => false, values);

        Test(dateTime => dateTime.GetTypeCode(), _ => TypeCode.DateTime, values);

        DoNamedTest();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void DateTimeOffset()
    {
        DateTimeOffset[] values =
        [
            System.DateTimeOffset.MinValue,
            System.DateTimeOffset.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.Zero),
            new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(2)),
            new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(-6)),
        ];

        // expression result

        Test(dateTimeOffset => dateTimeOffset.Equals(null), _ => false, values);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet100]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TimeSpan()
    {
        TimeSpan[] values =
        [
            System.TimeSpan.Zero,
            System.TimeSpan.MinValue,
            System.TimeSpan.MaxValue,
            new(0, 0, 1),
            new(0, 1, 0),
            new(1, 0, 0),
            new(1, 0, 0, 0, 1),
            new(0, 0, 0, 0, 1),
            new(1, 2, 3, 4),
            new(-1, 2, 3, 4),
        ];

        // expression result

        Test(() => new TimeSpan(0), () => System.TimeSpan.Zero);
        Test(() => new TimeSpan(long.MinValue), () => System.TimeSpan.MinValue);
        Test(() => new TimeSpan(long.MaxValue), () => System.TimeSpan.MaxValue);
        Test(() => new TimeSpan(0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new TimeSpan(0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new TimeSpan(0, 0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan._Ctor(0, 0, 0, 0, 0, 0), () => System.TimeSpan.Zero);

        Test(timeSpan => timeSpan.Equals(null), _ => false, values);

        Test(() => MissingTimeSpanMembers.FromDays(0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan.FromDays(0, 0), () => System.TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromHours(0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan.FromHours(0, 0), () => System.TimeSpan.Zero);

        Test(() => System.TimeSpan.FromMicroseconds(0), () => System.TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromMilliseconds(0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan.FromMilliseconds(0, 0), () => System.TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromMinutes(0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan.FromMinutes(0, 0), () => System.TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromSeconds(0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan.FromSeconds(0, 0), () => System.TimeSpan.Zero);

        Test(() => System.TimeSpan.FromTicks(0), () => System.TimeSpan.Zero);
        Test(() => System.TimeSpan.FromTicks(long.MinValue), () => System.TimeSpan.MinValue);
        Test(() => System.TimeSpan.FromTicks(long.MaxValue), () => System.TimeSpan.MaxValue);

        DoNamedTest();
    }

    [Test]
    [TestNet90]
    public void TimeSpan_Net9() => DoNamedTest();

    [Test]
    [TestNet60]
    public void DateOnly()
    {
        dateOnly[] values = [dateOnly.MinValue, dateOnly.MaxValue, new(2025, 7, 15)];

        // expression result

        Test(dateOnly => dateOnly.Equals(null), _ => false, values);

        DoNamedTest();
    }

    [Test]
    [TestNet70]
    public void TimeOnly()
    {
        timeOnly[] values = [timeOnly.MinValue, timeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5)];

        // expression result

        Test(() => new timeOnly(0), () => timeOnly.MinValue);
        Test(() => new timeOnly(0, 0), () => timeOnly.MinValue);
        Test(() => new timeOnly(0, 0, 0), () => timeOnly.MinValue);
        Test(() => new timeOnly(0, 0, 0, 0), () => timeOnly.MinValue);
        Test(() => new timeOnly(0, 0, 0, 0, 0), () => timeOnly.MinValue);

        Test(timeOnly => timeOnly.Equals(null), _ => false, values);

        DoNamedTest();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Guid()
    {
        Guid[] values = [System.Guid.Empty, new([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16])];

        // expression result

        Test(guid => guid.Equals(null), _ => false, values);

        DoNamedTest();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Char()
    {
        char[] values = ['a', 'A', '1', ' ', 'ä', 'ß', '€', char.MinValue, char.MaxValue];

        // expression result

        Test(c => c.Equals(null), _ => false, values);

        Test(c => c.GetTypeCode(), _ => TypeCode.Char, values);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "StringEndsWithIsCultureSpecific")]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "StringStartsWithIsCultureSpecific")]
    public void String()
    {
        string[] values = ["", "abcde", "  abcde  ", "ab;cd;e", "ab;cd:e", "..abcde.."];
        StringComparison[] comparisons =
        [
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase,
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
        ];
        StringSplitOptions[] stringSplitOptions = [StringSplitOptions.None, StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.TrimEntries];

        // expression result

        Test(text => text.Contains(""), _ => true, values);
        Test((text, comparisonType) => text.Contains("", comparisonType), (_, _) => true, values, comparisons);

        Test(text => text.EndsWith(""), _ => true, values);
        Test((text, comparisonType) => text.EndsWith("", comparisonType), (_, _) => true, values, comparisons);

        Test(text => text.GetTypeCode(), _ => TypeCode.String, values);

        Test(text => text.IndexOf(""), _ => 0, values);
        Test((text, comparisonType) => text.IndexOf("", comparisonType), (_, _) => 0, values, comparisons);

        Test(text => text.IndexOfAny([]), _ => -1, values);

        Test(() => string.Join(';', (IEnumerable<int>)[]), () => "");
        Test(() => string.Join(';', (IEnumerable<int>)[100]), () => $"{100}");
        Test(() => string.Join(';', (string?[])[]), () => "");
        Test(() => string.Join(';', (string?[])["item"]), () => "item");
        Test(() => string.Join(';', (object?[])[]), () => "");
        Test(() => string.Join(';', (object?[])[true]), () => $"{true}");
        Test(() => string.Join(',', (ReadOnlySpan<string?>)[]), () => "");
        Test(() => string.Join(',', (ReadOnlySpan<string?>)["item"]), () => "item");
        Test(() => string.Join(',', (ReadOnlySpan<object?>)[]), () => "");
        Test(() => string.Join(',', (ReadOnlySpan<object?>)[true]), () => $"{true}");
        Test(() => string.Join("; ", (IEnumerable<int>)[]), () => "");
        Test(() => string.Join("; ", (IEnumerable<int>)[100]), () => $"{100}");
        Test(() => string.Join("; ", (string?[])[]), () => "");
        Test(() => string.Join("; ", (string?[])["item"]), () => "item");
        Test(() => string.Join("; ", (object?[])[]), () => "");
        Test(() => string.Join("; ", (object?[])[true]), () => $"{true}");
        Test(() => string.Join("; ", (ReadOnlySpan<string?>)[]), () => "");
        Test(() => string.Join("; ", (ReadOnlySpan<string?>)["item"]), () => "item");
        Test(() => string.Join("; ", (ReadOnlySpan<object?>)[]), () => "");
        Test(() => string.Join("; ", (ReadOnlySpan<object?>)[true]), () => $"{true}");
        Test(() => string.Join(';', (string?[])["item1", "item2"], 0, 0), () => "");
        Test(() => string.Join(';', (string?[])["item"], 1, 0), () => "");
        Test(() => string.Join(';', (string?[])["item"], 0, 1), () => "item");
        Test(() => string.Join(", ", (string?[])["item1", "item2"], 0, 0), () => "");
        Test(() => string.Join(", ", (string?[])["item"], 1, 0), () => "");
        Test(() => string.Join(", ", (string?[])["item"], 0, 1), () => "item");

        Test(text => text.LastIndexOf('c', 0), _ => -1, values);

        Test(text => text.LastIndexOfAny([]), _ => -1, values);
        Test(text => text.LastIndexOfAny(['c', 'd'], 0), _ => -1, values);
        Test(text => text.LastIndexOfAny(['c', 'd'], 0, 0), _ => -1, values);
        Test(text => text.LastIndexOfAny(['c', 'd'], 0, 1), _ => -1, values);

        Test(text => text.Remove(0), _ => "", [..values.Except([""])]); // frameworks earlier than .NET 6 throw an exception for '"".Remove(0)'

        Test(text => text.Split(null as string), text => [text], values);
        Test(text => text.Split(""), text => [text], values);
        Test(text => text.Split(null as string, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split("", StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split([';'], 0), _ => [], values);
        Test(text => text.Split([';'], 1), text => [text], values);
        Test(text => text.Split([""], StringSplitOptions.None), text => [text], values);
        Test(text => text.Split_([""], StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split(';', 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split(';', 1), text => [text], values);
        Test(text => text.Split(';', 1, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split("; ", 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split("; ", 1), text => [text], values);
        Test(text => text.Split(null as string, 10), text => [text], values);
        Test(text => text.Split("", 10), text => [text], values);
        Test(text => text.Split("; ", 1, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split(null as string, 10, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split("", 10, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split_([';'], 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split([';'], 1, StringSplitOptions.None), text => [text], values);
        Test(text => text.Split_([';'], 1, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split_(["; "], 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split(["; "], 1, StringSplitOptions.None), text => [text], values);
        Test(text => text.Split([""], 10, StringSplitOptions.None), text => [text], values);
        Test(text => text.Split_(["; "], 1, StringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split_([""], 10, StringSplitOptions.TrimEntries), text => [text.Trim()], values);

        Test(text => text.StartsWith(""), _ => true, values);
        Test((text, comparisonType) => text.StartsWith("", comparisonType), (_, _) => true, values, comparisons);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet100]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void Random()
    {
        // expression result

        TestRandom(random => random.GetHexString(0), _ => "");
        TestRandom(random => random.GetHexString(0, true), _ => "");

        TestRandom(random => random.GetItems((int[])[1, 2, 3], 0), _ => []);
        TestRandom(random => random.GetItems([1, 2, 3], 0), _ => []);

        TestRandom(random => random.Next(0), _ => 0);
        TestRandom(random => random.Next(1), _ => 0);
        TestRandom(random => random.Next(10, 10), _ => 10);
        TestRandom(random => random.Next(10, 11), _ => 10);

        TestRandom(random => random.NextInt64(0), _ => 0);
        TestRandom(random => random.NextInt64(1), _ => 0);
        TestRandom(random => random.NextInt64(10, 10), _ => 10);
        TestRandom(random => random.NextInt64(10, 11), _ => 10);

        DoNamedTest();
    }
}