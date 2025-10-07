using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

using int128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Int128;
using uint128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.UInt128;
using half = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Half;

[TestFixture]
public sealed class ArgumentAnalyzerTests : CSharpHighlightingTestBase
{
    readonly IFormatProvider?[] formatProviders = [null, CultureInfo.InvariantCulture, new CultureInfo("en-US"), new CultureInfo("de-DE")];

    protected override string RelativeTestDataPath => @"Analyzers\Argument";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantArgumentHint or RedundantArgumentRangeHint || highlighting.IsError();

    static void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    static void Test<T, R>(Func<T, R> expected, Func<T, R> actual, T[] values)
    {
        foreach (var value in values)
        {
            Assert.AreEqual(expected(value), actual(value), $"with value: {value}");
        }
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test<T, O, R>(FuncWithOut<T, O, R> expected, FuncWithOut<T, O, R> actual, T[] values)
    {
        foreach (var value in values)
        {
            Assert.AreEqual(expected(value, out var expectedResult), actual(value, out var actualResult), $"with value: {value}");
            Assert.AreEqual(expectedResult, actualResult, $"with value: {value}");
        }
    }

    delegate R FuncWithOut<in T, in U, O, out R>(T arg1, U arg2, out O arg3);

    static void Test<T, U, O, R>(FuncWithOut<T, U, O, R> expected, FuncWithOut<T, U, O, R> actual, T[] xValues, U[] yValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                Assert.AreEqual(expected(x, y, out var expectedResult), actual(x, y, out var actualResult), $"with values: {x}, {y}");
                Assert.AreEqual(expectedResult, actualResult, $"with values: {x}, {y}");
            }
        }
    }

    static void Test<T, U, R>(Func<T, U, R> expected, Func<T, U, R> actual, T[] xValues, U[] yValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                Assert.AreEqual(expected(x, y), actual(x, y), $"with values: {x}, {y}");
            }
        }
    }

    delegate R FuncWithOut<in T, in U, in V, O1, out R>(T arg1, U arg2, V arg3, out O1 arg4);

    static void Test<T, U, V, O, R>(FuncWithOut<T, U, V, O, R> expected, FuncWithOut<T, U, V, O, R> actual, T[] xValues, U[] yValues, V[] zValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                foreach (var z in zValues)
                {
                    Assert.AreEqual(expected(x, y, z, out var expectedResult), actual(x, y, z, out var actualResult), $"with values: {x}, {y}, {z}");
                    Assert.AreEqual(expectedResult, actualResult, $"with values: {x}, {y}, {z}");
                }
            }
        }
    }

    static void Test<T, U, V, R>(Func<T, U, V, R> expected, Func<T, U, V, R> actual, T[] xValues, U[] yValues, V[] zValues)
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

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestByte()
    {
        var values = new byte[] { 0, 1, 2, byte.MaxValue };
        var styles = new[] { NumberStyles.None, NumberStyles.Integer };

        // redundant argument

        Test(n => byte.Parse($"{n}", NumberStyles.Integer), n => byte.Parse($"{n}"), values);
        Test(n => byte.Parse($"{n}", null), n => byte.Parse($"{n}"), values);
        Test(
            (n, provider) => byte.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => byte.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => byte.Parse($"{n}", style, null), (n, style) => byte.Parse($"{n}", style), values, styles);
        Test(n => MissingByteMethods.Parse($"{n}".AsSpan(), null), n => MissingByteMethods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingByteMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingByteMethods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}", null, out result),
            (byte n, out byte result) => byte.TryParse($"{n}", out result),
            values);
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), null, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (byte n, IFormatProvider? provider, out byte result) => byte.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (byte n, IFormatProvider? provider, out byte result)
                => MissingByteMethods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestSByte()
    {
        var values = new sbyte[] { 0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign, NumberStyles.Integer };

        // redundant argument

        Test(n => sbyte.Parse($"{n}", NumberStyles.Integer), n => sbyte.Parse($"{n}"), values);
        Test(n => sbyte.Parse($"{n}", null), n => sbyte.Parse($"{n}"), values);
        Test(
            (n, provider) => sbyte.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => sbyte.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => sbyte.Parse($"{n}", style, null), (n, style) => sbyte.Parse($"{n}", style), values, styles);
        Test(n => MissingSByteMethods.Parse($"{n}".AsSpan(), null), n => MissingSByteMethods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingSByteMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingSByteMethods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}", null, out result),
            (sbyte n, out sbyte result) => sbyte.TryParse($"{n}", out result),
            values);
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), null, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (sbyte n, IFormatProvider? provider, out sbyte result) => sbyte.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (sbyte n, IFormatProvider? provider, out sbyte result)
                => MissingSByteMethods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestInt16()
    {
        var values = new short[] { 0, 1, 2, -1, -2, short.MaxValue, short.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign, NumberStyles.Integer };

        // redundant argument

        Test(n => short.Parse($"{n}", NumberStyles.Integer), n => short.Parse($"{n}"), values);
        Test(n => short.Parse($"{n}", null), n => short.Parse($"{n}"), values);
        Test(
            (n, provider) => short.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => short.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => short.Parse($"{n}", style, null), (n, style) => short.Parse($"{n}", style), values, styles);
        Test(n => MissingInt16Methods.Parse($"{n}".AsSpan(), null), n => MissingInt16Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingInt16Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingInt16Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}", null, out result),
            (short n, out short result) => short.TryParse($"{n}", out result),
            values);
        Test(
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}".AsSpan(), null, out result),
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (short n, IFormatProvider? provider, out short result) => short.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (short n, IFormatProvider? provider, out short result)
                => MissingInt16Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestUInt16()
    {
        var values = new ushort[] { 0, 1, 2, ushort.MaxValue };
        var styles = new[] { NumberStyles.None, NumberStyles.Integer };

        // redundant argument

        Test(n => ushort.Parse($"{n}", NumberStyles.Integer), n => ushort.Parse($"{n}"), values);
        Test(n => ushort.Parse($"{n}", null), n => ushort.Parse($"{n}"), values);
        Test(
            (n, provider) => ushort.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => ushort.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => ushort.Parse($"{n}", style, null), (n, style) => ushort.Parse($"{n}", style), values, styles);
        Test(n => MissingUInt16Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt16Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingUInt16Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingUInt16Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}", null, out result),
            (ushort n, out ushort result) => ushort.TryParse($"{n}", out result),
            values);
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (ushort n, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (ushort n, IFormatProvider? provider, out ushort result) => ushort.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (ushort n, IFormatProvider? provider, out ushort result)
                => MissingUInt16Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (ushort n, IFormatProvider? provider, out ushort result) => MissingUInt16Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestInt32()
    {
        var values = new[] { 0, 1, 2, -1, -2, int.MaxValue, int.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign, NumberStyles.Integer };

        // redundant argument

        Test(n => int.Parse($"{n}", NumberStyles.Integer), n => int.Parse($"{n}"), values);
        Test(n => int.Parse($"{n}", null), n => int.Parse($"{n}"), values);
        Test(
            (n, provider) => int.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => int.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => int.Parse($"{n}", style, null), (n, style) => int.Parse($"{n}", style), values, styles);
        Test(n => MissingInt32Methods.Parse($"{n}".AsSpan(), null), n => MissingInt32Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingInt32Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingInt32Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}", null, out result),
            (int n, out int result) => int.TryParse($"{n}", out result),
            values);
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), null, out result),
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (int n, out int result) => MissingInt32Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (int n, IFormatProvider? provider, out int result) => int.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (int n, IFormatProvider? provider, out int result)
                => MissingInt32Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (int n, IFormatProvider? provider, out int result) => MissingInt32Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestUInt32()
    {
        var values = new uint[] { 0, 1, 2, uint.MaxValue };
        var styles = new[] { NumberStyles.None, NumberStyles.Integer };

        // redundant argument

        Test(n => uint.Parse($"{n}", NumberStyles.Integer), n => uint.Parse($"{n}"), values);
        Test(n => uint.Parse($"{n}", null), n => uint.Parse($"{n}"), values);
        Test(
            (n, provider) => uint.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => uint.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => uint.Parse($"{n}", style, null), (n, style) => uint.Parse($"{n}", style), values, styles);
        Test(n => MissingUInt32Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt32Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingUInt32Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingUInt32Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}", null, out result),
            (uint n, out uint result) => uint.TryParse($"{n}", out result),
            values);
        Test(
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsSpan(), null, out result),
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (uint n, IFormatProvider? provider, out uint result) => uint.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (uint n, IFormatProvider? provider, out uint result)
                => MissingUInt32Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestInt64()
    {
        var values = new[] { 0, 1, 2, -1, -2, long.MaxValue, long.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign, NumberStyles.Integer };

        // redundant argument

        Test(n => long.Parse($"{n}", NumberStyles.Integer), n => long.Parse($"{n}"), values);
        Test(n => long.Parse($"{n}", null), n => long.Parse($"{n}"), values);
        Test(
            (n, provider) => long.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => long.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => long.Parse($"{n}", style, null), (n, style) => long.Parse($"{n}", style), values, styles);
        Test(n => MissingInt64Methods.Parse($"{n}".AsSpan(), null), n => MissingInt64Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingInt64Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingInt64Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}", null, out result),
            (long n, out long result) => long.TryParse($"{n}", out result),
            values);
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), null, out result),
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (long n, out long result) => MissingInt64Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (long n, IFormatProvider? provider, out long result) => long.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (long n, IFormatProvider? provider, out long result)
                => MissingInt64Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (long n, IFormatProvider? provider, out long result) => MissingInt64Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestUInt64()
    {
        var values = new ulong[] { 0, 1, 2, ulong.MaxValue };
        var styles = new[] { NumberStyles.None, NumberStyles.Integer };

        // redundant argument

        Test(n => ulong.Parse($"{n}", NumberStyles.Integer), n => ulong.Parse($"{n}"), values);
        Test(n => ulong.Parse($"{n}", null), n => ulong.Parse($"{n}"), values);
        Test(
            (n, provider) => ulong.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => ulong.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => ulong.Parse($"{n}", style, null), (n, style) => ulong.Parse($"{n}", style), values, styles);
        Test(n => MissingUInt64Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt64Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingUInt64Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingUInt64Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}", null, out result),
            (ulong n, out ulong result) => ulong.TryParse($"{n}", out result),
            values);
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), null, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (ulong n, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (ulong n, IFormatProvider? provider, out ulong result) => ulong.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (ulong n, IFormatProvider? provider, out ulong result)
                => MissingUInt64Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (ulong n, IFormatProvider? provider, out ulong result) => MissingUInt64Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestInt128()
    {
        var values = new[] { 0, 1, 2, -1, -2, int128.MaxValue, int128.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign, NumberStyles.Integer };

        // redundant argument

        Test(n => int128.Parse($"{n}", NumberStyles.Integer), n => int128.Parse($"{n}"), values);
        Test(n => int128.Parse($"{n}", null), n => int128.Parse($"{n}"), values);
        Test(
            (n, provider) => int128.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => int128.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => int128.Parse($"{n}", style, null), (n, style) => int128.Parse($"{n}", style), values, styles);
        Test(n => MissingInt128Methods.Parse($"{n}".AsSpan(), null), n => MissingInt128Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingInt128Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingInt128Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (int128 n, out int128 result) => int128.TryParse($"{n}", null, out result),
            (int128 n, out int128 result) => int128.TryParse($"{n}", out result),
            values);
        Test(
            (int128 n, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), null, out result),
            (int128 n, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (int128 n, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (int128 n, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (int128 n, IFormatProvider? provider, out int128 result) => int128.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (int128 n, IFormatProvider? provider, out int128 result) => int128.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (int128 n, IFormatProvider? provider, out int128 result)
                => MissingInt128Methods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (int128 n, IFormatProvider? provider, out int128 result) => MissingInt128Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestUInt128()
    {
        var values = new[] { 0, 1, 2, uint128.MaxValue };
        var styles = new[] { NumberStyles.None, NumberStyles.Integer };

        // redundant argument

        Test(n => uint128.Parse($"{n}", NumberStyles.Integer), n => uint128.Parse($"{n}"), values);
        Test(n => uint128.Parse($"{n}", null), n => uint128.Parse($"{n}"), values);
        Test(
            (n, provider) => uint128.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => uint128.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => uint128.Parse($"{n}", style, null), (n, style) => uint128.Parse($"{n}", style), values, styles);
        Test(n => MissingUInt128Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt128Methods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingUInt128Methods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingUInt128Methods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", null, out result),
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", out result),
            values);
        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), null, out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (uint128 n, IFormatProvider? provider, out uint128 result) => uint128.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (uint128 n, IFormatProvider? provider, out uint128 result) => uint128.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestIntPtr()
    {
        var values = new[] { (nint)0, 1, 2, -1, -2 };
        var styles = new[] { NumberStyles.AllowLeadingSign, NumberStyles.Integer };

        // redundant argument

        Test(n => MissingIntPtrMethods.Parse($"{n}", NumberStyles.Integer), n => MissingIntPtrMethods.Parse($"{n}"), values);
        Test(n => MissingIntPtrMethods.Parse($"{n}", null), n => MissingIntPtrMethods.Parse($"{n}"), values);
        Test(
            (n, provider) => MissingIntPtrMethods.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => MissingIntPtrMethods.Parse($"{n}", provider),
            values,
            formatProviders);
        Test((n, style) => MissingIntPtrMethods.Parse($"{n}", style, null), (n, style) => MissingIntPtrMethods.Parse($"{n}", style), values, styles);
        Test(n => MissingIntPtrMethods.Parse($"{n}".AsSpan(), null), n => MissingIntPtrMethods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingIntPtrMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingIntPtrMethods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}", out result),
            values);
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (nint n, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (nint n, IFormatProvider? provider, out nint result)
                => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (nint n, IFormatProvider? provider, out nint result) => MissingIntPtrMethods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestUIntPtr()
    {
        var values = new nuint[] { 0, 1, 2 };
        var styles = new[] { NumberStyles.None, NumberStyles.Integer };

        // redundant argument

        Test(n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.Integer), n => MissingUIntPtrMethods.Parse($"{n}"), values);
        Test(n => MissingUIntPtrMethods.Parse($"{n}", null), n => MissingUIntPtrMethods.Parse($"{n}"), values);
        Test(
            (n, provider) => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => MissingUIntPtrMethods.Parse($"{n}", provider),
            values,
            formatProviders);
        Test(
            (n, style) => MissingUIntPtrMethods.Parse($"{n}", style, null),
            (n, style) => MissingUIntPtrMethods.Parse($"{n}", style),
            values,
            styles);
        Test(n => MissingUIntPtrMethods.Parse($"{n}".AsSpan(), null), n => MissingUIntPtrMethods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingUIntPtrMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingUIntPtrMethods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", out result),
            values);
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (nuint n, IFormatProvider? provider, out nuint result)
                => MissingUIntPtrMethods.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", provider, out result),
            values,
            formatProviders);
        Test(
            (nuint n, IFormatProvider? provider, out nuint result)
                => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse(
                $"{n}".AsUtf8Bytes(),
                NumberStyles.Integer,
                provider,
                out result),
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestDecimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };
        var styles = new[] { NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberStyles.Number };
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var decimalsValues = new[] { 0, 1, 2 };

        // redundant argument

        Test(n => decimal.Parse($"{n}", NumberStyles.Number), n => decimal.Parse($"{n}"), values);
        Test(n => decimal.Parse($"{n}", null), n => decimal.Parse($"{n}"), values);
        Test(
            (n, provider) => decimal.Parse(n.ToString(provider), NumberStyles.Number, provider),
            (n, provider) => decimal.Parse(n.ToString(provider), provider),
            values,
            formatProviders);
        Test((n, style) => decimal.Parse($"{n}", style, null), (n, style) => decimal.Parse($"{n}", style), values, styles);
        Test(n => MissingDecimalMethods.Parse($"{n}".AsSpan(), null), n => MissingDecimalMethods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingDecimalMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingDecimalMethods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}", null, out result),
            (decimal n, out decimal result) => decimal.TryParse($"{n}", out result),
            values);
        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsSpan(), null, out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (decimal n, out decimal result) => MissingDecimalMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (decimal n, IFormatProvider? provider, out decimal result)
                => decimal.TryParse(n.ToString(provider), NumberStyles.Number, provider, out result),
            (decimal n, IFormatProvider? provider, out decimal result) => MissingDecimalMethods.TryParse(n.ToString(provider), provider, out result),
            values,
            formatProviders);
        Test(
            (decimal n, IFormatProvider? provider, out decimal result) => MissingDecimalMethods.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Number,
                provider,
                out result),
            (decimal n, IFormatProvider? provider, out decimal result)
                => MissingDecimalMethods.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (decimal n, IFormatProvider? provider, out decimal result) => MissingDecimalMethods.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Number,
                provider,
                out result),
            (decimal n, IFormatProvider? provider, out decimal result)
                => MissingDecimalMethods.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        Test(n => decimal.Round(n, 0), n => decimal.Round(n), values);
        Test(n => decimal.Round(n, MidpointRounding.ToEven), n => decimal.Round(n), values);
        Test((n, mode) => decimal.Round(n, 0, mode), (n, mode) => decimal.Round(n, mode), values, roundings);
        Test(
            (n, decimals) => decimal.Round(n, decimals, MidpointRounding.ToEven),
            (n, decimals) => decimal.Round(n, decimals),
            values,
            decimalsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
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
        double[] valuesForParsing = [..values.Except([double.MinValue, double.MaxValue]), float.MinValue, float.MaxValue];
        var styles = new[]
        {
            NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
            NumberStyles.Float | NumberStyles.AllowThousands,
        };
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        // redundant argument

        Test(n => double.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => double.Parse($"{n}"), valuesForParsing);
        Test(n => double.Parse($"{n}", null), n => double.Parse($"{n}"), valuesForParsing);
        Test(
            (n, provider) => double.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => double.Parse(n.ToString(provider), provider),
            valuesForParsing,
            formatProviders);
        Test((n, style) => double.Parse($"{n}", style, null), (n, style) => double.Parse($"{n}", style), valuesForParsing, styles);
        Test(n => MissingDoubleMethods.Parse($"{n}".AsSpan(), null), n => MissingDoubleMethods.Parse($"{n}".AsSpan()), valuesForParsing);
        Test(n => MissingDoubleMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingDoubleMethods.Parse($"{n}".AsUtf8Bytes()), valuesForParsing);

        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}", null, out result),
            (double n, out double result) => double.TryParse($"{n}", out result),
            valuesForParsing);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsSpan(), out result),
            valuesForParsing);
        Test(
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (double n, out double result) => MissingDoubleMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            valuesForParsing);
        Test(
            (double n, IFormatProvider? provider, out double result) => double.TryParse(
                n.ToString(provider),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse(n.ToString(provider), provider, out result),
            valuesForParsing,
            formatProviders);
        Test(
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (double n, IFormatProvider? provider, out double result)
                => MissingDoubleMethods.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            valuesForParsing,
            formatProviders);
        Test(
            (double n, IFormatProvider? provider, out double result) => MissingDoubleMethods.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (double n, IFormatProvider? provider, out double result)
                => MissingDoubleMethods.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            valuesForParsing,
            formatProviders);

        Test(n => MissingDoubleMethods.Round(n, 0), n => MissingDoubleMethods.Round(n), values);
        Test(n => MissingDoubleMethods.Round(n, MidpointRounding.ToEven), n => MissingDoubleMethods.Round(n), values);
        Test((n, mode) => MissingDoubleMethods.Round(n, 0, mode), (n, mode) => MissingDoubleMethods.Round(n, mode), values, roundings);
        Test(
            (n, digits) => MissingDoubleMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingDoubleMethods.Round(n, digits),
            values,
            digitsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
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
        float[] valuesForParsing = [.. values.Except([float.MinValue, float.MaxValue])];
        var styles = new[]
        {
            NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
            NumberStyles.Float | NumberStyles.AllowThousands,
        };
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        // redundant argument

        Test(n => float.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => float.Parse($"{n}"), valuesForParsing);
        Test(n => float.Parse($"{n}", null), n => float.Parse($"{n}"), valuesForParsing);
        Test(
            (n, provider) => float.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => float.Parse(n.ToString(provider), provider),
            valuesForParsing,
            formatProviders);
        Test((n, style) => float.Parse($"{n}", style, null), (n, style) => float.Parse($"{n}", style), valuesForParsing, styles);
        Test(n => MissingSingleMethods.Parse($"{n}".AsSpan(), null), n => MissingSingleMethods.Parse($"{n}".AsSpan()), valuesForParsing);
        Test(n => MissingSingleMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingSingleMethods.Parse($"{n}".AsUtf8Bytes()), valuesForParsing);

        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}", null, out result),
            (float n, out float result) => float.TryParse($"{n}", out result),
            valuesForParsing);
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsSpan(), out result),
            valuesForParsing);
        Test(
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (float n, out float result) => MissingSingleMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            valuesForParsing);
        Test(
            (float n, IFormatProvider? provider, out float result) => float.TryParse(
                n.ToString(provider),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse(n.ToString(provider), provider, out result),
            valuesForParsing,
            formatProviders);
        Test(
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (float n, IFormatProvider? provider, out float result)
                => MissingSingleMethods.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            valuesForParsing,
            formatProviders);
        Test(
            (float n, IFormatProvider? provider, out float result) => MissingSingleMethods.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (float n, IFormatProvider? provider, out float result)
                => MissingSingleMethods.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            valuesForParsing,
            formatProviders);

        Test(n => MissingSingleMethods.Round(n, 0), n => MissingSingleMethods.Round(n), values);
        Test(n => MissingSingleMethods.Round(n, MidpointRounding.ToEven), n => MissingSingleMethods.Round(n), values);
        Test((n, mode) => MissingSingleMethods.Round(n, 0, mode), (n, mode) => MissingSingleMethods.Round(n, mode), values, roundings);
        Test(
            (n, digits) => MissingSingleMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingSingleMethods.Round(n, digits),
            values,
            digitsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
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
        var styles = new[]
        {
            NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
            NumberStyles.Float | NumberStyles.AllowThousands,
        };
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        // redundant argument

        Test(n => half.Parse($"{n}", NumberStyles.Float | NumberStyles.AllowThousands), n => half.Parse($"{n}"), values);
        Test(n => half.Parse($"{n}", null), n => half.Parse($"{n}"), values);
        Test(
            (n, provider) => half.Parse(n.ToString(provider), NumberStyles.Float | NumberStyles.AllowThousands, provider),
            (n, provider) => half.Parse(n.ToString(provider), provider),
            values,
            formatProviders);
        Test((n, style) => half.Parse($"{n}", style, null), (n, style) => half.Parse($"{n}", style), values, styles);
        Test(n => MissingHalfMethods.Parse($"{n}".AsSpan(), null), n => MissingHalfMethods.Parse($"{n}".AsSpan()), values);
        Test(n => MissingHalfMethods.Parse($"{n}".AsUtf8Bytes(), null), n => MissingHalfMethods.Parse($"{n}".AsUtf8Bytes()), values);

        Test(
            (half n, out half result) => half.TryParse($"{n}", null, out result),
            (half n, out half result) => half.TryParse($"{n}", out result),
            values);
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsSpan(), out result),
            values);
        Test(
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsUtf8Bytes(), null, out result),
            (half n, out half result) => MissingHalfMethods.TryParse($"{n}".AsUtf8Bytes(), out result),
            values);
        Test(
            (half n, IFormatProvider? provider, out half result) => half.TryParse(
                n.ToString(provider),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (half n, IFormatProvider? provider, out half result) => half.TryParse(n.ToString(provider), provider, out result),
            values,
            formatProviders);
        Test(
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse(
                n.ToString(provider).AsSpan(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse(n.ToString(provider).AsSpan(), provider, out result),
            values,
            formatProviders);
        Test(
            (half n, IFormatProvider? provider, out half result) => MissingHalfMethods.TryParse(
                n.ToString(provider).AsUtf8Bytes(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                provider,
                out result),
            (half n, IFormatProvider? provider, out half result)
                => MissingHalfMethods.TryParse(n.ToString(provider).AsUtf8Bytes(), provider, out result),
            values,
            formatProviders);

        Test(n => MissingHalfMethods.Round(n, 0), n => MissingHalfMethods.Round(n), values);
        Test(n => MissingHalfMethods.Round(n, MidpointRounding.ToEven), n => MissingHalfMethods.Round(n), values);
        Test((n, mode) => MissingHalfMethods.Round(n, 0, mode), (n, mode) => MissingHalfMethods.Round(n, mode), values, roundings);
        Test(
            (n, digits) => MissingHalfMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingHalfMethods.Round(n, digits),
            values,
            digitsValues);

        DoNamedTest2();
    }

    // [Test]
    public void TestBoolean()
    {
        var values = new[] { true, false };

        DoNamedTest2();
    }

    // [Test]
    public void TestChar()
    {
        var values = new[] { 'a', 'A', '1', ' ', 'ä', 'ß', '€', char.MinValue, char.MaxValue };

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
    [TestNet60]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestEnum()
    {
        var enumValues = new[] { SampleEnum.Red, (SampleEnum)1, (SampleEnum)10 };
        var flagValues = new[] { SampleFlags.Red, SampleFlags.Red | SampleFlags.Blue, (SampleFlags)3, (SampleFlags)0, (SampleFlags)9 };

        // redundant argument

        Test(e => MissingEnumMethods.Parse<SampleEnum>(e.ToString(), false), e => MissingEnumMethods.Parse<SampleEnum>(e.ToString()), enumValues);
        Test(
            e => MissingEnumMethods.Parse<SampleEnum>(e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse<SampleEnum>(e.ToString().AsSpan()),
            enumValues);
        Test(e => Enum.Parse(typeof(SampleEnum), e.ToString(), false), e => Enum.Parse(typeof(SampleEnum), e.ToString()), enumValues);
        Test(
            e => MissingEnumMethods.Parse(typeof(SampleEnum), e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse(typeof(SampleEnum), e.ToString().AsSpan()),
            enumValues);
        Test(e => MissingEnumMethods.Parse<SampleFlags>(e.ToString(), false), e => MissingEnumMethods.Parse<SampleFlags>(e.ToString()), flagValues);
        Test(
            e => MissingEnumMethods.Parse<SampleFlags>(e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse<SampleFlags>(e.ToString().AsSpan()),
            flagValues);
        Test(e => Enum.Parse(typeof(SampleFlags), e.ToString(), false), e => Enum.Parse(typeof(SampleFlags), e.ToString()), flagValues);
        Test(
            e => MissingEnumMethods.Parse(typeof(SampleFlags), e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse(typeof(SampleFlags), e.ToString().AsSpan()),
            flagValues);

        Test(
            (SampleEnum value, out SampleEnum result) => Enum.TryParse($"{value}", false, out result),
            (SampleEnum value, out SampleEnum result) => Enum.TryParse($"{value}", out result),
            enumValues);
        Test(
            (SampleEnum value, out SampleEnum result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), false, out result),
            (SampleEnum value, out SampleEnum result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), out result),
            enumValues);
        Test(
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}", false, out result),
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}", out result),
            enumValues);
        Test(
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}".AsSpan(), false, out result),
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}".AsSpan(), out result),
            enumValues);
        Test(
            (SampleFlags value, out SampleFlags result) => Enum.TryParse($"{value}", false, out result),
            (SampleFlags value, out SampleFlags result) => Enum.TryParse($"{value}", out result),
            flagValues);
        Test(
            (SampleFlags value, out SampleFlags result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), false, out result),
            (SampleFlags value, out SampleFlags result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), out result),
            flagValues);
        Test(
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}", false, out result),
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}", out result),
            flagValues);
        Test(
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}".AsSpan(), false, out result),
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}".AsSpan(), out result),
            flagValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestGuid()
    {
        var values = new[] { Guid.Empty, new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]) };

        // redundant argument

        Test(
            (guid, provider) => MissingGuidMethods.Parse(guid.ToString(), provider),
            (guid, _) => Guid.Parse(guid.ToString()),
            values,
            formatProviders);
        Test(
            (guid, provider) => MissingGuidMethods.Parse(guid.ToString().AsSpan(), provider),
            (guid, _) => MissingGuidMethods.Parse(guid.ToString().AsSpan()),
            values,
            formatProviders);

        Test(
            (Guid value, IFormatProvider? provider, out Guid result) => MissingGuidMethods.TryParse($"{value}", provider, out result),
            (Guid value, IFormatProvider? _, out Guid result) => Guid.TryParse($"{value}", out result),
            values,
            formatProviders);
        Test(
            (Guid value, IFormatProvider? provider, out Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), provider, out result),
            (Guid value, IFormatProvider? _, out Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), out result),
            values,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
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
        var formatSpecifiers = new[] { "c", "t", "T", "g", "G" };

        // redundant argument

        Test(() => new TimeSpan(0, 1, 2, 3), () => new TimeSpan(1, 2, 3));
        Test(() => new TimeSpan(1, 2, 3, 4, 0), () => new TimeSpan(1, 2, 3, 4));
        Test(() => MissingTimeSpanMembers._Ctor(1, 2, 3, 4, 5, 0), () => new TimeSpan(1, 2, 3, 4, 5));

        Test(timeSpan => TimeSpan.Parse($"{timeSpan}", null), timeSpan => TimeSpan.Parse($"{timeSpan}"), values);

        Test(
            (timeSpan, format, formatProvider) => TimeSpan.ParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                TimeSpanStyles.None),
            (timeSpan, format, formatProvider) => TimeSpan.ParseExact(timeSpan.ToString(format, formatProvider), format, formatProvider),
            values,
            formatSpecifiers,
            formatProviders);
        Test(
            (timeSpan, formatProvider) => TimeSpan.ParseExact($"{timeSpan}", formatSpecifiers, formatProvider, TimeSpanStyles.None),
            (timeSpan, formatProvider) => TimeSpan.ParseExact($"{timeSpan}", formatSpecifiers, formatProvider),
            values,
            formatProviders);

        Test(
            (TimeSpan timeSpan, out TimeSpan result) => TimeSpan.TryParse($"{timeSpan}", null, out result),
            (TimeSpan timeSpan, out TimeSpan result) => TimeSpan.TryParse($"{timeSpan}", out result),
            values);
        Test(
            (TimeSpan timeSpan, out TimeSpan result) => MissingTimeSpanMembers.TryParse($"{timeSpan}".AsSpan(), null, out result),
            (TimeSpan timeSpan, out TimeSpan result) => MissingTimeSpanMembers.TryParse($"{timeSpan}".AsSpan(), out result),
            values);

        Test(
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => TimeSpan.TryParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => TimeSpan.TryParseExact(
                timeSpan.ToString(format, formatProvider),
                format,
                formatProvider,
                out result),
            values,
            formatSpecifiers,
            formatProviders);
        Test(
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString(format, formatProvider).AsSpan(),
                format.AsSpan(),
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString(format, formatProvider).AsSpan(),
                format.AsSpan(),
                formatProvider,
                out result),
            values,
            formatSpecifiers,
            formatProviders);
        Test(
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => TimeSpan.TryParseExact(
                timeSpan.ToString(format, formatProvider),
                formatSpecifiers,
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => TimeSpan.TryParseExact(
                timeSpan.ToString(format, formatProvider),
                formatSpecifiers,
                formatProvider,
                out result),
            values,
            formatSpecifiers,
            formatProviders);
        Test(
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString(format, formatProvider).AsSpan(),
                formatSpecifiers,
                formatProvider,
                TimeSpanStyles.None,
                out result),
            (TimeSpan timeSpan, string format, IFormatProvider? formatProvider, out TimeSpan result) => MissingTimeSpanMembers.TryParseExact(
                timeSpan.ToString(format, formatProvider).AsSpan(),
                formatSpecifiers,
                formatProvider,
                out result),
            values,
            formatSpecifiers,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantArgumentRange")]
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
        var calendars = new Calendar[] { new GregorianCalendar(), new JulianCalendar(), new JapaneseCalendar() };
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" };

        // redundant argument

        Test(ticks => new DateTime(ticks, DateTimeKind.Unspecified), ticks => new DateTime(ticks), [0, 1, 638_882_119_800_000_000]);
        Test(
            dateTime => MissingDateTimeMembers._Ctor(DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime), DateTimeKind.Unspecified),
            dateTime => MissingDateTimeMembers._Ctor(DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime)),
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
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
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
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            calendars);
        Test(
            dateTime => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond()),
            dateTime => new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond),
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])]);
        Test(
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
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
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            calendars);
        Test(
            dateTime => MissingDateTimeMembers._Ctor(
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
            dateTime => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond(),
                DateTimeKind.Unspecified),
            dateTime => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond()),
            values);
        Test(
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
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
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            calendars);
        Test(
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond(),
                calendar,
                DateTimeKind.Unspecified),
            (dateTime, calendar) => MissingDateTimeMembers._Ctor(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond,
                dateTime.GetMicrosecond(),
                calendar),
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            calendars);

        Test(dateTime => dateTime.GetDateTimeFormats(null), dateTime => dateTime.GetDateTimeFormats(), values);
        Test(
            (dateTime, format) => dateTime.GetDateTimeFormats(format, null),
            (dateTime, format) => dateTime.GetDateTimeFormats(format),
            values,
            [..from f in formats select f[0]]);

        Test(dateTime => DateTime.Parse($"{dateTime}", null), dateTime => DateTime.Parse($"{dateTime}"), values);
        Test(
            dateTime => MissingDateTimeMembers.Parse($"{dateTime}".AsSpan(), null),
            dateTime => MissingDateTimeMembers.Parse($"{dateTime}".AsSpan()),
            values);
        Test(
            (dateTime, provider) => DateTime.Parse(dateTime.ToString(provider), provider, DateTimeStyles.None),
            (dateTime, provider) => DateTime.Parse(dateTime.ToString(provider), provider),
            values,
            formatProviders);

        Test(
            (dateTime, format, provider) => DateTime.ParseExact(dateTime.ToString(format, provider), format, provider, DateTimeStyles.None),
            (dateTime, format, provider) => DateTime.ParseExact(dateTime.ToString(format, provider), format, provider),
            values,
            formats,
            formatProviders);

        Test(
            (DateTime dateTime, out DateTime result) => MissingDateTimeMembers.TryParse($"{dateTime}", null, out result),
            (DateTime dateTime, out DateTime result) => DateTime.TryParse($"{dateTime}", out result),
            values);
        Test(
            (DateTime dateTime, out DateTime result) => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), null, out result),
            (DateTime dateTime, out DateTime result) => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), out result),
            values);
        Test(
            (DateTime dateTime, IFormatProvider? provider, out DateTime result)
                => DateTime.TryParse($"{dateTime}", provider, DateTimeStyles.None, out result),
            (DateTime dateTime, IFormatProvider? provider, out DateTime result)
                => MissingDateTimeMembers.TryParse($"{dateTime}", provider, out result),
            values,
            formatProviders);
        Test(
            (DateTime dateTime, IFormatProvider? provider, out DateTime result) => MissingDateTimeMembers.TryParse(
                $"{dateTime}".AsSpan(),
                provider,
                DateTimeStyles.None,
                out result),
            (DateTime dateTime, IFormatProvider? provider, out DateTime result)
                => MissingDateTimeMembers.TryParse($"{dateTime}".AsSpan(), provider, out result),
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
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            calendars);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "RedundantArgument")]
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
        var calendars = new Calendar[] { new GregorianCalendar(), new JulianCalendar(), new JapaneseCalendar() };
        var formats = new[] { "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" };

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
            dateTimeOffset => MissingDateTimeOffsetMembers._Ctor(
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
            (dateTimeOffset, calendar) => MissingDateTimeOffsetMembers._Ctor(
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
            [..values.Except([DateTimeOffset.MinValue, DateTimeOffset.MaxValue])],
            calendars);

        Test(dateTimeOffset => DateTimeOffset.Parse($"{dateTimeOffset}", null), dateTimeOffset => DateTimeOffset.Parse($"{dateTimeOffset}"), values);
        Test(
            dateTimeOffset => MissingDateTimeOffsetMembers.Parse($"{dateTimeOffset}".AsSpan(), null),
            dateTimeOffset => MissingDateTimeOffsetMembers.Parse($"{dateTimeOffset}".AsSpan()),
            values);
        Test(
            (dateTimeOffset, formatProvider) => DateTimeOffset.Parse(dateTimeOffset.ToString(formatProvider), formatProvider, DateTimeStyles.None),
            (dateTimeOffset, formatProvider) => DateTimeOffset.Parse(dateTimeOffset.ToString(formatProvider), formatProvider),
            values,
            formatProviders);

        Test(
            (DateTimeOffset dateTimeOffset, out DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}", null, out result),
            (DateTimeOffset dateTimeOffset, out DateTimeOffset result) => DateTimeOffset.TryParse($"{dateTimeOffset}", out result),
            values);
        Test(
            (DateTimeOffset dateTimeOffset, out DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), null, out result),
            (DateTimeOffset dateTimeOffset, out DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), out result),
            values);
        Test(
            (DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out DateTimeOffset result) => DateTimeOffset.TryParse(
                $"{dateTimeOffset}",
                formatProvider,
                DateTimeStyles.None,
                out result),
            (DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}", formatProvider, out result),
            values,
            formatProviders);
        Test(
            (DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out DateTimeOffset result) => MissingDateTimeOffsetMembers.TryParse(
                $"{dateTimeOffset}".AsSpan(),
                formatProvider,
                DateTimeStyles.None,
                out result),
            (DateTimeOffset dateTimeOffset, IFormatProvider? formatProvider, out DateTimeOffset result)
                => MissingDateTimeOffsetMembers.TryParse($"{dateTimeOffset}".AsSpan(), formatProvider, out result),
            values,
            formatProviders);

        Test(
            (dateTimeOffset, format, formatProvider) => DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider,
                DateTimeStyles.None),
            (dateTimeOffset, format, formatProvider) => DateTimeOffset.ParseExact(
                dateTimeOffset.ToString(format, formatProvider),
                format,
                formatProvider),
            [..values.Except([DateTimeOffset.MinValue, DateTimeOffset.MaxValue])],
            formats,
            formatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDateOnly()
    {
        var values = new[] { DateOnly.MinValue, DateOnly.MaxValue, new(2025, 7, 15) };
        var formats = new[] { "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" };

        // redundant argument

        Test(dateOnly => DateOnly.Parse(dateOnly.ToString(), null), dateOnly => DateOnly.Parse(dateOnly.ToString()), values);
        Test(dateOnly => DateOnly.Parse(dateOnly.ToString().AsSpan(), null), dateOnly => DateOnly.Parse(dateOnly.ToString().AsSpan()), values);

        Test(
            (DateOnly dateOnly, out DateOnly result) => DateOnly.TryParse(dateOnly.ToString(), null, out result),
            (DateOnly dateOnly, out DateOnly result) => DateOnly.TryParse(dateOnly.ToString(), out result),
            values);
        Test(
            (DateOnly dateOnly, out DateOnly result) => DateOnly.TryParse(dateOnly.ToString().AsSpan(), null, out result),
            (DateOnly dateOnly, out DateOnly result) => DateOnly.TryParse(dateOnly.ToString().AsSpan(), out result),
            values);
        Test(
            (DateOnly dateOnly, IFormatProvider? provider, out DateOnly result) => DateOnly.TryParse(
                dateOnly.ToString(),
                provider,
                DateTimeStyles.None,
                out result),
            (DateOnly dateOnly, IFormatProvider? provider, out DateOnly result) => DateOnly.TryParse(dateOnly.ToString(), provider, out result),
            values,
            formatProviders);
        Test(
            (DateOnly dateOnly, IFormatProvider? provider, out DateOnly result) => DateOnly.TryParse(
                dateOnly.ToString().AsSpan(),
                provider,
                DateTimeStyles.None,
                out result),
            (DateOnly dateOnly, IFormatProvider? provider, out DateOnly result)
                => DateOnly.TryParse(dateOnly.ToString().AsSpan(), provider, out result),
            values,
            formatProviders);

        // redundant argument range

        Test(dateOnly => DateOnly.Parse(dateOnly.ToString(), null, DateTimeStyles.None), dateOnly => DateOnly.Parse(dateOnly.ToString()), values);

        Test(
            (dateOnly, format) => DateOnly.ParseExact(dateOnly.ToString(format), format, null, DateTimeStyles.None),
            (dateOnly, format) => DateOnly.ParseExact(dateOnly.ToString(format), format),
            values,
            formats);
        Test(
            (dateOnly, format) => DateOnly.ParseExact(dateOnly.ToString(format), formats, null),
            (dateOnly, format) => DateOnly.ParseExact(dateOnly.ToString(format), formats),
            values,
            formats);
        Test(
            (dateOnly, format) => DateOnly.ParseExact(dateOnly.ToString(format).AsSpan(), formats, null),
            (dateOnly, format) => DateOnly.ParseExact(dateOnly.ToString(format).AsSpan(), formats),
            values,
            formats);

        Test(
            (DateOnly dateOnly, string format, out DateOnly result) => DateOnly.TryParseExact(
                dateOnly.ToString(format),
                format,
                null,
                DateTimeStyles.None,
                out result),
            (DateOnly dateOnly, string format, out DateOnly result) => DateOnly.TryParseExact(dateOnly.ToString(format), format, out result),
            values,
            formats);
        Test(
            (DateOnly dateOnly, string format, out DateOnly result) => DateOnly.TryParseExact(
                dateOnly.ToString(format).AsSpan(),
                format.AsSpan(),
                null,
                DateTimeStyles.None,
                out result),
            (DateOnly dateOnly, string format, out DateOnly result)
                => DateOnly.TryParseExact(dateOnly.ToString(format).AsSpan(), format.AsSpan(), out result),
            values,
            formats);
        Test(
            (DateOnly dateOnly, string format, out DateOnly result) => DateOnly.TryParseExact(
                dateOnly.ToString(format),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (DateOnly dateOnly, string format, out DateOnly result) => DateOnly.TryParseExact(dateOnly.ToString(format), format, out result),
            values,
            formats);
        Test(
            (DateOnly dateOnly, string format, out DateOnly result) => DateOnly.TryParseExact(
                dateOnly.ToString(format).AsSpan(),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (DateOnly dateOnly, string format, out DateOnly result)
                => DateOnly.TryParseExact(dateOnly.ToString(format).AsSpan(), formats, out result),
            values,
            formats);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTimeOnly()
    {
        var values = new[] { TimeOnly.MinValue, TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5) };
        var formats = new[] { "t", "T", "o", "O", "r", "R" };

        // redundant argument

        Test(
            (timeOnly, value) => timeOnly.Add(value, out _),
            (timeOnly, value) => timeOnly.Add(value),
            values,
            [TimeSpan.FromDays(2), TimeSpan.FromDays(-2), TimeSpan.FromHours(23), TimeSpan.FromHours(-23)]);

        Test((timeOnly, value) => timeOnly.AddHours(value, out _), (timeOnly, value) => timeOnly.AddHours(value), values, [48, -48, 23, -23]);

        Test(
            (timeOnly, value) => timeOnly.AddMinutes(value, out _),
            (timeOnly, value) => timeOnly.AddMinutes(value),
            values,
            [48 * 60, -48 * 60, 23 * 60, -23 * 60]);

        Test(timeOnly => TimeOnly.Parse(timeOnly.ToString(), null), timeOnly => TimeOnly.Parse(timeOnly.ToString()), values);
        Test(timeOnly => TimeOnly.Parse(timeOnly.ToString().AsSpan(), null), timeOnly => TimeOnly.Parse(timeOnly.ToString().AsSpan()), values);

        Test(
            (timeOnly, format) => TimeOnly.ParseExact(timeOnly.ToString(format), formats, null),
            (timeOnly, format) => TimeOnly.ParseExact(timeOnly.ToString(format), formats),
            values,
            formats);
        Test(
            (timeOnly, format) => TimeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), formats, null),
            (timeOnly, format) => TimeOnly.ParseExact(timeOnly.ToString(format).AsSpan(), formats),
            values,
            formats);

        Test(
            (TimeOnly timeOnly, out TimeOnly result) => TimeOnly.TryParse(timeOnly.ToString(), null, out result),
            (TimeOnly timeOnly, out TimeOnly result) => TimeOnly.TryParse(timeOnly.ToString(), out result),
            values);
        Test(
            (TimeOnly timeOnly, out TimeOnly result) => TimeOnly.TryParse(timeOnly.ToString().AsSpan(), null, out result),
            (TimeOnly timeOnly, out TimeOnly result) => TimeOnly.TryParse(timeOnly.ToString().AsSpan(), out result),
            values);
        Test(
            (TimeOnly timeOnly, IFormatProvider? provider, out TimeOnly result) => TimeOnly.TryParse(
                timeOnly.ToString(),
                provider,
                DateTimeStyles.None,
                out result),
            (TimeOnly timeOnly, IFormatProvider? provider, out TimeOnly result) => TimeOnly.TryParse(timeOnly.ToString(), provider, out result),
            values,
            formatProviders);
        Test(
            (TimeOnly timeOnly, IFormatProvider? provider, out TimeOnly result) => TimeOnly.TryParse(
                timeOnly.ToString().AsSpan(),
                provider,
                DateTimeStyles.None,
                out result),
            (TimeOnly timeOnly, IFormatProvider? provider, out TimeOnly result)
                => TimeOnly.TryParse(timeOnly.ToString().AsSpan(), provider, out result),
            values,
            formatProviders);

        // redundant argument range

        Test(timeOnly => TimeOnly.Parse(timeOnly.ToString(), null, DateTimeStyles.None), timeOnly => TimeOnly.Parse(timeOnly.ToString()), values);

        Test(
            (timeOnly, format) => TimeOnly.ParseExact(timeOnly.ToString(format), format, null, DateTimeStyles.None),
            (timeOnly, format) => TimeOnly.ParseExact(timeOnly.ToString(format), format),
            values,
            formats);

        Test(
            (TimeOnly timeOnly, string format, out TimeOnly result) => TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                format,
                null,
                DateTimeStyles.None,
                out result),
            (TimeOnly timeOnly, string format, out TimeOnly result) => TimeOnly.TryParseExact(timeOnly.ToString(format), format, out result),
            values,
            formats);
        Test(
            (TimeOnly timeOnly, string format, out TimeOnly result) => TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                format.AsSpan(),
                null,
                DateTimeStyles.None,
                out result),
            (TimeOnly timeOnly, string format, out TimeOnly result)
                => TimeOnly.TryParseExact(timeOnly.ToString(format).AsSpan(), format.AsSpan(), out result),
            values,
            formats);
        Test(
            (TimeOnly timeOnly, string format, out TimeOnly result) => TimeOnly.TryParseExact(
                timeOnly.ToString(format),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (TimeOnly timeOnly, string format, out TimeOnly result) => TimeOnly.TryParseExact(timeOnly.ToString(format), formats, out result),
            values,
            formats);
        Test(
            (TimeOnly timeOnly, string format, out TimeOnly result) => TimeOnly.TryParseExact(
                timeOnly.ToString(format).AsSpan(),
                formats,
                null,
                DateTimeStyles.None,
                out result),
            (TimeOnly timeOnly, string format, out TimeOnly result)
                => TimeOnly.TryParseExact(timeOnly.ToString(format).AsSpan(), formats, out result),
            values,
            formats);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantExplicitParamsArrayCreation")]
    public void TestString()
    {
        var values = new[] { null, "", "abcde", "  abcde  ", "ab;cd;e", "..abcde.." };
        var comparisons = new[]
        {
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase,
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
        };

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

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestStringBuilder()
    {
        var values = new[] { "", "abcde" };

        // redundant argument

        Test(value => new StringBuilder(value).Append('x', 1).ToString(), value => new StringBuilder(value).Append('x').ToString(), values);

        Test(
            value => new StringBuilder(value).Insert(1, "xyz", 1).ToString(),
            value => new StringBuilder(value).Insert(1, "xyz").ToString(),
            [..values.Except([""])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestMath()
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
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        // redundant argument

        Test(n => Math.Round(n, 0), n => Math.Round(n), decimalValues);
        Test(n => Math.Round(n, MidpointRounding.ToEven), n => Math.Round(n), decimalValues);
        Test((n, mode) => Math.Round(n, 0, mode), (n, mode) => Math.Round(n, mode), decimalValues, roundings);
        Test(
            (n, decimals) => Math.Round(n, decimals, MidpointRounding.ToEven),
            (n, decimals) => Math.Round(n, decimals),
            decimalValues,
            digitsValues);

        Test(n => Math.Round(n, 0), n => Math.Round(n), doubleValues);
        Test(n => Math.Round(n, MidpointRounding.ToEven), n => Math.Round(n), doubleValues);
        Test((n, mode) => Math.Round(n, 0, mode), (n, mode) => Math.Round(n, mode), doubleValues, roundings);
        Test((n, digits) => Math.Round(n, digits, MidpointRounding.ToEven), (n, digits) => Math.Round(n, digits), doubleValues, digitsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore20]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestMathF()
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
        var roundings = new[] { MidpointRounding.ToEven, MidpointRounding.AwayFromZero };
        var digitsValues = new[] { 0, 1, 2 };

        // redundant argument

        Test(n => MissingMathFMethods.Round(n, 0), n => MissingMathFMethods.Round(n), values);
        Test(n => MissingMathFMethods.Round(n, MidpointRounding.ToEven), n => MissingMathFMethods.Round(n), values);
        Test((n, mode) => MissingMathFMethods.Round(n, 0, mode), (n, mode) => MissingMathFMethods.Round(n, mode), values, roundings);
        Test(
            (n, digits) => MissingMathFMethods.Round(n, digits, MidpointRounding.ToEven),
            (n, digits) => MissingMathFMethods.Round(n, digits),
            values,
            digitsValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestRandom()
    {
        // redundant argument

        Test(() => new Random(1).Next(int.MaxValue), () => new Random(1).Next());
        Test(() => new Random(1).Next(0, 10), () => new Random(1).Next(10));

        Test(() => new Random(1).NextInt64(long.MaxValue), () => new Random(1).NextInt64());
        Test(() => new Random(1).NextInt64(0, 10), () => new Random(1).NextInt64(10));

        DoNamedTest2();
    }
}