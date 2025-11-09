using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ExpressionResult;
using ReCommendedExtension.Extensions.NumberInfos;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.ExpressionResult;

[TestFixture]
public sealed class ExpressionResultAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\ExpressionResult";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion || highlighting.IsError();

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
    public void TestBoolean()
    {
        var values = new[] { true, false };

        // expression result

        Test(flag => flag.Equals(false), flag => !flag, values);
        Test(flag => true.Equals(flag), flag => flag, values);
        Test(flag => false.Equals(flag), flag => !flag, values);
        Test(flag => flag.Equals(null), _ => false, values);

        Test(flag => flag.GetTypeCode(), _ => TypeCode.Boolean, values);

        DoNamedTest2();
    }


    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestByte()
    {
        var values = new byte[] { 0, 1, 2, byte.MaxValue };

        // expression result

        Test(right => MissingByteMethods.DivRem(0, right), _ => (0, 0), [..values.Except(new byte[1])]);

        Test(n => MissingByteMethods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingByteMethods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingByteMethods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingByteMethods.Clamp(n, 0, byte.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Byte, values);

        Test(n => MissingByteMethods.Max(n, n), n => n, values);
        Test(n => MissingByteMethods.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestSByte()
    {
        var values = new sbyte[] { 0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue };

        // expression result

        Test(right => MissingSByteMethods.DivRem(0, right), _ => (0, 0), [..values.Except(new sbyte[1])]);

        Test(n => MissingSByteMethods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingSByteMethods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingSByteMethods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingSByteMethods.Clamp(n, sbyte.MinValue, sbyte.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.SByte, values);

        Test(n => MissingSByteMethods.Max(n, n), n => n, values);
        Test(n => MissingSByteMethods.Min(n, n), n => n, values);

        Test(n => MissingSByteMethods.MaxMagnitude(n, n), n => n, values);
        Test(n => MissingSByteMethods.MinMagnitude(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestInt16()
    {
        var values = new short[] { 0, 1, 2, -1, -2, short.MaxValue, short.MinValue };

        // expression result

        Test(right => MissingInt16Methods.DivRem(0, right), _ => (0, 0), [..values.Except(new short[1])]);

        Test(n => MissingInt16Methods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingInt16Methods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingInt16Methods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingInt16Methods.Clamp(n, short.MinValue, short.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Int16, values);

        Test(n => MissingInt16Methods.Max(n, n), n => n, values);
        Test(n => MissingInt16Methods.Min(n, n), n => n, values);

        Test(n => MissingInt16Methods.MaxMagnitude(n, n), n => n, values);
        Test(n => MissingInt16Methods.MinMagnitude(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestUInt16()
    {
        var values = new ushort[] { 0, 1, 2, ushort.MaxValue };

        // expression result

        Test(right => MissingUInt16Methods.DivRem(0, right), _ => (0, 0), [..values.Except(new ushort[1])]);

        Test(n => MissingUInt16Methods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingUInt16Methods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingUInt16Methods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingUInt16Methods.Clamp(n, 0, ushort.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.UInt16, values);

        Test(n => MissingUInt16Methods.Max(n, n), n => n, values);
        Test(n => MissingUInt16Methods.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestInt32()
    {
        var values = new[] { 0, 1, 2, -1, -2, int.MaxValue, int.MinValue };

        // expression result

        Test(right => MissingInt32Methods.DivRem(0, right), _ => (0, 0), [..values.Except(new int[1])]);

        Test(n => MissingInt32Methods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingInt32Methods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingInt32Methods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingInt32Methods.Clamp(n, int.MinValue, int.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Int32, values);

        Test(n => MissingInt32Methods.Max(n, n), n => n, values);
        Test(n => MissingInt32Methods.Min(n, n), n => n, values);

        Test(n => MissingInt32Methods.MaxMagnitude(n, n), n => n, values);
        Test(n => MissingInt32Methods.MinMagnitude(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestUInt32()
    {
        var values = new uint[] { 0, 1, 2, uint.MaxValue };

        // expression result

        Test(right => MissingUInt32Methods.DivRem(0, right), _ => (0u, 0u), [..values.Except(new uint[1])]);

        Test(n => MissingUInt32Methods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingUInt32Methods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingUInt32Methods.Clamp(n, 1, 1), _ => 1u, values);
        Test(n => MissingUInt32Methods.Clamp(n, 0, uint.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.UInt32, values);

        Test(n => MissingUInt32Methods.Max(n, n), n => n, values);
        Test(n => MissingUInt32Methods.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestInt64()
    {
        var values = new[] { 0, 1, 2, -1, -2, long.MaxValue, long.MinValue };

        // expression result

        Test(right => MissingInt64Methods.DivRem(0, right), _ => (0, 0), [..values.Except(new long[1])]);

        Test(n => MissingInt64Methods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingInt64Methods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingInt64Methods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingInt64Methods.Clamp(n, long.MinValue, long.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Int64, values);

        Test(n => MissingInt64Methods.Max(n, n), n => n, values);
        Test(n => MissingInt64Methods.Min(n, n), n => n, values);

        Test(n => MissingInt64Methods.MaxMagnitude(n, n), n => n, values);
        Test(n => MissingInt64Methods.MinMagnitude(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestUInt64()
    {
        var values = new ulong[] { 0, 1, 2, ulong.MaxValue };

        // expression result

        Test(right => MissingUInt64Methods.DivRem(0, right), _ => (0ul, 0ul), [..values.Except(new ulong[1])]);

        Test(n => MissingUInt64Methods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingUInt64Methods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingUInt64Methods.Clamp(n, 1, 1), _ => 1ul, values);
        Test(n => MissingUInt64Methods.Clamp(n, 0, ulong.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.UInt64, values);

        Test(n => MissingUInt64Methods.Max(n, n), n => n, values);
        Test(n => MissingUInt64Methods.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestInt128()
    {
        var values = new[] { 0, 1, 2, -1, -2, Int128.MaxValue, Int128.MinValue };

        // expression result

        Test(right => Int128.DivRem(0, right), _ => (0, 0), [..values.Except(new Int128[1])]);

        Test(n => Int128.RotateLeft(n, 0), n => n, values);
        Test(n => Int128.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => Int128.Clamp(n, 1, 1), _ => 1, values);
        Test(n => Int128.Clamp(n, Int128.MinValue, Int128.MaxValue), n => n, values);

        Test(n => Int128.Max(n, n), n => n, values);
        Test(n => Int128.Min(n, n), n => n, values);

        Test(n => Int128.MaxMagnitude(n, n), n => n, values);
        Test(n => Int128.MinMagnitude(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestUInt128()
    {
        var values = new[] { 0, 1, 2, UInt128.MaxValue };

        // expression result

        Test(right => UInt128.DivRem(0, right), _ => (0, 0), [..values.Except(new UInt128[1])]);

        Test(n => UInt128.RotateLeft(n, 0), n => n, values);
        Test(n => UInt128.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => UInt128.Clamp(n, 1, 1), _ => 1, values);
        Test(n => UInt128.Clamp(n, 0, UInt128.MaxValue), n => n, values);

        Test(n => UInt128.Max(n, n), n => n, values);
        Test(n => UInt128.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestIntPtr()
    {
        var values = new[] { (nint)0, 1, 2, -1, -2 };

        // expression result

        Test(right => MissingIntPtrMethods.DivRem(0, right), _ => (0, 0), [..values.Except(new nint[1])]);

        Test(n => MissingIntPtrMethods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingIntPtrMethods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingIntPtrMethods.Clamp(n, 1, 1), _ => 1, values);

        Test(n => MissingIntPtrMethods.Max(n, n), n => n, values);
        Test(n => MissingIntPtrMethods.Min(n, n), n => n, values);

        Test(n => MissingIntPtrMethods.MaxMagnitude(n, n), n => n, values);
        Test(n => MissingIntPtrMethods.MinMagnitude(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestUIntPtr()
    {
        var values = new nuint[] { 0, 1, 2 };

        // expression result

        Test(right => MissingUIntPtrMethods.DivRem(0, right), _ => (0u, 0u), [..values.Except(new nuint[1])]);

        Test(n => MissingUIntPtrMethods.RotateLeft(n, 0), n => n, values);
        Test(n => MissingUIntPtrMethods.RotateRight(n, 0), n => n, values);

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingUIntPtrMethods.Clamp(n, 1, 1), _ => 1u, values);

        Test(n => MissingUIntPtrMethods.Max(n, n), n => n, values);
        Test(n => MissingUIntPtrMethods.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestDecimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        Test(n => MissingDecimalMethods.Clamp(n, 1, 1), _ => 1, values);
        Test(n => MissingDecimalMethods.Clamp(n, decimal.MinValue, decimal.MaxValue), n => n, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Decimal, values);

        Test(n => MissingDecimalMethods.Max(n, n), n => n, values);
        Test(n => MissingDecimalMethods.Min(n, n), n => n, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
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

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Double, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
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

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        Test(n => n.GetTypeCode(), _ => TypeCode.Single, values);

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

        // expression result

        Test(n => n.Equals(null), _ => false, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet60]
    public void TestMath()
    {
        var byteValues = new byte[] { 0, 1, 2, byte.MaxValue };
        var sbyteValues = new sbyte[] { 0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue };
        var int16Values = new short[] { 0, 1, 2, -1, -2, short.MaxValue, short.MinValue };
        var uint16Values = new ushort[] { 0, 1, 2, ushort.MaxValue };
        var int32Values = new[] { 0, 1, 2, -1, -2, int.MaxValue, int.MinValue };
        var uint32Values = new uint[] { 0, 1, 2, uint.MaxValue };
        var int64Values = new[] { 0, 1, 2, -1, -2, long.MaxValue, long.MinValue };
        var uint64Values = new ulong[] { 0, 1, 2, ulong.MaxValue };
        var intPtrValues = new[] { (nint)0, 1, 2, -1, -2 };
        var uintPtrValues = new nuint[] { 0, 1, 2 };
        var decimalValues = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };

        // expression result

        Test(right => MissingMathMethods.DivRem((byte)0, right), _ => (0, 0), [..byteValues.Except(new byte[1])]);
        Test(right => MissingMathMethods.DivRem((sbyte)0, right), _ => (0, 0), [..sbyteValues.Except(new sbyte[1])]);
        Test(right => MissingMathMethods.DivRem((short)0, right), _ => (0, 0), [..int16Values.Except(new short[1])]);
        Test(right => MissingMathMethods.DivRem((ushort)0, right), _ => (0, 0), [..uint16Values.Except(new ushort[1])]);
        Test(right => MissingMathMethods.DivRem(0, right), _ => (0, 0), [..int32Values.Except(new int[1])]);
        Test(right => MissingMathMethods.DivRem(0, right), _ => (0u, 0u), [..uint32Values.Except(new uint[1])]);
        Test(right => MissingMathMethods.DivRem(0, right), _ => (0, 0), [..int64Values.Except(new long[1])]);
        Test(right => MissingMathMethods.DivRem(0, right), _ => (0ul, 0ul), [..uint64Values.Except(new ulong[1])]);
        Test(right => MissingMathMethods.DivRem(0, right), _ => (0, 0), [..intPtrValues.Except(new nint[1])]);
        Test(right => MissingMathMethods.DivRem(0, right), _ => (0u, 0u), [..uintPtrValues.Except(new nuint[1])]);

        Test(value => MissingMathMethods.Clamp(value, (byte)1, (byte)1), _ => 1, byteValues);
        Test(value => MissingMathMethods.Clamp(value, (sbyte)1, (sbyte)1), _ => 1, sbyteValues);
        Test(value => MissingMathMethods.Clamp(value, (short)1, (short)1), _ => 1, int16Values);
        Test(value => MissingMathMethods.Clamp(value, (ushort)1, (ushort)1), _ => 1, uint16Values);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1, int32Values);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1u, uint32Values);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1, int64Values);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1ul, uint64Values);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1, intPtrValues);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1u, uintPtrValues);
        Test(value => MissingMathMethods.Clamp(value, 1, 1), _ => 1, decimalValues);

        Test(value => MissingMathMethods.Clamp(value, byte.MinValue, byte.MaxValue), value => value, byteValues);
        Test(value => MissingMathMethods.Clamp(value, sbyte.MinValue, sbyte.MaxValue), value => value, sbyteValues);
        Test(value => MissingMathMethods.Clamp(value, short.MinValue, short.MaxValue), value => value, int16Values);
        Test(value => MissingMathMethods.Clamp(value, ushort.MinValue, ushort.MaxValue), value => value, uint16Values);
        Test(value => MissingMathMethods.Clamp(value, int.MinValue, int.MaxValue), value => value, int32Values);
        Test(value => MissingMathMethods.Clamp(value, uint.MinValue, uint.MaxValue), value => value, uint32Values);
        Test(value => MissingMathMethods.Clamp(value, long.MinValue, long.MaxValue), value => value, int64Values);
        Test(value => MissingMathMethods.Clamp(value, ulong.MinValue, ulong.MaxValue), value => value, uint64Values);
        Test(value => MissingMathMethods.Clamp(value, decimal.MinValue, decimal.MaxValue), value => value, decimalValues);

        Test(n => Math.Max(n, n), n => n, byteValues);
        Test(n => Math.Max(n, n), n => n, sbyteValues);
        Test(n => Math.Max(n, n), n => n, int16Values);
        Test(n => Math.Max(n, n), n => n, uint16Values);
        Test(n => Math.Max(n, n), n => n, int32Values);
        Test(n => Math.Max(n, n), n => n, uint32Values);
        Test(n => Math.Max(n, n), n => n, int64Values);
        Test(n => Math.Max(n, n), n => n, uint64Values);
        Test(n => MissingMathMethods.Max(n, n), n => n, intPtrValues);
        Test(n => MissingMathMethods.Max(n, n), n => n, uintPtrValues);
        Test(n => Math.Max(n, n), n => n, decimalValues);

        Test(n => Math.Min(n, n), n => n, byteValues);
        Test(n => Math.Min(n, n), n => n, sbyteValues);
        Test(n => Math.Min(n, n), n => n, int16Values);
        Test(n => Math.Min(n, n), n => n, uint16Values);
        Test(n => Math.Min(n, n), n => n, int32Values);
        Test(n => Math.Min(n, n), n => n, uint32Values);
        Test(n => Math.Min(n, n), n => n, int64Values);
        Test(n => Math.Min(n, n), n => n, uint64Values);
        Test(n => MissingMathMethods.Min(n, n), n => n, intPtrValues);
        Test(n => MissingMathMethods.Min(n, n), n => n, uintPtrValues);
        Test(n => Math.Min(n, n), n => n, decimalValues);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
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

        // expression result

        Test(() => new DateTime(0), () => DateTime.MinValue);

        Test(dateTime => dateTime.Equals(null), _ => false, values);

        Test(dateTime => dateTime.GetTypeCode(), _ => TypeCode.DateTime, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
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

        // expression result

        Test(dateTimeOffset => dateTimeOffset.Equals(null), _ => false, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet90]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
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

        // expression result

        Test(() => new TimeSpan(0), () => TimeSpan.Zero);
        Test(() => new TimeSpan(long.MinValue), () => TimeSpan.MinValue);
        Test(() => new TimeSpan(long.MaxValue), () => TimeSpan.MaxValue);
        Test(() => new TimeSpan(0, 0, 0), () => TimeSpan.Zero);
        Test(() => new TimeSpan(0, 0, 0, 0), () => TimeSpan.Zero);
        Test(() => new TimeSpan(0, 0, 0, 0, 0), () => TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers._Ctor(0, 0, 0, 0, 0, 0), () => TimeSpan.Zero);

        Test(timeSpan => timeSpan.Equals(null), _ => false, values);

        Test(() => MissingTimeSpanMembers.FromDays(0), () => TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers.FromDays(0, 0), () => TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromHours(0), () => TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers.FromHours(0, 0), () => TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromMicroseconds(0), () => TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromMilliseconds(0), () => TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromMinutes(0), () => TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers.FromMinutes(0, 0), () => TimeSpan.Zero);

        Test(() => MissingTimeSpanMembers.FromSeconds(0), () => TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers.FromSeconds(0, 0), () => TimeSpan.Zero);

        Test(() => TimeSpan.FromTicks(0), () => TimeSpan.Zero);
        Test(() => TimeSpan.FromTicks(long.MinValue), () => TimeSpan.MinValue);
        Test(() => TimeSpan.FromTicks(long.MaxValue), () => TimeSpan.MaxValue);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestDateOnly()
    {
        var values = new[] { DateOnly.MinValue, DateOnly.MaxValue, new(2025, 7, 15) };

        // expression result

        Test(dateOnly => dateOnly.Equals(null), _ => false, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTimeOnly()
    {
        var values = new[] { TimeOnly.MinValue, TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5) };

        // expression result

        Test(() => new TimeOnly(0), () => TimeOnly.MinValue);
        Test(() => new TimeOnly(0, 0), () => TimeOnly.MinValue);
        Test(() => new TimeOnly(0, 0, 0), () => TimeOnly.MinValue);
        Test(() => new TimeOnly(0, 0, 0, 0), () => TimeOnly.MinValue);
        Test(() => new TimeOnly(0, 0, 0, 0, 0), () => TimeOnly.MinValue);

        Test(timeOnly => timeOnly.Equals(null), _ => false, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGuid()
    {
        var values = new[] { Guid.Empty, new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]) };

        // expression result

        Test(guid => guid.Equals(null), _ => false, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestChar()
    {
        var values = new[] { 'a', 'A', '1', ' ', 'ä', 'ß', '€', char.MinValue, char.MaxValue };

        // expression result

        Test(c => c.Equals(null), _ => false, values);

        Test(c => c.GetTypeCode(), _ => TypeCode.Char, values);

        DoNamedTest2();
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
    public void TestString()
    {
        var values = new[] { "", "abcde", "  abcde  ", "ab;cd;e", "ab;cd:e", "..abcde.." };
        var comparisons = new[]
        {
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase,
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
        };
        var stringSplitOptions = new[]
        {
            MissingStringSplitOptions.None, MissingStringSplitOptions.RemoveEmptyEntries, MissingStringSplitOptions.TrimEntries,
        };

        // expression result

        Test(text => text.Contains(""), _ => true, values);
        Test((text, comparisonType) => text.Contains("", comparisonType), (_, _) => true, values, comparisons);

        Test(text => text.EndsWith(""), _ => true, values);
        Test((text, comparisonType) => text.EndsWith("", comparisonType), (_, _) => true, values, comparisons);

        Test(text => text.GetTypeCode(), _ => TypeCode.String, values);

        Test(text => text.IndexOf(""), _ => 0, values);
        Test((text, comparisonType) => text.IndexOf("", comparisonType), (_, _) => 0, values, comparisons);

        Test(text => text.IndexOfAny([]), _ => -1, values);

        Test(() => MissingStringMethods.Join(';', (IEnumerable<int>)[]), () => "");
        Test(() => MissingStringMethods.Join(';', (IEnumerable<int>)[100]), () => $"{100}");
        Test(() => MissingStringMethods.Join(';', (string?[])[]), () => "");
        Test(() => MissingStringMethods.Join(';', (string?[])["item"]), () => "item");
        Test(() => MissingStringMethods.Join(';', (object?[])[]), () => "");
        Test(() => MissingStringMethods.Join(';', (object?[])[true]), () => $"{true}");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<string?>)[]), () => "");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<string?>)["item"]), () => "item");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<object?>)[]), () => "");
        Test(() => MissingStringMethods.Join(',', (ReadOnlySpan<object?>)[true]), () => $"{true}");
        Test(() => string.Join("; ", (IEnumerable<int>)[]), () => "");
        Test(() => string.Join("; ", (IEnumerable<int>)[100]), () => $"{100}");
        Test(() => string.Join("; ", (string?[])[]), () => "");
        Test(() => string.Join("; ", (string?[])["item"]), () => "item");
        Test(() => string.Join("; ", (object?[])[]), () => "");
        Test(() => string.Join("; ", (object?[])[true]), () => $"{true}");
        Test(() => MissingStringMethods.Join("; ", (ReadOnlySpan<string?>)[]), () => "");
        Test(() => MissingStringMethods.Join("; ", (ReadOnlySpan<string?>)["item"]), () => "item");
        Test(() => MissingStringMethods.Join("; ", (ReadOnlySpan<object?>)[]), () => "");
        Test(() => MissingStringMethods.Join("; ", (ReadOnlySpan<object?>)[true]), () => $"{true}");
        Test(() => MissingStringMethods.Join(';', (string?[])["item1", "item2"], 0, 0), () => "");
        Test(() => MissingStringMethods.Join(';', (string?[])["item"], 1, 0), () => "");
        Test(() => MissingStringMethods.Join(';', (string?[])["item"], 0, 1), () => "item");
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
        Test(text => text.Split(null as string, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split("", MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split([';'], 0), _ => [], values);
        Test(text => text.Split([';'], 1), text => [text], values);
        Test(text => text.Split([""], MissingStringSplitOptions.None), text => [text], values);
        Test(text => text.Split([""], MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split(';', 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split(';', 1), text => [text], values);
        Test(text => text.Split(';', 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split("; ", 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split("; ", 1), text => [text], values);
        Test(text => text.Split(null as string, 10), text => [text], values);
        Test(text => text.Split("", 10), text => [text], values);
        Test(text => text.Split("; ", 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split(null as string, 10, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split("", 10, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split([';'], 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split([';'], 1, MissingStringSplitOptions.None), text => [text], values);
        Test(text => text.Split([';'], 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test((text, options) => text.Split(["; "], 0, options), (_, _) => [], values, stringSplitOptions);
        Test(text => text.Split(["; "], 1, MissingStringSplitOptions.None), text => [text], values);
        Test(text => text.Split([""], 10, MissingStringSplitOptions.None), text => [text], values);
        Test(text => text.Split(["; "], 1, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);
        Test(text => text.Split([""], 10, MissingStringSplitOptions.TrimEntries), text => [text.Trim()], values);

        Test(text => text.StartsWith(""), _ => true, values);
        Test((text, comparisonType) => text.StartsWith("", comparisonType), (_, _) => true, values, comparisons);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestRandom()
    {
        // expression result

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

        DoNamedTest2();
    }
}