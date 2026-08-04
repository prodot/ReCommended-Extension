using System.Globalization;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;
using ReCommendedExtension.Tests.Missing;

using int128 = ReCommendedExtension.Extensions.NumberInfos.Int128;
using uint128 = ReCommendedExtension.Extensions.NumberInfos.UInt128;
using half = ReCommendedExtension.Extensions.NumberInfos.Half;
using dateOnly = ReCommendedExtension.Tests.Missing.DateOnly;
using timeOnly = ReCommendedExtension.Tests.Missing.TimeOnly;
using mathF = ReCommendedExtension.Tests.Missing.MathF;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class ArgumentAnalyzerTests : CSharpAnalyzerTests
{
    readonly IFormatProvider?[] formatProviders = [null, CultureInfo.InvariantCulture, new CultureInfo("en-US"), new CultureInfo("de-DE")];

    readonly Calendar[] calendars = [new GregorianCalendar(), new JulianCalendar(), new JapaneseCalendar()];

    readonly NumberStyles[] unsignedIntegerStyles = [NumberStyles.None, NumberStyles.Integer];

    readonly NumberStyles[] signedIntegerStyles = [NumberStyles.AllowLeadingSign, NumberStyles.Integer];

    readonly NumberStyles[] floatingPointStyles =
    [
        NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
        NumberStyles.Float | NumberStyles.AllowThousands,
    ];

    readonly DateTimeStyles[] dateTimeStyles =
    [
        DateTimeStyles.None,
        DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces,
        DateTimeStyles.AssumeLocal,
        DateTimeStyles.AssumeUniversal,
        DateTimeStyles.AdjustToUniversal,
    ];

    readonly MidpointRounding[] roundings = [MidpointRounding.ToEven, MidpointRounding.AwayFromZero];

    readonly int[] digitsValues = [0, 1, 2];

    protected override string RelativeTestDataPath => @"Analyzers\Argument";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantArgumentHint
            or RedundantArgumentRangeHint
            or RedundantElementHint
            or UseOtherArgumentSuggestion
            or UseOtherArgumentRangeSuggestion;

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void TestRandom<R>(Func<Random, R> expected, Func<Random, R> actual) => Test(() => expected(new Random(1)), () => actual(new Random(1)));

    static void Test<T, R>(Func<T, R> expected, Func<T, R> actual, T[] args)
    {
        foreach (var a in args)
        {
            Assert.AreEqual(expected(a), actual(a), $"with value: {a}");
        }
    }

    static void TestStringBuilder(Func<StringBuilder, StringBuilder> expected, Func<StringBuilder, StringBuilder> actual, string[] args)
        => Test(value => expected(new StringBuilder(value)).ToString(), value => actual(new StringBuilder(value)).ToString(), args);

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test<T, O, R>(FuncWithOut<T, O, R> expected, FuncWithOut<T, O, R> actual, T[] args)
    {
        foreach (var a in args)
        {
            Assert.AreEqual(expected(a, out var expectedResult), actual(a, out var actualResult), $"with value: {a}");
            Assert.AreEqual(expectedResult, actualResult, $"with value: {a}");
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

    delegate R FuncWithOut<in T, in U, O, out R>(T arg1, U arg2, out O arg3);

    static void Test<T, U, O, R>(FuncWithOut<T, U, O, R> expected, FuncWithOut<T, U, O, R> actual, T[] args1, U[] args2)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                Assert.AreEqual(expected(a, b, out var expectedResult), actual(a, b, out var actualResult), $"with values: {a}, {b}");
                Assert.AreEqual(expectedResult, actualResult, $"with values: {a}, {b}");
            }
        }
    }

    static void Test<T, U, V, R>(Func<T, U, V, R> expected, Func<T, U, V, R> actual, T[] args1, U[] args2, V[] args3)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                foreach (var c in args3)
                {
                    Assert.AreEqual(expected(a, b, c), actual(a, b, c), $"with values: {a}, {b}, {c}");
                }
            }
        }
    }

    delegate R FuncWithOut<in T, in U, in V, O1, out R>(T arg1, U arg2, V arg3, out O1 arg4);

    static void Test<T, U, V, O, R>(FuncWithOut<T, U, V, O, R> expected, FuncWithOut<T, U, V, O, R> actual, T[] args1, U[] args2, V[] args3)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                foreach (var c in args3)
                {
                    Assert.AreEqual(expected(a, b, c, out var expectedResult), actual(a, b, c, out var actualResult), $"with values: {a}, {b}, {c}");
                    Assert.AreEqual(expectedResult, actualResult, $"with values: {a}, {b}, {c}");
                }
            }
        }
    }

    static void Test<T, U, V, W, R>(Func<T, U, V, W, R> expected, Func<T, U, V, W, R> actual, T[] args1, U[] args2, V[] args3, W[] args4)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                foreach (var c in args3)
                {
                    foreach (var d in args4)
                    {
                        Assert.AreEqual(expected(a, b, c, d), actual(a, b, c, d), $"with values: {a}, {b}, {c}, {d}");
                    }
                }
            }
        }
    }

    delegate R FuncWithOut<in T, in U, in V, in W, O, out R>(T arg1, U arg2, V arg3, W arg4, out O arg5);

    static void Test<T, U, V, W, O, R>(
        FuncWithOut<T, U, V, W, O, R> expected,
        FuncWithOut<T, U, V, W, O, R> actual,
        T[] args1,
        U[] args2,
        V[] args3,
        W[] args4)
    {
        foreach (var a in args1)
        {
            foreach (var b in args2)
            {
                foreach (var c in args3)
                {
                    foreach (var d in args4)
                    {
                        Assert.AreEqual(
                            expected(a, b, c, d, out var expectedResult),
                            actual(a, b, c, d, out var actualResult),
                            $"with values: {a}, {b}, {c}, {d}");
                        Assert.AreEqual(expectedResult, actualResult, $"with values: {a}, {b}, {c}, {d}");
                    }
                }
            }
        }
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Byte()
    {
        var values = new byte[] { 0, 1, 2, byte.MaxValue };

        // redundant argument

        Test(n => byte.Parse($"{n}", NumberStyles.Integer), n => byte.Parse($"{n}"), values);
        Test(n => byte.Parse($"{n}", null), n => byte.Parse($"{n}"), values);
        Test(
            (n, provider) => byte.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => byte.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => byte.Parse($"{n}", style, null), (n, style) => byte.Parse($"{n}", style), values, unsignedIntegerStyles);
        Test(n => byte.Parse($"{n}".AsSpan(), null), n => byte.Parse($"{n}".AsSpan()), values);
        Test(n => byte.Parse($"{n}".AsUtf8Bytes(), null), n => byte.Parse($"{n}".AsUtf8Bytes()), values);

        Test<byte, byte, bool>(
            (n, out result) => byte.TryParse($"{n}", null, out result),
            (n, out result) => byte.TryParse($"{n}", out result),
            values);
        Test<byte, byte, bool>(
            (n, out result) => byte.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => byte.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<byte, byte, bool>(
            (n, out result) => byte.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => byte.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<byte, IFormatProvider?, byte, bool>(
            (n, provider, out result) => byte.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => byte.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<byte, IFormatProvider?, byte, bool>(
            (n, provider, out result) => byte.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => byte.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<byte, IFormatProvider?, byte, bool>(
            (n, provider, out result) => byte.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => byte.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void SByte()
    {
        var values = new sbyte[] { 0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue };

        // redundant argument

        Test(n => sbyte.Parse($"{n}", NumberStyles.Integer), n => sbyte.Parse($"{n}"), values);
        Test(n => sbyte.Parse($"{n}", null), n => sbyte.Parse($"{n}"), values);
        Test(
            (n, provider) => sbyte.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => sbyte.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => sbyte.Parse($"{n}", style, null), (n, style) => sbyte.Parse($"{n}", style), values, signedIntegerStyles);
        Test(n => sbyte.Parse($"{n}".AsSpan(), null), n => sbyte.Parse($"{n}".AsSpan()), values);
        Test(n => sbyte.Parse($"{n}".AsUtf8Bytes(), null), n => sbyte.Parse($"{n}".AsUtf8Bytes()), values);

        Test<sbyte, sbyte, bool>(
            (n, out result) => sbyte.TryParse($"{n}", null, out result),
            (n, out result) => sbyte.TryParse($"{n}", out result),
            values);
        Test<sbyte, sbyte, bool>(
            (n, out result) => sbyte.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => sbyte.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<sbyte, sbyte, bool>(
            (n, out result) => sbyte.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => sbyte.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<sbyte, IFormatProvider?, sbyte, bool>(
            (n, provider, out result) => sbyte.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => sbyte.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<sbyte, IFormatProvider?, sbyte, bool>(
            (n, provider, out result) => sbyte.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => sbyte.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<sbyte, IFormatProvider?, sbyte, bool>(
            (n, provider, out result) => sbyte.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => sbyte.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Int16()
    {
        var values = new short[] { 0, 1, 2, -1, -2, short.MaxValue, short.MinValue };

        // redundant argument

        Test(n => short.Parse($"{n}", NumberStyles.Integer), n => short.Parse($"{n}"), values);
        Test(n => short.Parse($"{n}", null), n => short.Parse($"{n}"), values);
        Test(
            (n, provider) => short.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => short.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => short.Parse($"{n}", style, null), (n, style) => short.Parse($"{n}", style), values, signedIntegerStyles);
        Test(n => short.Parse($"{n}".AsSpan(), null), n => short.Parse($"{n}".AsSpan()), values);
        Test(n => short.Parse($"{n}".AsUtf8Bytes(), null), n => short.Parse($"{n}".AsUtf8Bytes()), values);

        Test<short, short, bool>(
            (n, out result) => short.TryParse($"{n}", null, out result),
            (n, out result) => short.TryParse($"{n}", out result),
            values);
        Test<short, short, bool>(
            (n, out result) => short.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => short.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<short, short, bool>(
            (n, out result) => short.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => short.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<short, IFormatProvider?, short, bool>(
            (n, provider, out result) => short.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => short.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<short, IFormatProvider?, short, bool>(
            (n, provider, out result) => short.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => short.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<short, IFormatProvider?, short, bool>(
            (n, provider, out result) => short.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => short.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void UInt16()
    {
        var values = new ushort[] { 0, 1, 2, ushort.MaxValue };

        // redundant argument

        Test(n => ushort.Parse($"{n}", NumberStyles.Integer), n => ushort.Parse($"{n}"), values);
        Test(n => ushort.Parse($"{n}", null), n => ushort.Parse($"{n}"), values);
        Test(
            (n, provider) => ushort.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => ushort.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => ushort.Parse($"{n}", style, null), (n, style) => ushort.Parse($"{n}", style), values, unsignedIntegerStyles);
        Test(n => ushort.Parse($"{n}".AsSpan(), null), n => ushort.Parse($"{n}".AsSpan()), values);
        Test(n => ushort.Parse($"{n}".AsUtf8Bytes(), null), n => ushort.Parse($"{n}".AsUtf8Bytes()), values);

        Test<ushort, ushort, bool>(
            (n, out result) => ushort.TryParse($"{n}", null, out result),
            (n, out result) => ushort.TryParse($"{n}", out result),
            values);
        Test<ushort, ushort, bool>(
            (n, out result) => ushort.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => ushort.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<ushort, ushort, bool>(
            (n, out result) => ushort.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => ushort.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<ushort, IFormatProvider?, ushort, bool>(
            (n, provider, out result) => ushort.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => ushort.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<ushort, IFormatProvider?, ushort, bool>(
            (n, provider, out result) => ushort.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => ushort.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<ushort, IFormatProvider?, ushort, bool>(
            (n, provider, out result) => ushort.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => ushort.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Int32()
    {
        var values = new[] { 0, 1, 2, -1, -2, int.MaxValue, int.MinValue };

        // redundant argument

        Test(n => int.Parse($"{n}", NumberStyles.Integer), n => int.Parse($"{n}"), values);
        Test(n => int.Parse($"{n}", null), n => int.Parse($"{n}"), values);
        Test(
            (n, provider) => int.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => int.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => int.Parse($"{n}", style, null), (n, style) => int.Parse($"{n}", style), values, signedIntegerStyles);
        Test(n => int.Parse($"{n}".AsSpan(), null), n => int.Parse($"{n}".AsSpan()), values);
        Test(n => int.Parse($"{n}".AsUtf8Bytes(), null), n => int.Parse($"{n}".AsUtf8Bytes()), values);

        Test<int, int, bool>((n, out result) => int.TryParse($"{n}", null, out result), (n, out result) => int.TryParse($"{n}", out result), values);
        Test<int, int, bool>(
            (n, out result) => int.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => int.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<int, int, bool>(
            (n, out result) => int.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => int.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<int, IFormatProvider?, int, bool>(
            (n, provider, out result) => int.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => int.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<int, IFormatProvider?, int, bool>(
            (n, provider, out result) => int.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => int.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<int, IFormatProvider?, int, bool>(
            (n, provider, out result) => int.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => int.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void UInt32()
    {
        var values = new uint[] { 0, 1, 2, uint.MaxValue };

        // redundant argument

        Test(n => uint.Parse($"{n}", NumberStyles.Integer), n => uint.Parse($"{n}"), values);
        Test(n => uint.Parse($"{n}", null), n => uint.Parse($"{n}"), values);
        Test(
            (n, provider) => uint.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => uint.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => uint.Parse($"{n}", style, null), (n, style) => uint.Parse($"{n}", style), values, unsignedIntegerStyles);
        Test(n => uint.Parse($"{n}".AsSpan(), null), n => uint.Parse($"{n}".AsSpan()), values);
        Test(n => uint.Parse($"{n}".AsUtf8Bytes(), null), n => uint.Parse($"{n}".AsUtf8Bytes()), values);

        Test<uint, uint, bool>(
            (n, out result) => uint.TryParse($"{n}", null, out result),
            (n, out result) => uint.TryParse($"{n}", out result),
            values);
        Test<uint, uint, bool>(
            (n, out result) => uint.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => uint.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<uint, uint, bool>(
            (n, out result) => uint.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => uint.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<uint, IFormatProvider?, uint, bool>(
            (n, provider, out result) => uint.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => uint.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<uint, IFormatProvider?, uint, bool>(
            (n, provider, out result) => uint.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => uint.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<uint, IFormatProvider?, uint, bool>(
            (n, provider, out result) => uint.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => uint.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Int64()
    {
        var values = new[] { 0, 1, 2, -1, -2, long.MaxValue, long.MinValue };

        // redundant argument

        Test(n => long.Parse($"{n}", NumberStyles.Integer), n => long.Parse($"{n}"), values);
        Test(n => long.Parse($"{n}", null), n => long.Parse($"{n}"), values);
        Test(
            (n, provider) => long.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => long.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => long.Parse($"{n}", style, null), (n, style) => long.Parse($"{n}", style), values, signedIntegerStyles);
        Test(n => long.Parse($"{n}".AsSpan(), null), n => long.Parse($"{n}".AsSpan()), values);
        Test(n => long.Parse($"{n}".AsUtf8Bytes(), null), n => long.Parse($"{n}".AsUtf8Bytes()), values);

        Test<long, long, bool>(
            (n, out result) => long.TryParse($"{n}", null, out result),
            (n, out result) => long.TryParse($"{n}", out result),
            values);
        Test<long, long, bool>(
            (n, out result) => long.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => long.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<long, long, bool>(
            (n, out result) => long.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => long.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<long, IFormatProvider?, long, bool>(
            (n, provider, out result) => long.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => long.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<long, IFormatProvider?, long, bool>(
            (n, provider, out result) => long.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => long.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<long, IFormatProvider?, long, bool>(
            (n, provider, out result) => long.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => long.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void UInt64()
    {
        var values = new ulong[] { 0, 1, 2, ulong.MaxValue };

        // redundant argument

        Test(n => ulong.Parse($"{n}", NumberStyles.Integer), n => ulong.Parse($"{n}"), values);
        Test(n => ulong.Parse($"{n}", null), n => ulong.Parse($"{n}"), values);
        Test(
            (n, provider) => ulong.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => ulong.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => ulong.Parse($"{n}", style, null), (n, style) => ulong.Parse($"{n}", style), values, unsignedIntegerStyles);
        Test(n => ulong.Parse($"{n}".AsSpan(), null), n => ulong.Parse($"{n}".AsSpan()), values);
        Test(n => ulong.Parse($"{n}".AsUtf8Bytes(), null), n => ulong.Parse($"{n}".AsUtf8Bytes()), values);

        Test<ulong, ulong, bool>(
            (n, out result) => ulong.TryParse($"{n}", null, out result),
            (n, out result) => ulong.TryParse($"{n}", out result),
            values);
        Test<ulong, ulong, bool>(
            (n, out result) => ulong.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => ulong.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<ulong, ulong, bool>(
            (n, out result) => ulong.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => ulong.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<ulong, IFormatProvider?, ulong, bool>(
            (n, provider, out result) => ulong.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => ulong.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<ulong, IFormatProvider?, ulong, bool>(
            (n, provider, out result) => ulong.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => ulong.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<ulong, IFormatProvider?, ulong, bool>(
            (n, provider, out result) => ulong.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => ulong.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Int128()
    {
        var values = new[] { 0, 1, 2, -1, -2, int128.MaxValue, int128.MinValue };

        // redundant argument

        Test(n => int128.Parse($"{n}", NumberStyles.Integer), n => int128.Parse($"{n}"), values);
        Test(n => int128.Parse($"{n}", null), n => int128.Parse($"{n}"), values);
        Test(
            (n, provider) => int128.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => int128.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => int128.Parse($"{n}", style, null), (n, style) => int128.Parse($"{n}", style), values, signedIntegerStyles);
        Test(n => int128.Parse($"{n}".AsSpan(), null), n => int128.Parse($"{n}".AsSpan()), values);
        Test(n => int128.Parse($"{n}".AsUtf8Bytes(), null), n => int128.Parse($"{n}".AsUtf8Bytes()), values);

        Test<int128, int128, bool>(
            (n, out result) => int128.TryParse($"{n}", null, out result),
            (n, out result) => int128.TryParse($"{n}", out result),
            values);
        Test<int128, int128, bool>(
            (n, out result) => int128.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => int128.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<int128, int128, bool>(
            (n, out result) => int128.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => int128.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<int128, IFormatProvider?, int128, bool>(
            (n, provider, out result) => int128.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => int128.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<int128, IFormatProvider?, int128, bool>(
            (n, provider, out result) => int128.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => int128.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<int128, IFormatProvider?, int128, bool>(
            (n, provider, out result) => int128.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => int128.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void UInt128()
    {
        var values = new[] { 0, 1, 2, uint128.MaxValue };

        // redundant argument

        Test(n => uint128.Parse($"{n}", NumberStyles.Integer), n => uint128.Parse($"{n}"), values);
        Test(n => uint128.Parse($"{n}", null), n => uint128.Parse($"{n}"), values);
        Test(
            (n, provider) => uint128.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => uint128.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => uint128.Parse($"{n}", style, null), (n, style) => uint128.Parse($"{n}", style), values, unsignedIntegerStyles);
        Test(n => uint128.Parse($"{n}".AsSpan(), null), n => uint128.Parse($"{n}".AsSpan()), values);
        Test(n => uint128.Parse($"{n}".AsUtf8Bytes(), null), n => uint128.Parse($"{n}".AsUtf8Bytes()), values);

        Test<uint128, uint128, bool>(
            (n, out result) => uint128.TryParse($"{n}", null, out result),
            (n, out result) => uint128.TryParse($"{n}", out result),
            values);
        Test<uint128, uint128, bool>(
            (n, out result) => uint128.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => uint128.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<uint128, uint128, bool>(
            (n, out result) => uint128.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => uint128.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<uint128, IFormatProvider?, uint128, bool>(
            (n, provider, out result) => uint128.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => uint128.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<uint128, IFormatProvider?, uint128, bool>(
            (n, provider, out result) => uint128.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => uint128.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<uint128, IFormatProvider?, uint128, bool>(
            (n, provider, out result) => uint128.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => uint128.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void IntPtr()
    {
        var values = new[] { (nint)0, 1, 2, -1, -2 };

        // redundant argument

        Test(n => nint.Parse($"{n}", NumberStyles.Integer), n => nint.Parse($"{n}"), values);
        Test(n => nint.Parse($"{n}", null), n => nint.Parse($"{n}"), values);
        Test(
            (n, provider) => nint.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => nint.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => nint.Parse($"{n}", style, null), (n, style) => nint.Parse($"{n}", style), values, signedIntegerStyles);
        Test(n => nint.Parse($"{n}".AsSpan(), null), n => nint.Parse($"{n}".AsSpan()), values);
        Test(n => nint.Parse($"{n}".AsUtf8Bytes(), null), n => nint.Parse($"{n}".AsUtf8Bytes()), values);

        Test<nint, nint, bool>(
            (n, out result) => nint.TryParse($"{n}", null, out result),
            (n, out result) => nint.TryParse($"{n}", out result),
            values);
        Test<nint, nint, bool>(
            (n, out result) => nint.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => nint.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<nint, nint, bool>(
            (n, out result) => nint.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => nint.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<nint, IFormatProvider?, nint, bool>(
            (n, provider, out result) => nint.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => nint.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<nint, IFormatProvider?, nint, bool>(
            (n, provider, out result) => nint.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => nint.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<nint, IFormatProvider?, nint, bool>(
            (n, provider, out result) => nint.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => nint.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void UIntPtr()
    {
        var values = new nuint[] { 0, 1, 2 };

        // redundant argument

        Test(n => nuint.Parse($"{n}", NumberStyles.Integer), n => nuint.Parse($"{n}"), values);
        Test(n => nuint.Parse($"{n}", null), n => nuint.Parse($"{n}"), values);
        Test(
            (n, provider) => nuint.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => nuint.Parse($"{n}", provider),
            values,
            formatProviders);
        Test(
            (n, style) => nuint.Parse($"{n}", style, null),
            (n, style) => nuint.Parse($"{n}", style),
            values,
            unsignedIntegerStyles);
        Test(n => nuint.Parse($"{n}".AsSpan(), null), n => nuint.Parse($"{n}".AsSpan()), values);
        Test(n => nuint.Parse($"{n}".AsUtf8Bytes(), null), n => nuint.Parse($"{n}".AsUtf8Bytes()), values);

        Test<nuint, nuint, bool>(
            (n, out result) => nuint.TryParse($"{n}", null, out result),
            (n, out result) => nuint.TryParse($"{n}", out result),
            values);
        Test<nuint, nuint, bool>(
            (n, out result) => nuint.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => nuint.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<nuint, nuint, bool>(
            (n, out result) => nuint.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => nuint.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<nuint, IFormatProvider?, nuint, bool>(
            (n, provider, out result) => nuint.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (n, provider, out result) => nuint.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test<nuint, IFormatProvider?, nuint, bool>(
            (n, provider, out result) => nuint.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => nuint.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<nuint, IFormatProvider?, nuint, bool>(
            (n, provider, out result) => nuint.TryParse($"{n}".AsUtf8Bytes(), NumberStyles.Integer, provider, out result),
            (n, provider, out result) => nuint.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Decimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberStyles.Number };

        // redundant argument

        Test(n => decimal.Parse($"{n}", NumberStyles.Number), n => decimal.Parse($"{n}"), values);
        Test(n => decimal.Parse($"{n}", null), n => decimal.Parse($"{n}"), values);
        Test(
            (n, provider) => decimal.Parse(n.ToString(provider), NumberStyles.Number, provider),
            (n, provider) => decimal.Parse(n.ToString(provider), provider),
            values,
            formatProviders);
        Test((n, style) => decimal.Parse($"{n}", style, null), (n, style) => decimal.Parse($"{n}", style), values, styles);
        Test(n => decimal.Parse($"{n}".AsSpan(), null), n => decimal.Parse($"{n}".AsSpan()), values);
        Test(n => decimal.Parse($"{n}".AsUtf8Bytes(), null), n => decimal.Parse($"{n}".AsUtf8Bytes()), values);

        Test<decimal, decimal, bool>(
            (n, out result) => decimal.TryParse($"{n}", null, out result),
            (n, out result) => decimal.TryParse($"{n}", out result),
            values);
        Test<decimal, decimal, bool>(
            (n, out result) => decimal.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => decimal.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<decimal, decimal, bool>(
            (n, out result) => decimal.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => decimal.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<decimal, IFormatProvider?, decimal, bool>(
            (n, provider, out result) => decimal.TryParse(n.ToString(provider), NumberStyles.Number, provider, out result),
            (n, provider, out result) => decimal.TryParse(n.ToString(provider), provider, out result),
            values,
            formatProviders);
        Test<decimal, IFormatProvider?, decimal, bool>(
            (n, provider, out result) => decimal.TryParse(n.ToString(provider).AsSpan(), NumberStyles.Number, provider, out result),
            (n, provider, out result) => decimal.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<decimal, IFormatProvider?, decimal, bool>(
            (n, provider, out result) => decimal.TryParse(n.ToString(provider).AsUtf8Bytes(), NumberStyles.Number, provider, out result),
            (n, provider, out result) => decimal.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        Test(n => decimal.Round(n, 0), n => decimal.Round(n), values);
        Test(n => decimal.Round(n, MidpointRounding.ToEven), n => decimal.Round(n), values);
        Test((n, mode) => decimal.Round(n, 0, mode), (n, mode) => decimal.Round(n, mode), values, roundings);
        Test((n, decimals) => decimal.Round(n, decimals, MidpointRounding.ToEven), (n, decimals) => decimal.Round(n, decimals), values, digitsValues);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Double()
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
        double[] valuesForParsing = [..values.Except([double.MinValue, double.MaxValue]), float.MinValue, float.MaxValue];

        // redundant argument

        Test(n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => double.Parse($"{n}"), valuesForParsing);
        Test(n => double.Parse($"{n}", null), n => double.Parse($"{n}"), valuesForParsing);
        Test(
            (n, provider) => double.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => double.Parse(n.ToString(provider), provider),
            valuesForParsing,
            formatProviders);
        Test((n, style) => double.Parse($"{n}", style, null), (n, style) => double.Parse($"{n}", style), valuesForParsing, floatingPointStyles);
        Test(n => double.Parse($"{n}".AsSpan(), null), n => double.Parse($"{n}".AsSpan()), valuesForParsing);
        Test(n => double.Parse($"{n}".AsUtf8Bytes(), null), n => double.Parse($"{n}".AsUtf8Bytes()), valuesForParsing);

        Test<double, double, bool>(
            (n, out result) => double.TryParse($"{n}", null, out result),
            (n, out result) => double.TryParse($"{n}", out result),
            valuesForParsing);
        Test<double, double, bool>(
            (n, out result) => double.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => double.TryParse($"{n}".AsSpan(), out result),
            valuesForParsing);
        Test<double, double, bool>(
            (n, out result) => double.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => double.TryParse($"{n}".AsUtf8Bytes(), out result),
            valuesForParsing);
        Test<double, IFormatProvider?, double, bool>(
            (n, provider, out result) => double.TryParse(
                n.ToString(provider),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => double.TryParse(n.ToString(provider), provider, out result),
            valuesForParsing,
            formatProviders);
        Test<double, IFormatProvider?, double, bool>(
            (n, provider, out result) => double.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => double.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            valuesForParsing,
            formatProviders);
        Test<double, IFormatProvider?, double, bool>(
            (n, provider, out result) => double.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => double.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            valuesForParsing,
            formatProviders);

        Test(n => double.Round(n, 0), n => double.Round(n), values);
        Test(n => double.Round(n, MidpointRounding.ToEven), n => double.Round(n), values);
        Test((n, mode) => double.Round(n, 0, mode), (n, mode) => double.Round(n, mode), values, roundings);
        Test((n, digits) => double.Round(n, digits, MidpointRounding.ToEven), (n, digits) => double.Round(n, digits), values, digitsValues);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Single()
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
        float[] valuesForParsing = [.. values.Except([float.MinValue, float.MaxValue])];

        // redundant argument

        Test(n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => float.Parse($"{n}"), valuesForParsing);
        Test(n => float.Parse($"{n}", null), n => float.Parse($"{n}"), valuesForParsing);
        Test(
            (n, provider) => float.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => float.Parse(n.ToString(provider), provider),
            valuesForParsing,
            formatProviders);
        Test((n, style) => float.Parse($"{n}", style, null), (n, style) => float.Parse($"{n}", style), valuesForParsing, floatingPointStyles);
        Test(n => float.Parse($"{n}".AsSpan(), null), n => float.Parse($"{n}".AsSpan()), valuesForParsing);
        Test(n => float.Parse($"{n}".AsUtf8Bytes(), null), n => float.Parse($"{n}".AsUtf8Bytes()), valuesForParsing);

        Test<float, float, bool>(
            (n, out result) => float.TryParse($"{n}", null, out result),
            (n, out result) => float.TryParse($"{n}", out result),
            valuesForParsing);
        Test<float, float, bool>(
            (n, out result) => float.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => float.TryParse($"{n}".AsSpan(), out result),
            valuesForParsing);
        Test<float, float, bool>(
            (n, out result) => float.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => float.TryParse($"{n}".AsUtf8Bytes(), out result),
            valuesForParsing);
        Test<float, IFormatProvider?, float, bool>(
            (n, provider, out result) => float.TryParse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result),
            (n, provider, out result) => float.TryParse(n.ToString(provider), provider, out result),
            valuesForParsing,
            formatProviders);
        Test<float, IFormatProvider?, float, bool>(
            (n, provider, out result) => float.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => float.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            valuesForParsing,
            formatProviders);
        Test<float, IFormatProvider?, float, bool>(
            (n, provider, out result) => float.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => float.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            valuesForParsing,
            formatProviders);

        Test(n => float.Round(n, 0), n => float.Round(n), values);
        Test(n => float.Round(n, MidpointRounding.ToEven), n => float.Round(n), values);
        Test((n, mode) => float.Round(n, 0, mode), (n, mode) => float.Round(n, mode), values, roundings);
        Test((n, digits) => float.Round(n, digits, MidpointRounding.ToEven), (n, digits) => float.Round(n, digits), values, digitsValues);

        DoNamedTest();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Half()
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

        // redundant argument

        Test(n => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => half.Parse($"{n}"), values);
        Test(n => half.Parse($"{n}", null), n => half.Parse($"{n}"), values);
        Test(
            (n, provider) => half.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => half.Parse(n.ToString(provider), provider),
            values,
            formatProviders);
        Test((n, style) => half.Parse($"{n}", style, null), (n, style) => half.Parse($"{n}", style), values, floatingPointStyles);
        Test(n => half.Parse($"{n}".AsSpan(), null), n => half.Parse($"{n}".AsSpan()), values);
        Test(n => half.Parse($"{n}".AsUtf8Bytes(), null), n => half.Parse($"{n}".AsUtf8Bytes()), values);

        Test<half, half, bool>(
            (n, out result) => half.TryParse($"{n}", null, out result),
            (n, out result) => half.TryParse($"{n}", out result),
            values);
        Test<half, half, bool>(
            (n, out result) => half.TryParse($"{n}".AsSpan(), null, out result),
            (n, out result) => half.TryParse($"{n}".AsSpan(), out result),
            values);
        Test<half, half, bool>(
            (n, out result) => half.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (n, out result) => half.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test<half, IFormatProvider?, half, bool>(
            (n, provider, out result) => half.TryParse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result),
            (n, provider, out result) => half.TryParse(n.ToString(provider), provider, out result),
            values,
            formatProviders);
        Test<half, IFormatProvider?, half, bool>(
            (n, provider, out result) => half.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => half.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            values,
            formatProviders);
        Test<half, IFormatProvider?, half, bool>(
            (n, provider, out result) => half.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (n, provider, out result) => half.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        Test(n => half.Round(n, 0), n => half.Round(n), values);
        Test(n => half.Round(n, MidpointRounding.ToEven), n => half.Round(n), values);
        Test((n, mode) => half.Round(n, 0, mode), (n, mode) => half.Round(n, mode), values, roundings);
        Test((n, digits) => half.Round(n, digits, MidpointRounding.ToEven), (n, digits) => half.Round(n, digits), values, digitsValues);

        DoNamedTest();
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
    [TestNet60]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Enum()
    {
        var enumValues = new[] { SampleEnum.Red, (SampleEnum)1, (SampleEnum)10 };
        var flagValues = new[] { SampleFlags.Red, SampleFlags.Red | SampleFlags.Blue, (SampleFlags)3, (SampleFlags)0, (SampleFlags)9 };

        // redundant argument

        Test(e => System.Enum.Parse<SampleEnum>(e.ToString(), false), e => System.Enum.Parse<SampleEnum>(e.ToString()), enumValues);
        Test(e => System.Enum.Parse<SampleEnum>(e.ToString().AsSpan(), false), e => System.Enum.Parse<SampleEnum>(e.ToString().AsSpan()), enumValues);
        Test(e => System.Enum.Parse(typeof(SampleEnum), e.ToString(), false), e => System.Enum.Parse(typeof(SampleEnum), e.ToString()), enumValues);
        Test(
            e => System.Enum.Parse(typeof(SampleEnum), e.ToString().AsSpan(), false),
            e => System.Enum.Parse(typeof(SampleEnum), e.ToString().AsSpan()),
            enumValues);
        Test(e => System.Enum.Parse<SampleFlags>(e.ToString(), false), e => System.Enum.Parse<SampleFlags>(e.ToString()), flagValues);
        Test(
            e => System.Enum.Parse<SampleFlags>(e.ToString().AsSpan(), false),
            e => System.Enum.Parse<SampleFlags>(e.ToString().AsSpan()),
            flagValues);
        Test(e => System.Enum.Parse(typeof(SampleFlags), e.ToString(), false), e => System.Enum.Parse(typeof(SampleFlags), e.ToString()), flagValues);
        Test(
            e => System.Enum.Parse(typeof(SampleFlags), e.ToString().AsSpan(), false),
            e => System.Enum.Parse(typeof(SampleFlags), e.ToString().AsSpan()),
            flagValues);

        Test<SampleEnum, SampleEnum, bool>(
            (value, out result) => System.Enum.TryParse($"{value}", false, out result),
            (value, out result) => System.Enum.TryParse($"{value}", out result),
            enumValues);
        Test<SampleEnum, SampleEnum, bool>(
            (value, out result) => System.Enum.TryParse($"{value}".AsSpan(), false, out result),
            (value, out result) => System.Enum.TryParse($"{value}".AsSpan(), out result),
            enumValues);
        Test<SampleEnum, object?, bool>(
            (value, out result) => System.Enum.TryParse(typeof(SampleEnum), $"{value}", false, out result),
            (value, out result) => System.Enum.TryParse(typeof(SampleEnum), $"{value}", out result),
            enumValues);
        Test<SampleEnum, object?, bool>(
            (value, out result) => System.Enum.TryParse(typeof(SampleEnum), $"{value}".AsSpan(), false, out result),
            (value, out result) => System.Enum.TryParse(typeof(SampleEnum), $"{value}".AsSpan(), out result),
            enumValues);
        Test<SampleFlags, SampleFlags, bool>(
            (value, out result) => System.Enum.TryParse($"{value}", false, out result),
            (value, out result) => System.Enum.TryParse($"{value}", out result),
            flagValues);
        Test<SampleFlags, SampleFlags, bool>(
            (value, out result) => System.Enum.TryParse($"{value}".AsSpan(), false, out result),
            (value, out result) => System.Enum.TryParse($"{value}".AsSpan(), out result),
            flagValues);
        Test<SampleFlags, object?, bool>(
            (value, out result) => System.Enum.TryParse(typeof(SampleFlags), $"{value}", false, out result),
            (value, out result) => System.Enum.TryParse(typeof(SampleFlags), $"{value}", out result),
            flagValues);
        Test<SampleFlags, object?, bool>(
            (value, out result) => System.Enum.TryParse(typeof(SampleFlags), $"{value}".AsSpan(), false, out result),
            (value, out result) => System.Enum.TryParse(typeof(SampleFlags), $"{value}".AsSpan(), out result),
            flagValues);

        DoNamedTest();
    }

    [Test]
    [TestNet100]
    public void Guid()
    {
        var values = new[] { System.Guid.Empty, new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]) };

        // redundant argument

        Test(
            (guid, provider) => System.Guid.Parse(guid.ToString(), provider),
            (guid, _) => System.Guid.Parse(guid.ToString()),
            values,
            formatProviders);
        Test(
            (guid, provider) => System.Guid.Parse(guid.ToString().AsSpan(), provider),
            (guid, _) => System.Guid.Parse(guid.ToString().AsSpan()),
            values,
            formatProviders);
        Test(
            (guid, provider) => System.Guid.Parse(guid.ToString().AsUtf8Bytes(), provider),
            (guid, _) => System.Guid.Parse(guid.ToString().AsUtf8Bytes()),
            values,
            formatProviders);

        Test<Guid, IFormatProvider?, Guid, bool>(
            (value, provider, out result) => System.Guid.TryParse($"{value}", provider, out result),
            (value, _, out result) => System.Guid.TryParse($"{value}", out result),
            values,
            formatProviders);
        Test<Guid, IFormatProvider?, Guid, bool>(
            (value, provider, out result) => System.Guid.TryParse($"{value}".AsSpan(), provider, out result),
            (value, _, out result) => System.Guid.TryParse($"{value}".AsSpan(), out result),
            values,
            formatProviders);
        Test<Guid, IFormatProvider?, Guid, bool>(
            (value, provider, out result) => System.Guid.TryParse($"{value}".AsUtf8Bytes(), provider, out result),
            (value, _, out result) => System.Guid.TryParse($"{value}".AsUtf8Bytes(), out result),
            values,
            formatProviders);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet70]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "RedundantFormatProvider")]
    public void TimeSpan()
    {
        var values = new[]
        {
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
        };
        var formats = new[] { "c", "t", "T", "g", "G" };
        var formatsInvariant = new[] { "c", "t", "T" };
        var styles = new[] { TimeSpanStyles.None, TimeSpanStyles.AssumeNegative };

        // redundant argument

        Test(() => new TimeSpan(0, 1, 2, 3), () => new TimeSpan(1, 2, 3));
        Test(() => new TimeSpan(1, 2, 3, 4, 0), () => new TimeSpan(1, 2, 3, 4));
        Test(() => System.TimeSpan._Ctor(1, 2, 3, 4, 5, 0), () => new TimeSpan(1, 2, 3, 4, 5));

        Test(timeSpan => System.TimeSpan.Parse($"{timeSpan}", null), timeSpan => System.TimeSpan.Parse($"{timeSpan}"), values);

        Test(
            (timeSpan, format, provider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, provider), format, provider, TimeSpanStyles.None),
            (timeSpan, format, provider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, provider), format, provider),
            values,
            formats,
            formatProviders);
        Test(
            (timeSpan, provider) => System.TimeSpan.ParseExact($"{timeSpan}", formats, provider, TimeSpanStyles.None),
            (timeSpan, provider) => System.TimeSpan.ParseExact($"{timeSpan}", formats, provider),
            values,
            formatProviders);

        Test<TimeSpan, TimeSpan, bool>(
            (timeSpan, out result) => System.TimeSpan.TryParse($"{timeSpan}", null, out result),
            (timeSpan, out result) => System.TimeSpan.TryParse($"{timeSpan}", out result),
            values);
        Test<TimeSpan, TimeSpan, bool>(
            (timeSpan, out result) => System.TimeSpan.TryParse($"{timeSpan}".AsSpan(), null, out result),
            (timeSpan, out result) => System.TimeSpan.TryParse($"{timeSpan}".AsSpan(), out result),
            values);

        Test<TimeSpan, string, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider),
                format,
                provider,
                TimeSpanStyles.None,
                out result),
            (timeSpan, format, provider, out result)
                => System.TimeSpan.TryParseExact(timeSpan.ToString(format, provider), format, provider, out result),
            values,
            formats,
            formatProviders);
        Test<TimeSpan, string, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider).AsSpan(),
                format.AsSpan(),
                provider,
                TimeSpanStyles.None,
                out result),
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider).AsSpan(),
                format.AsSpan(),
                provider,
                out result),
            values,
            formats,
            formatProviders);
        Test<TimeSpan, string, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider),
                formats,
                provider,
                TimeSpanStyles.None,
                out result),
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider),
                formats,
                provider,
                out result),
            values,
            formats,
            formatProviders);
        Test<TimeSpan, string, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider).AsSpan(),
                formats,
                provider,
                TimeSpanStyles.None,
                out result),
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider).AsSpan(),
                formats,
                provider,
                out result),
            values,
            formats,
            formatProviders);

        // redundant collection element

        Test(
            (timeSpan, provider) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "t", "T", "g", "g", "G"], provider),
            (timeSpan, provider) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "g", "G"], provider),
            values,
            formatProviders);
        Test(
            (timeSpan, provider, style) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "t", "T", "g", "g", "G"], provider, style),
            (timeSpan, provider, style) => System.TimeSpan.ParseExact($"{timeSpan}", ["c", "g", "G"], provider, style),
            values,
            formatProviders,
            styles);
        Test(
            (timeSpan, provider, style) => System.TimeSpan.ParseExact($"{timeSpan}".AsSpan(), ["c", "t", "T", "g", "g", "G"], provider, style),
            (timeSpan, provider, style) => System.TimeSpan.ParseExact($"{timeSpan}".AsSpan(), ["c", "g", "G"], provider, style),
            values,
            formatProviders,
            styles);

        Test<TimeSpan, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider),
                ["c", "t", "T", "g", "g", "G"],
                provider,
                out result),
            (timeSpan, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider),
                ["c", "g", "G"],
                provider,
                out result),
            values,
            formatProviders);
        Test<TimeSpan, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider).AsSpan(),
                ["c", "t", "T", "g", "g", "G"],
                provider,
                out result),
            (timeSpan, provider, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider).AsSpan(),
                ["c", "g", "G"],
                provider,
                out result),
            values,
            formatProviders);
        Test<TimeSpan, IFormatProvider?, TimeSpanStyles, TimeSpan, bool>(
            (timeSpan, provider, style, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider),
                ["c", "t", "T", "g", "g", "G"],
                provider,
                style,
                out result),
            (timeSpan, provider, style, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider),
                ["c", "g", "G"],
                provider,
                style,
                out result),
            values,
            formatProviders,
            styles);
        Test<TimeSpan, IFormatProvider?, TimeSpanStyles, TimeSpan, bool>(
            (timeSpan, provider, style, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider).AsSpan(),
                ["c", "t", "T", "g", "g", "G"],
                provider,
                style,
                out result),
            (timeSpan, provider, style, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString("c", provider).AsSpan(),
                ["c", "g", "G"],
                provider,
                style,
                out result),
            values,
            formatProviders,
            styles);

        // other argument

        Test(
            (timeSpan, format, provider) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, provider),
            (timeSpan, format, _) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, null),
            values,
            formatsInvariant,
            formatProviders);
        Test(
            (timeSpan, format, provider, style) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, provider, style),
            (timeSpan, format, _, style) => System.TimeSpan.ParseExact(timeSpan.ToString(format), format, null, style),
            values,
            formatsInvariant,
            formatProviders,
            styles);
        Test(
            (timeSpan, format, provider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, provider), [format], provider),
            (timeSpan, format, provider) => System.TimeSpan.ParseExact(timeSpan.ToString(format, provider), format, provider),
            values,
            formats,
            formatProviders);
        Test(
            (timeSpan, format, provider, style) => System.TimeSpan.ParseExact(timeSpan.ToString(format, provider), [format], provider, style),
            (timeSpan, format, provider, style) => System.TimeSpan.ParseExact(timeSpan.ToString(format, provider), format, provider, style),
            values,
            formats,
            formatProviders,
            styles);

        Test<TimeSpan, string, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact($"{timeSpan}", format, provider, out result),
            (timeSpan, format, _, out result) => System.TimeSpan.TryParseExact($"{timeSpan}", format, null, out result),
            values,
            formatsInvariant,
            formatProviders);
        Test<TimeSpan, string, IFormatProvider?, TimeSpanStyles, TimeSpan, bool>(
            (timeSpan, format, provider, style, out result) => System.TimeSpan.TryParseExact($"{timeSpan}", format, provider, style, out result),
            (timeSpan, format, _, style, out result) => System.TimeSpan.TryParseExact($"{timeSpan}", format, null, style, out result),
            values,
            formatsInvariant,
            formatProviders,
            styles);
        Test<TimeSpan, string, IFormatProvider?, TimeSpan, bool>(
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact($"{timeSpan}", [format], provider, out result),
            (timeSpan, format, provider, out result) => System.TimeSpan.TryParseExact($"{timeSpan}", format, provider, out result),
            values,
            formats,
            formatProviders);
        Test<TimeSpan, string, IFormatProvider?, TimeSpanStyles, TimeSpan, bool>(
            (timeSpan, format, provider, style, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider),
                [format],
                provider,
                style,
                out result),
            (timeSpan, format, provider, style, out result) => System.TimeSpan.TryParseExact(
                timeSpan.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            styles);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantArgumentRange")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    public void DateTime()
    {
        var values = new[]
        {
            System.DateTime.MinValue,
            System.DateTime.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
        };
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" };
        var formatsInvariant = new[] { "o", "O", "r", "R", "s", "u" };

        // redundant argument

        Test(ticks => new DateTime(ticks, DateTimeKind.Unspecified), ticks => new DateTime(ticks), [0, 1, 638_882_119_800_000_000]);
        Test(
            dateTime => System.DateTime._Ctor(dateOnly.FromDateTime(dateTime), timeOnly.FromDateTime(dateTime), DateTimeKind.Unspecified),
            dateTime => System.DateTime._Ctor(dateOnly.FromDateTime(dateTime), timeOnly.FromDateTime(dateTime)),
            values);
        Test(
            dateTime => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                DateTimeKind.Unspecified),
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second),
            values);
        Test(
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0),
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second),
            values);
        Test(
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0, dateTime.Kind),
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind),
            values);
        Test(
            dateTime => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                DateTimeKind.Unspecified),
            dateTime => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond),
            values);
        Test(
            (dateTime, calendar) => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                0,
                calendar),
            (dateTime, calendar) => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                calendar),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);
        Test(
            (dateTime, calendar) => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar,
                DateTimeKind.Unspecified),
            (dateTime, calendar) => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);
        Test(
            dateTime => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Microsecond),
            dateTime => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])]);
        Test(
            (dateTime, calendar) => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                0,
                calendar),
            (dateTime, calendar) => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);
        Test(
            dateTime => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                0,
                dateTime.Kind),
            dateTime => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Kind),
            values);
        Test(
            dateTime => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Microsecond,
                DateTimeKind.Unspecified),
            dateTime => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Microsecond),
            values);
        Test(
            (dateTime, calendar) => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                0,
                calendar,
                dateTime.Kind),
            (dateTime, calendar) => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                calendar,
                dateTime.Kind),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);
        Test(
            (dateTime, calendar) => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Microsecond,
                calendar,
                DateTimeKind.Unspecified),
            (dateTime, calendar) => System.DateTime._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.Microsecond,
                calendar),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        Test(dateTime => dateTime.GetDateTimeFormats(null), dateTime => dateTime.GetDateTimeFormats(), values);
        Test(
            (dateTime, format) => dateTime.GetDateTimeFormats(format, null),
            (dateTime, format) => dateTime.GetDateTimeFormats(format),
            values,
            [..from f in formats select f[0]]);

        Test(dateTime => System.DateTime.Parse($"{dateTime}", null), dateTime => System.DateTime.Parse($"{dateTime}"), values);
        Test(dateTime => System.DateTime.Parse($"{dateTime}".AsSpan(), null), dateTime => System.DateTime.Parse($"{dateTime}".AsSpan()), values);
        Test(
            (dateTime, provider) => System.DateTime.Parse(dateTime.ToString(provider), provider, DateTimeStyles.None),
            (dateTime, provider) => System.DateTime.Parse(dateTime.ToString(provider), provider),
            values,
            formatProviders);

        Test(
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, DateTimeStyles.None),
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider),
            values,
            formats,
            formatProviders);

        Test<DateTime, DateTime, bool>(
            (dateTime, out result) => System.DateTime.TryParse($"{dateTime}", null, out result),
            (dateTime, out result) => System.DateTime.TryParse($"{dateTime}", out result),
            values);
        Test<DateTime, DateTime, bool>(
            (dateTime, out result) => System.DateTime.TryParse($"{dateTime}".AsSpan(), null, out result),
            (dateTime, out result) => System.DateTime.TryParse($"{dateTime}".AsSpan(), out result),
            values);
        Test<DateTime, IFormatProvider?, DateTime, bool>(
            (dateTime, provider, out result) => System.DateTime.TryParse($"{dateTime}", provider, DateTimeStyles.None, out result),
            (dateTime, provider, out result) => System.DateTime.TryParse($"{dateTime}", provider, out result),
            values,
            formatProviders);
        Test<DateTime, IFormatProvider?, DateTime, bool>(
            (dateTime, provider, out result) => System.DateTime.TryParse($"{dateTime}".AsSpan(), provider, DateTimeStyles.None, out result),
            (dateTime, provider, out result) => System.DateTime.TryParse($"{dateTime}".AsSpan(), provider, out result),
            values,
            formatProviders);

        // redundant argument range

        Test(
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0),
            dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day),
            values);
        Test(
            (dateTime, calendar) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, calendar),
            (dateTime, calendar) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, calendar),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            calendars);

        // redundant collection element

        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                provider,
                style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                provider,
                style),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                provider,
                style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                provider,
                style),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);

        Test<DateTime, string, IFormatProvider?, DateTimeStyles, DateTime, bool>(
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                provider,
                style,
                out result),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test<DateTime, string, IFormatProvider?, DateTimeStyles, DateTime, bool>(
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"],
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"],
                provider,
                style,
                out result),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);

        // other argument

        Test(
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider),
            (dateTime, format, provider) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, null),
            values,
            formatsInvariant,
            formatProviders);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, null, style),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), [format], provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, style),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), formatsInvariant, provider, style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(dateTime.ToString(format, provider), formatsInvariant, null, style),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style),
            (dateTime, format, provider, style) => System.DateTime.ParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                formatsInvariant,
                null,
                style),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        Test<DateTime, string, IFormatProvider?, DateTimeStyles, DateTime, bool>(
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                format,
                null,
                style,
                out result),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<DateTime, string, IFormatProvider?, DateTimeStyles, DateTime, bool>(
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                [format],
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test<DateTime, string, IFormatProvider?, DateTimeStyles, DateTime, bool>(
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                formatsInvariant,
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider),
                formatsInvariant,
                null,
                style,
                out result),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<DateTime, string, IFormatProvider?, DateTimeStyles, DateTime, bool>(
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTime.TryParseExact(
                dateTime.ToString(format, provider).AsSpan(),
                formatsInvariant,
                null,
                style,
                out result),
            [..values.Except([System.DateTime.MinValue, System.DateTime.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet70]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    public void DateTimeOffset()
    {
        var values = new[]
        {
            System.DateTimeOffset.MinValue,
            System.DateTimeOffset.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.Zero),
            new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(2)),
            new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(-6)),
        };
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" };
        var formatsInvariant = new[] { "o", "O", "r", "R", "s", "u" };

        // redundant argument

        Test(
            dateTimeOffset => new DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                0,
                dateTimeOffset.Offset),
            dateTimeOffset => new DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Offset),
            values);
        Test(
            dateTimeOffset => System.DateTimeOffset._Ctor(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                0,
                dateTimeOffset.Offset),
            dateTimeOffset => new DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                dateTimeOffset.Offset),
            values);
        Test(
            (dateTimeOffset, calendar) => System.DateTimeOffset._Ctor(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                0,
                calendar,
                dateTimeOffset.Offset),
            (dateTimeOffset, calendar) => new DateTimeOffset(
                dateTimeOffset.Year,
                dateTimeOffset.Month,
                dateTimeOffset.Day,
                dateTimeOffset.Hour,
                dateTimeOffset.Minute,
                dateTimeOffset.Second,
                dateTimeOffset.Millisecond,
                calendar,
                dateTimeOffset.Offset),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            calendars);

        Test(
            dateTimeOffset => System.DateTimeOffset.Parse($"{dateTimeOffset}", null),
            dateTimeOffset => System.DateTimeOffset.Parse($"{dateTimeOffset}"),
            values);
        Test(
            dateTimeOffset => System.DateTimeOffset.Parse($"{dateTimeOffset}".AsSpan(), null),
            dateTimeOffset => System.DateTimeOffset.Parse($"{dateTimeOffset}".AsSpan()),
            values);
        Test(
            (dateTimeOffset, provider) => System.DateTimeOffset.Parse(dateTimeOffset.ToString(provider), provider, DateTimeStyles.None),
            (dateTimeOffset, provider) => System.DateTimeOffset.Parse(dateTimeOffset.ToString(provider), provider),
            values,
            formatProviders);

        Test<DateTimeOffset, DateTimeOffset, bool>(
            (dateTimeOffset, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}", null, out result),
            (dateTimeOffset, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}", out result),
            values);
        Test<DateTimeOffset, DateTimeOffset, bool>(
            (dateTimeOffset, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}".AsSpan(), null, out result),
            (dateTimeOffset, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}".AsSpan(), out result),
            values);
        Test<DateTimeOffset, IFormatProvider?, DateTimeOffset, bool>(
            (dateTimeOffset, provider, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}", provider, DateTimeStyles.None, out result),
            (dateTimeOffset, provider, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}", provider, out result),
            values,
            formatProviders);
        Test<DateTimeOffset, IFormatProvider?, DateTimeOffset, bool>(
            (dateTimeOffset, provider, out result) => System.DateTimeOffset.TryParse(
                $"{dateTimeOffset}".AsSpan(),
                provider,
                DateTimeStyles.None,
                out result),
            (dateTimeOffset, provider, out result) => System.DateTimeOffset.TryParse($"{dateTimeOffset}".AsSpan(), provider, out result),
            values,
            formatProviders);

        Test(
            (dateTimeOffset, format, provider) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                format,
                provider,
                DateTimeStyles.None),
            (dateTimeOffset, format, provider) => System.DateTimeOffset.ParseExact(dateTimeOffset.ToString(format, provider), format, provider),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders);

        // redundant collection element

        Test(
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                provider,
                style),
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                provider,
                style),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                provider,
                style),
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                provider,
                style),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);

        Test<DateTimeOffset, string, IFormatProvider?, DateTimeStyles, DateTimeOffset, bool>(
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                provider,
                style,
                out result),
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                provider,
                style,
                out result),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test<DateTimeOffset, string, IFormatProvider?, DateTimeStyles, DateTimeOffset, bool>(
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"],
                provider,
                style,
                out result),
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"],
                provider,
                style,
                out result),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);

        // other argument

        Test(
            (dateTimeOffset, format, provider) => System.DateTimeOffset.ParseExact(dateTimeOffset.ToString(format, provider), format, provider),
            (dateTimeOffset, format, provider) => System.DateTimeOffset.ParseExact(dateTimeOffset.ToString(format, provider), format, null),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders);
        Test(
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                format,
                provider,
                style),
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(dateTimeOffset.ToString(format, provider), format, null, style),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                [format],
                provider,
                style),
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                format,
                provider,
                style),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                formatsInvariant,
                provider,
                style),
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider),
                formatsInvariant,
                null,
                style),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style),
            (dateTimeOffset, format, provider, style) => System.DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                formatsInvariant,
                null,
                style),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        Test<DateTimeOffset, string, IFormatProvider?, DateTimeStyles, DateTimeOffset, bool>(
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                format,
                null,
                style,
                out result),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<DateTimeOffset, string, IFormatProvider?, DateTimeStyles, DateTimeOffset, bool>(
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                [format],
                provider,
                style,
                out result),
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formats,
            formatProviders,
            dateTimeStyles);
        Test<DateTimeOffset, string, IFormatProvider?, DateTimeStyles, DateTimeOffset, bool>(
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider),
                formatsInvariant,
                provider,
                style,
                out result),
            (dateTime, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTime.ToString(format, provider),
                formatsInvariant,
                null,
                style,
                out result),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<DateTimeOffset, string, IFormatProvider?, DateTimeStyles, DateTimeOffset, bool>(
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style,
                out result),
            (dateTimeOffset, format, provider, style, out result) => System.DateTimeOffset.TryParseExact(
                dateTimeOffset.ToString(format, provider).AsSpan(),
                formatsInvariant,
                null,
                style,
                out result),
            [..values.Except([System.DateTimeOffset.MinValue, System.DateTimeOffset.MaxValue])],
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet70]
    public void DateOnly()
    {
        var values = new[] { dateOnly.MinValue, dateOnly.MaxValue, new(2025, 7, 15) };
        var formats = new[] { "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" };
        var formatsInvariant = new[] { "o", "O", "r", "R" };

        // redundant argument

        Test(dateOnly => dateOnly.Parse(dateOnly.ToString(), null), dateOnly => dateOnly.Parse(dateOnly.ToString()), values);
        Test(dateOnly => dateOnly.Parse(dateOnly.ToString().AsSpan(), null), dateOnly => dateOnly.Parse(dateOnly.ToString().AsSpan()), values);

        Test<dateOnly, dateOnly, bool>(
            (dateOnly, out result) => dateOnly.TryParse(dateOnly.ToString(), null, out result),
            (dateOnly, out result) => dateOnly.TryParse(dateOnly.ToString(), out result),
            values);
        Test<dateOnly, dateOnly, bool>(
            (dateOnly, out result) => dateOnly.TryParse(dateOnly.ToString().AsSpan(), null, out result),
            (dateOnly, out result) => dateOnly.TryParse(dateOnly.ToString().AsSpan(), out result),
            values);
        Test<dateOnly, IFormatProvider?, dateOnly, bool>(
            (dateOnly, provider, out result) => dateOnly.TryParse(dateOnly.ToString(), provider, DateTimeStyles.None, out result),
            (dateOnly, provider, out result) => dateOnly.TryParse(dateOnly.ToString(), provider, out result),
            values,
            formatProviders);
        Test<dateOnly, IFormatProvider?, dateOnly, bool>(
            (dateOnly, provider, out result) => dateOnly.TryParse(dateOnly.ToString().AsSpan(), provider, DateTimeStyles.None, out result),
            (dateOnly, provider, out result) => dateOnly.TryParse(dateOnly.ToString().AsSpan(), provider, out result),
            values,
            formatProviders);

        // redundant argument range

        Test(dateOnly => dateOnly.Parse(dateOnly.ToString(), null, DateTimeStyles.None), dateOnly => dateOnly.Parse(dateOnly.ToString()), values);

        Test(
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), format, null, DateTimeStyles.None),
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), format),
            values,
            formats);
        Test(
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), formats, null),
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), formats),
            values,
            formats);
        Test(
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format).AsSpan(), formats, null),
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format).AsSpan(), formats),
            values,
            formats);

        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), format, null, DateTimeStyles.None, out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), format, out result),
            values,
            formats);
        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format).AsSpan(),
                format.AsSpan(),
                null,
                DateTimeStyles.None,
                out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format).AsSpan(), format.AsSpan(), out result),
            values,
            formats);
        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), formats, null, DateTimeStyles.None, out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), format, out result),
            values,
            formats);
        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format).AsSpan(),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format).AsSpan(), formats, out result),
            values,
            formats);

        // redundant collection element

        Test(
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]),
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), ["d", "D", "m", "o", "r", "y"]),
            values,
            formats);
        Test(
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format).AsSpan(), ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]),
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format).AsSpan(), ["d", "D", "m", "o", "r", "y"]),
            values,
            formats);
        Test(
            (dateOnly, format, provider, style) => dateOnly.ParseExact(
                dateOnly.ToString(format, provider),
                ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"],
                provider,
                style),
            (dateOnly, format, provider, style) => dateOnly.ParseExact(
                dateOnly.ToString(format, provider),
                ["d", "D", "m", "o", "r", "y"],
                provider,
                style),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateOnly, format, provider, style) => dateOnly.ParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"],
                provider,
                style),
            (dateOnly, format, provider, style) => dateOnly.ParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                ["d", "D", "m", "o", "r", "y"],
                provider,
                style),
            values,
            formats,
            formatProviders,
            dateTimeStyles);

        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format),
                ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"],
                out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), ["d", "D", "m", "o", "r", "y"], out result),
            values,
            formats);
        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format).AsSpan(),
                ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"],
                out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format).AsSpan(), ["d", "D", "m", "o", "r", "y"], out result),
            values,
            formats);
        Test<dateOnly, string, IFormatProvider?, DateTimeStyles, dateOnly, bool>(
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider),
                ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"],
                provider,
                style,
                out result),
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider),
                ["d", "D", "m", "o", "r", "y"],
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test<dateOnly, string, IFormatProvider?, DateTimeStyles, dateOnly, bool>(
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"],
                provider,
                style,
                out result),
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                ["d", "D", "m", "o", "r", "y"],
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            dateTimeStyles);

        // other argument

        Test(
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), [format]),
            (dateOnly, format) => dateOnly.ParseExact(dateOnly.ToString(format), format),
            values,
            formats);
        Test(
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider), format, provider, style),
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider), format, null, style),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider), [format], provider, style),
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider), format, provider, style),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider), formatsInvariant, provider, style),
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider), formatsInvariant, null, style),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (dateOnly, format, provider, style) => dateOnly.ParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style),
            (dateOnly, format, provider, style) => dateOnly.ParseExact(dateOnly.ToString(format, provider).AsSpan(), formatsInvariant, null, style),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        Test<dateOnly, string, dateOnly, bool>(
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), [format], out result),
            (dateOnly, format, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), format, out result),
            values,
            formats);
        Test<dateOnly, string, IFormatProvider?, DateTimeStyles, dateOnly, bool>(
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), format, provider, style, out result),
            (dateOnly, format, _, style, out result) => dateOnly.TryParseExact(dateOnly.ToString(format), format, null, style, out result),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<dateOnly, string, IFormatProvider?, DateTimeStyles, dateOnly, bool>(
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider),
                [format],
                provider,
                style,
                out result),
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test<dateOnly, string, IFormatProvider?, DateTimeStyles, dateOnly, bool>(
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider),
                formatsInvariant,
                provider,
                style,
                out result),
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider),
                formatsInvariant,
                null,
                style,
                out result),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<dateOnly, string, IFormatProvider?, DateTimeStyles, dateOnly, bool>(
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style,
                out result),
            (dateOnly, format, provider, style, out result) => dateOnly.TryParseExact(
                dateOnly.ToString(format, provider).AsSpan(),
                formatsInvariant,
                null,
                style,
                out result),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet70]
    public void TimeOnly()
    {
        var values = new[] { timeOnly.MinValue, timeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5) };
        var formats = new[] { "t", "T", "o", "O", "r", "R" };
        var formatsInvariant = new[] { "o", "O", "r", "R" };

        // redundant argument

        Test(
            (timeOnly, value) => timeOnly.Add(value, out _),
            (timeOnly, value) => timeOnly.Add(value),
            values,
            [System.TimeSpan.FromDays(2), System.TimeSpan.FromDays(-2), System.TimeSpan.FromHours(23), System.TimeSpan.FromHours(-23)]);

        Test((timeOnly, value) => timeOnly.AddHours(value, out _), (timeOnly, value) => timeOnly.AddHours(value), values, [48, -48, 23, -23]);

        Test(
            (timeOnly, value) => timeOnly.AddMinutes(value, out _),
            (timeOnly, value) => timeOnly.AddMinutes(value),
            values,
            [48 * 60, -48 * 60, 23 * 60, -23 * 60]);

        Test(timeOnly => timeOnly.Parse(timeOnly.ToString(), null), timeOnly => timeOnly.Parse(timeOnly.ToString()), values);
        Test(timeOnly => timeOnly.Parse(timeOnly.ToString().AsSpan(), null), timeOnly => timeOnly.Parse(timeOnly.ToString().AsSpan()), values);

        Test(
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), formats, null),
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), formats),
            values,
            formats);
        Test(
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), formats, null),
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), formats),
            values,
            formats);

        Test<timeOnly, timeOnly, bool>(
            (timeOnly, out result) => timeOnly.TryParse(timeOnly.ToString(), null, out result),
            (timeOnly, out result) => timeOnly.TryParse(timeOnly.ToString(), out result),
            values);
        Test<timeOnly, timeOnly, bool>(
            (timeOnly, out result) => timeOnly.TryParse(timeOnly.ToString().AsSpan(), null, out result),
            (timeOnly, out result) => timeOnly.TryParse(timeOnly.ToString().AsSpan(), out result),
            values);
        Test<timeOnly, IFormatProvider?, timeOnly, bool>(
            (timeOnly, provider, out result) => timeOnly.TryParse(timeOnly.ToString(), provider, DateTimeStyles.None, out result),
            (timeOnly, provider, out result) => timeOnly.TryParse(timeOnly.ToString(), provider, out result),
            values,
            formatProviders);
        Test<timeOnly, IFormatProvider?, timeOnly, bool>(
            (timeOnly, provider, out result) => timeOnly.TryParse(timeOnly.ToString().AsSpan(), provider, DateTimeStyles.None, out result),
            (timeOnly, provider, out result) => timeOnly.TryParse(timeOnly.ToString().AsSpan(), provider, out result),
            values,
            formatProviders);

        // redundant argument range

        Test(timeOnly => timeOnly.Parse(timeOnly.ToString(), null, DateTimeStyles.None), timeOnly => timeOnly.Parse(timeOnly.ToString()), values);

        Test(
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), format, null, DateTimeStyles.None),
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), format),
            values,
            formats);

        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), format, null, DateTimeStyles.None, out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
            values,
            formats);
        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                format.AsSpan(),
                null,
                DateTimeStyles.None,
                out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format).AsSpan(), format.AsSpan(), out result),
            values,
            formats);
        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), formats, null, DateTimeStyles.None, out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), formats, out result),
            values,
            formats);
        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format).AsSpan(), formats, out result),
            values,
            formats);

        // redundant collection element

        Test(
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), ["t", "t", "T", "o", "O", "r", "R"]),
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), ["t", "T", "o", "r"]),
            values,
            formats);
        Test(
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), ["t", "t", "T", "o", "O", "r", "R"]),
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), ["t", "T", "o", "r"]),
            values,
            formats);
        Test(
            (timeOnly, format, provider, style) => timeOnly.ParseExact(
                timeOnly.ToString(format, provider),
                ["t", "t", "T", "o", "O", "r", "R"],
                provider,
                style),
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), ["t", "T", "o", "r"], provider, style),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (timeOnly, format, provider, style) => timeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                ["t", "t", "T", "o", "O", "r", "R"],
                provider,
                style),
            (timeOnly, format, provider, style) => timeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                ["t", "T", "o", "r"],
                provider,
                style),
            values,
            formats,
            formatProviders,
            dateTimeStyles);

        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), ["t", "t", "T", "o", "O", "r", "R"], out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), ["t", "T", "o", "r"], out result),
            values,
            formats);
        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                ["t", "t", "T", "o", "O", "r", "R"],
                out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format).AsSpan(), ["t", "T", "o", "r"], out result),
            values,
            formats);
        Test<timeOnly, string, IFormatProvider?, DateTimeStyles, timeOnly, bool>(
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider),
                ["t", "t", "T", "o", "O", "r", "R"],
                provider,
                style,
                out result),
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider),
                ["t", "T", "o", "r"],
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test<timeOnly, string, IFormatProvider?, DateTimeStyles, timeOnly, bool>(
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                ["t", "t", "T", "o", "O", "r", "R"],
                provider,
                style,
                out result),
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                ["t", "T", "o", "r"],
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            dateTimeStyles);

        // other argument

        Test(
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), [format]),
            (timeOnly, format) => timeOnly.ParseExact(timeOnly.ToString(format), format),
            values,
            formats);
        Test(
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), format, provider, style),
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), format, null, style),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), [format], provider, style),
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), format, provider, style),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test(
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), formatsInvariant, provider, style),
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider), formatsInvariant, null, style),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test(
            (timeOnly, format, provider, style) => timeOnly.ParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style),
            (timeOnly, format, provider, style) => timeOnly.ParseExact(timeOnly.ToString(format, provider).AsSpan(), formatsInvariant, null, style),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        Test<timeOnly, string, timeOnly, bool>(
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), [format], out result),
            (timeOnly, format, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
            values,
            formats);
        Test<timeOnly, string, IFormatProvider?, DateTimeStyles, timeOnly, bool>(
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), format, provider, style, out result),
            (timeOnly, format, _, style, out result) => timeOnly.TryParseExact(timeOnly.ToString(format), format, null, style, out result),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<timeOnly, string, IFormatProvider?, DateTimeStyles, timeOnly, bool>(
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider),
                [format],
                provider,
                style,
                out result),
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider),
                format,
                provider,
                style,
                out result),
            values,
            formats,
            formatProviders,
            dateTimeStyles);
        Test<timeOnly, string, IFormatProvider?, DateTimeStyles, timeOnly, bool>(
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider),
                formatsInvariant,
                provider,
                style,
                out result),
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider),
                formatsInvariant,
                null,
                style,
                out result),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);
        Test<timeOnly, string, IFormatProvider?, DateTimeStyles, timeOnly, bool>(
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                formatsInvariant,
                provider,
                style,
                out result),
            (timeOnly, format, provider, style, out result) => timeOnly.TryParseExact(
                timeOnly.ToString(format, provider).AsSpan(),
                formatsInvariant,
                null,
                style,
                out result),
            values,
            formatsInvariant,
            formatProviders,
            dateTimeStyles);

        DoNamedTest();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantExplicitParamsArrayCreation")]
    [SuppressMessage("ReSharper", "RedundantElement")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "UseOtherArgumentRange")]
    public void String()
    {
        var values = new[] { null, "", "abcde", "  abcde  ", "ab;cd;e", "ab;cd:e", "..abcde.." };
        var comparisons = new[]
        {
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase,
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
        };
        var stringSplitOptions = new[] { StringSplitOptions.None, StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.TrimEntries };

        // redundant argument

        Test(text => text?.IndexOf('c', 0), text => text?.IndexOf('c'), values);
        Test(text => text?.IndexOf("bcd", 0), text => text?.IndexOf("bcd"), values);
        Test((text, comparison) => text?.IndexOf("bcd", 0, comparison), (text, comparison) => text?.IndexOf("bcd", comparison), values, comparisons);

        Test(text => text?.IndexOfAny(['b', 'c'], 0), text => text?.IndexOfAny(['b', 'c']), values);

        Test(text => text?.PadLeft(3, ' '), text => text?.PadLeft(3), values);

        Test(text => text?.Split(';', ';'), text => text?.Split(';'), values);

        Test(text => text?.Trim(null), text => text?.Trim(), values);
        Test(text => text?.Trim([]), text => text?.Trim(), values);
        Test(text => text?.Trim('.', '.'), text => text?.Trim('.'), values);

        Test(text => text?.TrimEnd(null), text => text?.TrimEnd(), values);
        Test(text => text?.TrimEnd([]), text => text?.TrimEnd(), values);
        Test(text => text?.TrimEnd('.', '.'), text => text?.TrimEnd('.'), values);

        Test(text => text?.TrimStart(null), text => text?.TrimStart(), values);
        Test(text => text?.TrimStart([]), text => text?.TrimStart(), values);
        Test(text => text?.TrimStart('.', '.'), text => text?.TrimStart('.'), values);

        // redundant collection element

        Test(text => text?.IndexOfAny(['b', 'c', 'c']), text => text?.IndexOfAny(['b', 'c']), values);
        Test(text => text?.IndexOfAny(['b', 'c', 'c'], 1), text => text?.IndexOfAny(['b', 'c'], 1), [..values.Except([""])]);
        Test(text => text?.IndexOfAny(['b', 'c', 'c'], 1, 3), text => text?.IndexOfAny(['b', 'c'], 1, 3), [..values.Except([""])]);

        Test(text => text?.LastIndexOfAny(['b', 'c', 'c']), text => text?.LastIndexOfAny(['b', 'c']), values);
        Test(text => text?.LastIndexOfAny(['b', 'c', 'c'], 4), text => text?.LastIndexOfAny(['b', 'c'], 4), values);
        Test(text => text?.LastIndexOfAny(['b', 'c', 'c'], 4, 3), text => text?.LastIndexOfAny(['b', 'c'], 4, 3), values);

        Test(text => text?.Split([';', ';']), text => text?.Split([';']), values);
        Test(text => text?.Split([';', ';'], 2), text => text?.Split([';'], 2), values);
        Test((text, options) => text?.Split_([';', ';'], options), (text, options) => text?.Split_([';'], options), values, stringSplitOptions);
        Test((text, options) => text?.Split_([';', ';'], 2, options), (text, options) => text?.Split_([';'], 2, options), values, stringSplitOptions);
        Test((text, options) => text?.Split_([";", ";"], options), (text, options) => text?.Split_([";"], options), values, stringSplitOptions);
        Test((text, options) => text?.Split_([";", ";"], 2, options), (text, options) => text?.Split_([";"], 2, options), values, stringSplitOptions);

        Test(text => text?.Trim(['.', '.']), text => text?.Trim(['.']), values);

        Test(text => text?.TrimEnd(['.', '.']), text => text?.TrimEnd(['.']), values);

        Test(text => text?.TrimStart(['.', '.']), text => text?.TrimStart(['.']), values);

        // other argument

        Test(text => text?.Contains("c"), text => text?.Contains('c'), values);
        Test((text, comparison) => text?.Contains("c", comparison), (text, comparison) => text?.Contains('c', comparison), values, comparisons);

        Test(text => text?.IndexOf("c"), text => text?.IndexOf('c', StringComparison.CurrentCulture), values);
        Test((text, comparison) => text?.IndexOf("c", comparison), (text, comparison) => text?.IndexOf('c', comparison), values, comparisons);

        Test(() => string.Join(";", (IEnumerable<int>)[1, 2, 3]), () => string.Join(';', (IEnumerable<int>)[1, 2, 3]));
        Test(() => string.Join(";", (string?[])["one", "two", "three", null]), () => string.Join(';', (string?[])["one", "two", "three", null]));
        Test(
            () => string.Join(";", (string?[])["one", "two", "three", null], 1, 2),
            () => string.Join(';', (string?[])["one", "two", "three", null], 1, 2));
        Test(() => string.Join(";", (object?[])[1, "two", true, null]), () => string.Join(';', (object?[])[1, "two", true, null]));
        Test(
            () => string.Join(";", (ReadOnlySpan<string?>)["one", "two", "three", null]),
            () => string.Join(';', (ReadOnlySpan<string?>)["one", "two", "three", null]));
        Test(
            () => string.Join(";", (ReadOnlySpan<object?>)[1, "two", true, null]),
            () => string.Join(';', (ReadOnlySpan<object?>)[1, "two", true, null]));

        Test(text => text?.LastIndexOf("c", StringComparison.Ordinal), text => text?.LastIndexOf('c'), values);

        Test((text, options) => text?.Split(";", options), (text, options) => text?.Split(';', options), values, stringSplitOptions);
        Test((text, options) => text?.Split(";", 2, options), (text, options) => text?.Split(';', 2, options), values, stringSplitOptions);
        Test((text, options) => text?.Split_([";", ":"], options), (text, options) => text?.Split_([';', ':'], options), values, stringSplitOptions);
        Test(
            (text, options) => text?.Split_([";", ":"], 10, options),
            (text, options) => text?.Split_([';', ':'], 10, options),
            values,
            stringSplitOptions);

        // other argument range

        Test(text => text?.Replace("c", "x"), text => text?.Replace('c', 'x'), values);
        Test(text => text?.Replace("c", "x", StringComparison.Ordinal), text => text?.Replace('c', 'x'), values);

        DoNamedTest();
    }

    [Test]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "UseOtherArgument")]
    [SuppressMessage("ReSharper", "UseOtherArgumentRange")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public void StringBuilder()
    {
        var values = new[] { "", "abcde" };

        // redundant argument

        TestStringBuilder(builder => builder.Append('x', 1), builder => builder.Append('x'), values);

        TestStringBuilder(builder => builder.Insert(1, "xyz", 1), builder => builder.Insert(1, "xyz"), [..values.Except([""])]);

        // other argument

        TestStringBuilder(builder => builder.Append("x"), builder => builder.Append('x'), values);

        TestStringBuilder(
            builder => builder.AppendJoin("x", (IEnumerable<int>)[1, 2, 3]),
            builder => builder.AppendJoin('x', (IEnumerable<int>)[1, 2, 3]),
            values);
        TestStringBuilder(
            builder => builder.AppendJoin("x", (string?[])["one", "two", "three", null]),
            builder => builder.AppendJoin('x', (string?[])["one", "two", "three", null]),
            values);
        TestStringBuilder(
            builder => builder.AppendJoin("x", (object?[])[1, "two", true, null]),
            builder => builder.AppendJoin('x', (object?[])[1, "two", true, null]),
            values);
        TestStringBuilder(
            builder => builder.AppendJoin("x", (ReadOnlySpan<string?>)["one", "two", "three", null]),
            builder => builder.AppendJoin('x', (ReadOnlySpan<string?>)["one", "two", "three", null]),
            values);
        TestStringBuilder(
            builder => builder.AppendJoin("x", (ReadOnlySpan<object?>)[1, "two", true, null]),
            builder => builder.AppendJoin('x', (ReadOnlySpan<object?>)[1, "two", true, null]),
            values);

        TestStringBuilder(builder => builder.Insert(1, "x"), builder => builder.Insert(1, 'x'), [..values.Except([""])]);
        TestStringBuilder(builder => builder.Insert(1, "x", 1), builder => builder.Insert(1, 'x'), [..values.Except([""])]);

        // other argument range

        TestStringBuilder(builder => builder.Replace("c", "x"), builder => builder.Replace('c', 'x'), values);
        TestStringBuilder(builder => builder.Replace("c", "x", 1, 3), builder => builder.Replace('c', 'x', 1, 3), [..values.Except([""])]);

        DoNamedTest();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Math()
    {
        var decimalValues = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };
        var doubleValues = new[]
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

        // redundant argument

        Test(n => System.Math.Round(n, 0), n => System.Math.Round(n), decimalValues);
        Test(n => System.Math.Round(n, MidpointRounding.ToEven), n => System.Math.Round(n), decimalValues);
        Test((n, mode) => System.Math.Round(n, 0, mode), (n, mode) => System.Math.Round(n, mode), decimalValues, roundings);
        Test(
            (n, decimals) => System.Math.Round(n, decimals, MidpointRounding.ToEven),
            (n, decimals) => System.Math.Round(n, decimals),
            decimalValues,
            digitsValues);

        Test(n => System.Math.Round(n, 0), n => System.Math.Round(n), doubleValues);
        Test(n => System.Math.Round(n, MidpointRounding.ToEven), n => System.Math.Round(n), doubleValues);
        Test((n, mode) => System.Math.Round(n, 0, mode), (n, mode) => System.Math.Round(n, mode), doubleValues, roundings);
        Test(
            (n, digits) => System.Math.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => System.Math.Round(n, digits),
            doubleValues,
            digitsValues);

        DoNamedTest();
    }

    [Test]
    [TestNetCore20]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void MathF()
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

        // redundant argument

        Test(n => mathF.Round(n, 0), n => mathF.Round(n), values);
        Test(n => mathF.Round(n, MidpointRounding.ToEven), n => mathF.Round(n), values);
        Test((n, mode) => mathF.Round(n, 0, mode), (n, mode) => mathF.Round(n, mode), values, roundings);
        Test((n, digits) => mathF.Round(n, digits, MidpointRounding.ToEven), (n, digits) => mathF.Round(n, digits), values, digitsValues);

        DoNamedTest();
    }

    [Test]
    [TestNet60]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void Random()
    {
        // redundant argument

        TestRandom(random => random.Next(int.MaxValue), random => random.Next());
        TestRandom(random => random.Next(0, 10), random => random.Next(10));

        TestRandom(random => random.NextInt64(long.MaxValue), random => random.NextInt64());
        TestRandom(random => random.NextInt64(0, 10), random => random.NextInt64(10));

        DoNamedTest();
    }
}