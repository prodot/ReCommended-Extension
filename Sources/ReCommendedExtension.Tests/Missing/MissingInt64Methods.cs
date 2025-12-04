using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingInt64Methods
{
    extension(long)
    {
        [Pure]
        public static long Clamp(long value, long min, long max)
        {
            if (min > max)
            {
                throw new ArgumentException($"'{min}' cannot be greater than {max}.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        [Pure]
        public static (long Quotient, long Remainder) DivRem(long left, [ValueRange(long.MinValue, -1)][ValueRange(1, long.MaxValue)] long right)
        {
            var quotient = left / right;
            return (quotient, left - quotient * right);
        }

        [Pure]
        public static bool IsNegative(long value) => value < 0;

        [Pure]
        public static bool IsPositive(long value) => value >= 0;

        [Pure]
        public static long Max(long x, long y) => x >= y ? x : y;

        [Pure]
        public static long Min(long x, long y) => x <= y ? x : y;

        [Pure]
        public static long MaxMagnitude(long x, long y)
        {
            if (x == long.MinValue)
            {
                return x;
            }

            if (y == long.MinValue)
            {
                return y;
            }

            var ax = Math.Abs(x);
            var ay = Math.Abs(y);

            if (ax > ay)
            {
                return x;
            }

            if (ax < ay)
            {
                return y;
            }

            return x < 0 ? y : x;
        }

        [Pure]
        public static long MinMagnitude(long x, long y)
        {
            if (x == long.MinValue)
            {
                return y;
            }

            if (y == long.MinValue)
            {
                return x;
            }

            var ax = Math.Abs(x);
            var ay = Math.Abs(y);

            if (ax < ay)
            {
                return x;
            }

            if (ax > ay)
            {
                return y;
            }

            return x < 0 ? x : y;
        }

        [Pure]
        public static long Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
            => long.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), provider);

        [Pure]
        public static long Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
            => long.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

        [Pure]
        public static long Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => long.Parse(s.ToString(), provider);

        [Pure]
        public static long Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
            => long.Parse(s.ToString(), style, provider);

        [Pure]
        public static long RotateLeft(long value, int rotateAmount) => unchecked((long)ulong.RotateLeft((ulong)value, rotateAmount));

        [Pure]
        public static long RotateRight(long value, int rotateAmount) => unchecked((long)ulong.RotateRight((ulong)value, rotateAmount));

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out long result)
            => long.TryParse(s, NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out long result)
            => long.TryParse(s.ToString(), style, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out long result)
            => long.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, out long result) => long.TryParse(s.ToString(), out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out long result)
            => long.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out long result)
            => long.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, out long result)
            => long.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
    }
}