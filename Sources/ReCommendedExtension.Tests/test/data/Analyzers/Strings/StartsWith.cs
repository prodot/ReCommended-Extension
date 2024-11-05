using System;

namespace Test
{
    public class Strings
    {
        public void Empty(string text, StringComparison comparisonType)
        {
            var result1 = text.StartsWith("");
            var result2 = text.StartsWith("", comparisonType);
        }

        public void Char(string text, char c)
        {
            var result1 = text.StartsWith('a');
            var result2 = text.StartsWith(c);
        }

        public void SingleCharacter(string text)
        {
            var result1 = text.StartsWith("a", StringComparison.Ordinal);
            var result2 = text.StartsWith("a", StringComparison.OrdinalIgnoreCase);
        }

        public void NoDetection(string text, string value, StringComparison comparisonType)
        {
            text.StartsWith("");
            text.StartsWith("", StringComparison.OrdinalIgnoreCase)

            text.StartsWith('a');
            text.StartsWith("a", StringComparison.Ordinal);
            text.StartsWith("a", StringComparison.OrdinalIgnoreCase);

            var result1 = text.StartsWith("abc", StringComparison.Ordinal);
            var result2 = text.StartsWith(value, StringComparison.Ordinal);

            var result3 = text.StartsWith("a", StringComparison.CurrentCulture);
            var result4 = text.StartsWith("a", StringComparison.CurrentCulture);
        }

        public void NoDetection(string? text, char c)
        {
            var result1 = text?.StartsWith('a');
            var result2 = text?.StartsWith(c);
            var result3 = text?.StartsWith("");
            var result4 = text?.StartsWith("a", StringComparison.Ordinal);
            var result5 = text?.StartsWith("a", StringComparison.OrdinalIgnoreCase);
        }
    }
}