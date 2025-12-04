namespace ReCommendedExtension.Tests.Missing;

internal static class MissingEnumMethods
{
    extension(Enum)
    {
        [Pure]
        public static E Parse<E>(string value, bool ignoreCase) where E : struct, Enum => (E)Enum.Parse(typeof(E), value, ignoreCase);

        [Pure]
        public static E Parse<E>(string value) where E : struct, Enum => (E)Enum.Parse(typeof(E), value);

        [Pure]
        public static E Parse<E>(ReadOnlySpan<char> value, bool ignoreCase) where E : struct, Enum
            => (E)Enum.Parse(typeof(E), value.ToString(), ignoreCase);

        [Pure]
        public static E Parse<E>(ReadOnlySpan<char> value) where E : struct, Enum => (E)Enum.Parse(typeof(E), value.ToString());

        [Pure]
        public static object Parse(Type enumType, ReadOnlySpan<char> value, bool ignoreCase) => Enum.Parse(enumType, value.ToString(), ignoreCase);

        [Pure]
        public static object Parse(Type enumType, ReadOnlySpan<char> value) => Enum.Parse(enumType, value.ToString());

        [Pure]
        public static bool TryParse<E>(ReadOnlySpan<char> value, bool ignoreCase, out E result) where E : struct, Enum
            => Enum.TryParse(value.ToString(), ignoreCase, out result);

        [Pure]
        public static bool TryParse<E>(ReadOnlySpan<char> value, out E result) where E : struct, Enum => Enum.TryParse(value.ToString(), out result);

        [Pure]
        public static bool TryParse(Type enumType, string? value, bool ignoreCase, [NotNullWhen(true)] out object? result)
        {
            try
            {
                result = Enum.Parse(enumType, value ?? "", ignoreCase);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        [Pure]
        public static bool TryParse(Type enumType, string? value, [NotNullWhen(true)] out object? result)
        {
            try
            {
                result = Enum.Parse(enumType, value ?? "");
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        [Pure]
        public static bool TryParse(Type enumType, ReadOnlySpan<char> value, bool ignoreCase, [NotNullWhen(true)] out object? result)
            => TryParse(enumType, value.ToString(), ignoreCase, out result);

        [Pure]
        public static bool TryParse(Type enumType, ReadOnlySpan<char> value, [NotNullWhen(true)] out object? result)
            => TryParse(enumType, value.ToString(), out result);
    }
}