using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUInt32Methods
{
    extension(uint)
    {
        [Pure]
        public static uint Clamp(uint value, uint min, uint max)
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
        public static (uint Quotient, uint Remainder) DivRem(uint left, [ValueRange(1, uint.MaxValue)] uint right)
        {
            var quotient = left / right;
            return (quotient, left - quotient * right);
        }

        [Pure]
        public static uint Max(uint x, uint y) => x >= y ? x : y;

        [Pure]
        public static uint Min(uint x, uint y) => x <= y ? x : y;

        [Pure]
        public static uint Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
            => uint.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), provider);

        [Pure]
        public static uint Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
            => uint.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

        [Pure]
        public static uint Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => uint.Parse(s.ToString(), provider);

        [Pure]
        public static uint Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
            => uint.Parse(s.ToString(), style, provider);

        [Pure]
        public static uint RotateLeft(uint value, int rotateAmount) => value << rotateAmount | value >> (32 - rotateAmount);

        [Pure]
        public static uint RotateRight(uint value, int rotateAmount) => value >> rotateAmount | value << (32 - rotateAmount);

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out uint result)
            => uint.TryParse(s, NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out uint result)
            => uint.TryParse(s.ToString(), style, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out uint result)
            => uint.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, out uint result) => uint.TryParse(s.ToString(), out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out uint result)
            => uint.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out uint result)
            => uint.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, out uint result)
            => uint.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
    }
}