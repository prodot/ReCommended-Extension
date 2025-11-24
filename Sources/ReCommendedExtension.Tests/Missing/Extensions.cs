using System.Text;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Tests.Missing;

internal static class Extensions
{
    extension<T>(T value)
    {
        [Pure]
        public string ToString(string? format)
            => value switch
            {
                Half v => v.ToString(format),
                Int128 v => v.ToString(format),
                UInt128 v => v.ToString(format),
                IFormattable formattable => formattable.ToString(format, null),
                nint v => v.ToString(format),
                nuint v => v.ToString(format),

                _ => throw new NotSupportedException(),
            };

        [Pure]
        public string ToString(IFormatProvider? provider)
            => value switch
            {
                Half v => v.ToString(provider),
                Int128 v => v.ToString(provider),
                UInt128 v => v.ToString(provider),
                IConvertible convertible => convertible.ToString(provider),
                IFormattable formattable => formattable.ToString(null, provider),
                nint v => v.ToString(provider),
                nuint v => v.ToString(provider),

                _ => throw new NotSupportedException(),
            };

        [Pure]
        public string ToString(string? format, IFormatProvider? provider)
            => value switch
            {
                IFormattable formattable => formattable.ToString(format, provider),
                nint v => v.ToString(format, provider),
                nuint v => v.ToString(format, provider),

                _ => throw new NotSupportedException(),
            };
    }

    extension(string value)
    {
        [Pure]
        public byte[] AsUtf8Bytes() => Encoding.UTF8.GetBytes(value);
    }
}