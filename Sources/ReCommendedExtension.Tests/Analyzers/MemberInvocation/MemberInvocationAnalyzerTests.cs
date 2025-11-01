using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

using int128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Int128;
using uint128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.UInt128;

[TestFixture]
public sealed class MemberInvocationAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantMethodInvocationHint
                or UseOtherMethodSuggestion
                or UseBinaryOperatorSuggestion
                or UseUnaryOperatorSuggestion
                or UsePatternSuggestion
                or UseNullableHasValueAlternativeSuggestion
                or ReplaceNullableValueWithTypeCastSuggestion
                or UseRangeIndexerSuggestion
                or UsePropertySuggestion
                or UseStaticPropertySuggestion
                or RedundantToStringCallWarning // to figure out which cases are supported by R#
                or ReplaceSubstringWithRangeIndexerWarning // to figure out which cases are supported by R#
            || highlighting.IsError();

    static void Test<T, R>(Func<T, R> expected, Func<T, R> actual, T[] args)
    {
        foreach (var a in args)
        {
            Assert.AreEqual(expected(a), actual(a), $"with value: {a}");
        }
    }

    static void TestStringBuilder(Func<StringBuilder, StringBuilder> expected, Func<StringBuilder, StringBuilder> actual, string[] args)
        => Test(value => expected(new StringBuilder(value)).ToString(), value => actual(new StringBuilder(value)).ToString(), args);

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

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestBoolean()
    {
        var values = new[] { true, false };

        // redundant method invocation

        Test(flag => flag.Equals(true), flag => flag, values);

        // binary operator

        Test((flag, value) => flag.Equals(value), (flag, obj) => flag == obj, values, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestByte()
    {
        var values = new byte[] { 0, 1, 2, byte.MaxValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestSByte()
    {
        var values = new sbyte[] { 0, 1, 2, -1, -2, sbyte.MaxValue, sbyte.MinValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        Test(number => MissingSByteMethods.IsNegative(number), number => number < 0, values);
        Test(number => MissingSByteMethods.IsPositive(number), number => number >= 0, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestInt16()
    {
        var values = new short[] { 0, 1, 2, -1, -2, short.MaxValue, short.MinValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        Test(number => MissingInt16Methods.IsNegative(number), number => number < 0, values);
        Test(number => MissingInt16Methods.IsPositive(number), number => number >= 0, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestUInt16()
    {
        var values = new ushort[] { 0, 1, 2, ushort.MaxValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestInt32()
    {
        var values = new[] { 0, 1, 2, -1, -2, int.MaxValue, int.MinValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        Test(number => MissingInt32Methods.IsNegative(number), number => number < 0, values);
        Test(number => MissingInt32Methods.IsPositive(number), number => number >= 0, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestUInt32()
    {
        var values = new uint[] { 0, 1, 2, uint.MaxValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestInt64()
    {
        var values = new[] { 0, 1, 2, -1, -2, long.MaxValue, long.MinValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        Test(number => MissingInt64Methods.IsNegative(number), number => number < 0, values);
        Test(number => MissingInt64Methods.IsPositive(number), number => number >= 0, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestUInt64()
    {
        var values = new ulong[] { 0, 1, 2, ulong.MaxValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestInt128()
    {
        var values = new[] { 0, 1, 2, -1, -2, int128.MaxValue, int128.MinValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        Test(number => int128.IsNegative(number), number => number < 0, values);
        Test(number => int128.IsPositive(number), number => number >= 0, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestUInt128()
    {
        var values = new[] { 0, 1, 2, uint128.MaxValue };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIntPtr()
    {
        var values = new[] { (nint)0, 1, 2, -1, -2 };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        Test(number => MissingIntPtrMethods.IsNegative(number), number => number < 0, values);
        Test(number => MissingIntPtrMethods.IsPositive(number), number => number >= 0, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestUIntPtr()
    {
        var values = new nuint[] { 0, 1, 2 };

        // binary operator

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseUnaryOperator")]
    public void TestDecimal()
    {
        var values = new[] { 0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MaxValue, decimal.MinValue };

        // binary operator

        Test(
            (d1, d2) => decimal.Add(d1, d2),
            (d1, d2) => d1 + d2,
            [..values.Except([decimal.MinValue, decimal.MaxValue])],
            [..values.Except([decimal.MinValue, decimal.MaxValue])]);
        Test(
            (d1, d2) => decimal.Subtract(d1, d2),
            (d1, d2) => d1 - d2,
            [..values.Except([decimal.MinValue, decimal.MaxValue])],
            [..values.Except([decimal.MinValue, decimal.MaxValue])]);
        Test(
            (d1, d2) => decimal.Multiply(d1, d2),
            (d1, d2) => d1 * d2,
            [..values.Except([decimal.MinValue, decimal.MaxValue])],
            [..values.Except([decimal.MinValue, decimal.MaxValue])]);
        Test((d1, d2) => decimal.Divide(d1, d2), (d1, d2) => d1 / d2, [..values.Except([decimal.MinValue, decimal.MaxValue])], [1, -1, 2, -2]);
        Test((d1, d2) => decimal.Remainder(d1, d2), (d1, d2) => d1 % d2, [..values.Except([decimal.MinValue, decimal.MaxValue])], [1, -1, 2, -2]);

        Test((number, value) => number.Equals(value), (number, value) => number == value, values, values);

        // unary operator

        Test(d => decimal.Negate(d), d => -d, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "UsePattern")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
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

        // pattern

        Test(n => double.IsNaN(n), n => n is double.NaN, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "UsePattern")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
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

        // pattern

        Test(n => float.IsNaN(n), n => n is float.NaN, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
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
        var timeSpans = new[] { TimeSpan.Zero, new TimeSpan(1, 2, 3, 4, 5), -new TimeSpan(1, 2, 3, 4, 5) };
        var dateTimeValue = new DateTime(2021, 7, 21);

        // redundant method invocation

        Test(dateTime => dateTime.AddTicks(0), dateTime => dateTime, values);

        // binary operator

        Test(
            (dateTime, timeSpan) => dateTime.Add(timeSpan),
            (dateTime, timeSpan) => dateTime + timeSpan,
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            timeSpans);

        Test(
            dateTime => dateTime.Subtract(dateTimeValue),
            dateTime => dateTime - dateTimeValue,
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])]);
        Test(
            (dateTime, timeSpan) => dateTime.Subtract(timeSpan),
            (dateTime, timeSpan) => dateTime - timeSpan,
            [..values.Except([DateTime.MinValue, DateTime.MaxValue])],
            timeSpans);

        Test((dateTime, value) => dateTime.Equals(value), (dateTime, value) => dateTime == value, values, values);
        Test((t1, t2) => DateTime.Equals(t1, t2), (t1, t2) => t1 == t2, values, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
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
        var timeSpans = new[] { TimeSpan.Zero, new TimeSpan(1, 2, 3, 4, 5), -new TimeSpan(1, 2, 3, 4, 5) };
        var dateTimeOffsetValue = new DateTimeOffset(2021, 7, 21, 13, 08, 52, TimeSpan.FromHours(2));

        // redundant method invocation

        Test(dateTimeOffset => dateTimeOffset.AddTicks(0), dateTimeOffset => dateTimeOffset, values);

        // binary operator

        Test(
            (dateTimeOffset, timeSpan) => dateTimeOffset.Add(timeSpan),
            (dateTimeOffset, timeSpan) => dateTimeOffset + timeSpan,
            [..values.Except([DateTimeOffset.MinValue, DateTimeOffset.MaxValue])],
            timeSpans);

        Test(
            dateTimeOffset => dateTimeOffset.Subtract(dateTimeOffsetValue),
            dateTimeOffset => dateTimeOffset - dateTimeOffsetValue,
            [..values.Except([DateTimeOffset.MinValue, DateTimeOffset.MaxValue])]);
        Test(
            (dateTimeOffset, timeSpan) => dateTimeOffset.Subtract(timeSpan),
            (dateTimeOffset, timeSpan) => dateTimeOffset - timeSpan,
            [..values.Except([DateTimeOffset.MinValue, DateTimeOffset.MaxValue])],
            timeSpans);

        Test((dateTimeOffset, value) => dateTimeOffset.Equals(value), (dateTimeOffset, value) => dateTimeOffset == value, values, values);
        Test((t1, t2) => DateTimeOffset.Equals(t1, t2), (t1, t2) => t1 == t2, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore20]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseUnaryOperator")]
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

        // binary operator

        Test(
            (timeSpan, ts) => timeSpan.Add(ts),
            (timeSpan, ts) => timeSpan + ts,
            [..values.Except([TimeSpan.MinValue, TimeSpan.MaxValue])],
            [..values.Except([TimeSpan.MinValue, TimeSpan.MaxValue])]);

        Test(
            (timeSpan, ts) => timeSpan.Subtract(ts),
            (timeSpan, ts) => timeSpan - ts,
            [..values.Except([TimeSpan.MinValue, TimeSpan.MaxValue])],
            [..values.Except([TimeSpan.MinValue, TimeSpan.MaxValue])]);

        Test(
            (timeSpan, factor) => timeSpan.Multiply(factor),
            (timeSpan, factor) => MissingTimeSpanMembers.op_Multiply(timeSpan, factor),
            [..values.Except([TimeSpan.MinValue, TimeSpan.MaxValue])],
            [0d, -0d, 1d, 2d, double.Epsilon]);

        Test(
            (timeSpan, divisor) => timeSpan.Divide(divisor),
            (timeSpan, divisor) => MissingTimeSpanMembers.op_Division(timeSpan, divisor),
            [..values.Except([TimeSpan.MinValue, TimeSpan.MaxValue])],
            [1d, 2d, -1d, double.MaxValue, double.MinValue, double.PositiveInfinity, double.NegativeInfinity]);
        Test((timeSpan, ts) => timeSpan.Divide(ts), (timeSpan, ts) => MissingTimeSpanMembers.op_Division(timeSpan, ts), values, values);

        Test((timeSpan, obj) => timeSpan.Equals(obj), (timeSpan, obj) => timeSpan == obj, values, values);
        Test((t1, t2) => TimeSpan.Equals(t1, t2), (t1, t2) => t1 == t2, values, values);

        // unary operator

        Test(timeSpan => timeSpan.Negate(), timeSpan => -timeSpan, [..values.Except([TimeSpan.MinValue])]);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestDateOnly()
    {
        var values = new[] { DateOnly.MinValue, DateOnly.MaxValue, new(2025, 7, 15) };

        // redundant method invocation

        Test(dateOnly => dateOnly.AddDays(0), dateOnly => dateOnly, values);

        // binary operator

        Test((dateOnly, value) => dateOnly.Equals(value), (dateOnly, value) => dateOnly == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestTimeOnly()
    {
        var values = new[] { TimeOnly.MinValue, TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5) };

        // binary operator

        Test((timeOnly, value) => timeOnly.Equals(value), (timeOnly, value) => timeOnly == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestGuid()
    {
        var values = new[] { Guid.Empty, new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]) };

        // binary operator

        Test((guid, value) => guid.Equals(value), (guid, value) => guid == value, values, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestChar()
    {
        var values = new[] { 'a', 'A', '1', ' ', 'ä', 'ß', '€', char.MinValue, char.MaxValue };

        // binary operator

        Test((c, value) => c.Equals(value), (c, value) => c == value, values, values);

        // pattern

        Test(c => MissingCharMethods.IsAsciiDigit(c), c => c is >= '0' and <= '9', values);
        Test(c => MissingCharMethods.IsAsciiHexDigit(c), c => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f', values);
        Test(c => MissingCharMethods.IsAsciiHexDigitLower(c), c => c is >= '0' and <= '9' or >= 'a' and <= 'f', values);
        Test(c => MissingCharMethods.IsAsciiHexDigitUpper(c), c => c is >= '0' and <= '9' or >= 'A' and <= 'F', values);
        Test(c => MissingCharMethods.IsAsciiLetter(c), c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z', values);
        Test(c => MissingCharMethods.IsAsciiLetterLower(c), c => c is >= 'a' and <= 'z', values);
        Test(c => MissingCharMethods.IsAsciiLetterOrDigit(c), c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9', values);
        Test(c => MissingCharMethods.IsAsciiLetterUpper(c), c => c is >= 'A' and <= 'Z', values);
        Test(c => MissingCharMethods.IsBetween(c, 'a', 'c'), c => c is >= 'a' and <= 'c', values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet70]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "StringStartsWithIsCultureSpecific")]
    [SuppressMessage("ReSharper", "UseOtherMethod")]
    [SuppressMessage("ReSharper", "MergeIntoPattern")]
    [SuppressMessage("ReSharper", "MergeIntoNegatedPattern")]
    [SuppressMessage("ReSharper", "UseRangeIndexer")]
    public void TestString()
    {
        var values = new[] { null, "", "abcde", "  abcde  ", "ab;cd;e", "ab;cd:e", "..abcde.." };
        var chars = new[] { 'c', 'x' };
        var comparisons = new[]
        {
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase,
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
        };

        var valuesNonNull = (from item in values where item is { } select item).ToArray();

        // redundant method invocation

        Test(text => text?.PadLeft(0), text => text, values);
        Test(text => text?.PadLeft(0, 'x'), text => text, values);

        Test(text => text?.PadRight(0), text => text, values);
        Test(text => text?.PadRight(0, 'x'), text => text, values);

        Test(text => text?.Replace('c', 'c'), text => text, values);
        Test(text => text?.Replace("cd", "cd"), text => text, values);
        Test(text => text?.Replace("cd", "cd", StringComparison.Ordinal), text => text, values);

        // other method invocation

        Test((text, c) => text.IndexOf(c) != -1, (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => text.IndexOf(c) > -1, (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => text.IndexOf(c) >= 0, (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => -1 != text.IndexOf(c), (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => -1 < text.IndexOf(c), (text, c) => text.Contains(c), valuesNonNull, chars);
        Test((text, c) => 0 <= text.IndexOf(c), (text, c) => text.Contains(c), valuesNonNull, chars);

        Test((text, c) => text.IndexOf(c) == -1, (text, c) => !text.Contains(c), valuesNonNull, chars);
        Test((text, c) => text.IndexOf(c) < 0, (text, c) => !text.Contains(c), valuesNonNull, chars);
        Test((text, c) => -1 == text.IndexOf(c), (text, c) => !text.Contains(c), valuesNonNull, chars);
        Test((text, c) => 0 > text.IndexOf(c), (text, c) => !text.Contains(c), valuesNonNull, chars);

        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) != -1,
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) > -1,
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) >= 0,
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => -1 != text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => -1 < text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => 0 <= text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);

        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) == -1,
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => text.IndexOf(c, comparisonType) < 0,
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => -1 == text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);
        Test(
            (text, c, comparisonType) => 0 > text.IndexOf(c, comparisonType),
            (text, c, comparisonType) => !text.Contains(c, comparisonType),
            valuesNonNull,
            chars,
            comparisons);

        Test((text, s) => text.IndexOf(s) == 0, (text, s) => text.StartsWith(s), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 == text.IndexOf(s), (text, s) => text.StartsWith(s), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) != 0, (text, s) => !text.StartsWith(s), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 != text.IndexOf(s), (text, s) => !text.StartsWith(s), valuesNonNull, valuesNonNull);

        Test((text, s) => text.IndexOf(s) != -1, (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) > -1, (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) >= 0, (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => -1 != text.IndexOf(s), (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => -1 < text.IndexOf(s), (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 <= text.IndexOf(s), (text, s) => text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);

        Test((text, s) => text.IndexOf(s) == -1, (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => text.IndexOf(s) < 0, (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => -1 == text.IndexOf(s), (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);
        Test((text, s) => 0 > text.IndexOf(s), (text, s) => !text.Contains(s, StringComparison.CurrentCulture), valuesNonNull, valuesNonNull);

        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) == 0,
            (text, s, comparisonType) => text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 == text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) != 0,
            (text, s, comparisonType) => !text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 != text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => !text.StartsWith(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);

        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) != -1,
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) > -1,
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) >= 0,
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => -1 != text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => -1 < text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 <= text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);

        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) == -1,
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => text.IndexOf(s, comparisonType) < 0,
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => -1 == text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);
        Test(
            (text, s, comparisonType) => 0 > text.IndexOf(s, comparisonType),
            (text, s, comparisonType) => !text.Contains(s, comparisonType),
            valuesNonNull,
            valuesNonNull,
            comparisons);

        Test((text, c) => text?.IndexOfAny([c]), (text, c) => text?.IndexOf(c), values, chars);
        Test((text, c) => text?.IndexOfAny([c], 1), (text, c) => text?.IndexOf(c, 1), [..values.Except([""])], chars);
        Test((text, c) => text?.IndexOfAny([c], 1, 3), (text, c) => text?.IndexOf(c, 1, 3), [..values.Except([""])], chars);

        Test((text, c) => text?.LastIndexOfAny([c]), (text, c) => text?.LastIndexOf(c), values, chars);
        Test((text, c) => text?.LastIndexOfAny([c], 4), (text, c) => text?.LastIndexOf(c, 4), values, chars);
        Test((text, c) => text?.LastIndexOfAny([c], 4, 3), (text, c) => text?.LastIndexOf(c, 4, 3), values, chars);

        // pattern

        Test(text => text.EndsWith('e'), text => text is [.., 'e'], valuesNonNull);
        Test(text => text.EndsWith('e'), text => text is [.., var lastChar] && lastChar == 'e', valuesNonNull);
        Test(text => text.EndsWith("e", StringComparison.Ordinal), text => text is [.., 'e'], valuesNonNull);
        Test(text => text.EndsWith("e", StringComparison.OrdinalIgnoreCase), text => text is [.., 'e' or 'E'], valuesNonNull);

        Test(text => text?.IndexOf('a') == 0, text => text is ['a', ..], values);
        Test(text => 0 == text?.IndexOf('a'), text => text is ['a', ..], values);
        Test(text => text?.IndexOf('a') != 0, text => text is not ['a', ..], values);
        Test(text => 0 != text?.IndexOf('a'), text => text is not ['a', ..], values);
        Test(text => text?.IndexOf('a') == 0, text => text is [var firstChar, ..] && firstChar == 'a', values);
        Test(text => 0 == text?.IndexOf('a'), text => text is [var firstChar, ..] && firstChar == 'a', values);
        Test(text => text?.IndexOf('a') != 0, text => text is not [var firstChar, ..] || firstChar != 'a', values);
        Test(text => 0 != text?.IndexOf('a'), text => text is not [var firstChar, ..] || firstChar != 'a', values);

        Test(text => text.StartsWith('a'), text => text is ['a', ..], valuesNonNull);
        Test(text => text.StartsWith('a'), text => text is [var firstChar, ..] && firstChar == 'a', valuesNonNull);
        Test(text => text.StartsWith("a", StringComparison.Ordinal), text => text is ['a', ..], valuesNonNull);
        Test(text => text.StartsWith("a", StringComparison.OrdinalIgnoreCase), text => text is ['a' or 'A', ..], valuesNonNull);

        // range indexer

        Test(text => text?.Remove(1), text => text?[..1], [..values.Except([""])]);
        Test(text => text?.Remove(0, 1), text => text?[1..], [..values.Except([""])]);

        // property
        // todo: uncomment the tests below when this is built with .NET 5 (https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/lastindexof-improved-handling-of-empty-values)
        /*
        Test(text => text?.LastIndexOf(""), text => text?.Length, values);
        Test((text, comparisonType) => text?.LastIndexOf("", comparisonType), (text, _) => text?.Length, values, comparisons);        
        */

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public void TestStringBuilder()
    {
        var values = new[] { "", "abcde" };

        // redundant method invocation

        TestStringBuilder(builder => builder.Append(null as string), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as string), builder => builder, values);
        TestStringBuilder(builder => builder.Append(""), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as char[]), builder => builder, values);
        TestStringBuilder(builder => builder.Append([]), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as object), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as StringBuilder), builder => builder, values);
        TestStringBuilder(builder => builder.Append('x', 0), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as string, 0, 0), builder => builder, values);
        TestStringBuilder(builder => builder.Append("abcde", 1, 0), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as StringBuilder, 0, 0), builder => builder, values);
        TestStringBuilder(builder => builder.Append(new StringBuilder("abcde"), 1, 0), builder => builder, values);
        TestStringBuilder(builder => builder.Append(null as char[], 0, 0), builder => builder, values);
        TestStringBuilder(builder => builder.Append([], 0, 0), builder => builder, values);

        TestStringBuilder(builder => builder.AppendJoin(',', (IEnumerable<int>)[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(',', (string[])[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(',', (object[])[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(',', (ReadOnlySpan<string?>)[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(',', (ReadOnlySpan<object?>)[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (IEnumerable<int>)[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (string[])[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (object[])[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (ReadOnlySpan<string?>)[]), builder => builder, values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (ReadOnlySpan<object?>)[]), builder => builder, values);

        TestStringBuilder(builder => builder.Insert(0, null as object), builder => builder, values);

        TestStringBuilder(builder => builder.Replace('c', 'c'), builder => builder, values);
        TestStringBuilder(builder => builder.Replace("cd", "cd"), builder => builder, values);

        // other method invocation

        TestStringBuilder(builder => builder.AppendJoin(',', (IEnumerable<int>)[1]), builder => builder.Append((object)1), values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (IEnumerable<int>)[1]), builder => builder.Append((object)1), values);
        TestStringBuilder(builder => builder.AppendJoin(',', (string[])["item"]), builder => builder.Append("item"), values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (string[])["item"]), builder => builder.Append("item"), values);
        TestStringBuilder(builder => builder.AppendJoin(',', (object[])[1]), builder => builder.Append((object)1), values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (object[])[1]), builder => builder.Append((object)1), values);
        TestStringBuilder(builder => builder.AppendJoin(',', (ReadOnlySpan<string?>)["item"]), builder => builder.Append("item"), values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (ReadOnlySpan<string?>)["item"]), builder => builder.Append("item"), values);
        TestStringBuilder(builder => builder.AppendJoin(',', (ReadOnlySpan<object?>)[1]), builder => builder.Append((object)1), values);
        TestStringBuilder(builder => builder.AppendJoin(", ", (ReadOnlySpan<object?>)[1]), builder => builder.Append((object)1), values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "ArrangeNullCheckingPattern")]
    [SuppressMessage("ReSharper", "UseNullableHasValueAlternative")]
    [SuppressMessage("ReSharper", "Solution.PatternMatchingNullCheck")]
    [SuppressMessage("ReSharper", "ReplaceNullableValueWithTypeCast")]
    public void TestNullable()
    {
        var values = new[] { 1, null as int? };
        var tupleValues = new[] { (1, true), null as (int, bool)? };

        // binary operator

        Test(nullable => nullable.GetValueOrDefault(), nullable => nullable ?? 0, values);
        Test(nullable => nullable.GetValueOrDefault(-1), nullable => nullable ?? -1, values);

        // property of nullable

        Test(nullable => nullable.HasValue, nullable => nullable is { }, values);
        Test(nullable => nullable.HasValue, nullable => nullable is not null, values);
        Test(nullable => nullable.HasValue, nullable => nullable != null, values);
        Test(nullable => nullable.HasValue, nullable => nullable is (_, _), tupleValues);

        Test(nullable => nullable!.Value, nullable => (int)nullable!, [..values.Except(new int?[1])]);

        DoNamedTest2();
    }
}