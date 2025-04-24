using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingSingleMethods
{
    [Pure]
    public static float Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => float.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static float Parse(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => float.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static float Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => float.Parse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static float Parse(
        ReadOnlySpan<char> s,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => float.Parse(s.ToString(), style, provider);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out float result)
        => float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out float result)
        => float.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out float result)
        => float.TryParse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out float result) => float.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out float result)
        => float.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out float result)
        => float.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out float result)
        => float.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}