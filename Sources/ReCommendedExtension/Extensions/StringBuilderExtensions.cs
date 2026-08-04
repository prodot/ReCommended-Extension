using System.Text;

namespace ReCommendedExtension.Extensions;

internal static class StringBuilderExtensions
{
    extension(StringBuilder stringBuilder)
    {
        [Pure]
        public bool EndsWith(string value)
        {
            if (stringBuilder.Length < value.Length)
            {
                return false;
            }

            var offset = stringBuilder.Length - value.Length;

            for (var i = 0; i < value.Length; i++)
            {
                if (stringBuilder[offset + i] != value[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}