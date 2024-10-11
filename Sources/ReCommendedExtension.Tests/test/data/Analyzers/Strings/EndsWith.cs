using System;

namespace Test
{
    public class Strings
    {
        public void Empty(string text)
        {
            var result1 = text.EndsWith("");
            var result2 = text.EndsWith("", StringComparison.OrdinalIgnoreCase);
        }

        public void Char(string text, char c)
        {
            var result1 = text.EndsWith('a');
            var result2 = text.EndsWith(c);
        }

        public void SingleCharacter(string text)
        {
            var result1 = text.EndsWith("a", StringComparison.Ordinal);
            var result2 = text.EndsWith("a", StringComparison.OrdinalIgnoreCase);
        }

        public void NoDetection(string text, string value)
        {
            text.EndsWith("");
            text.EndsWith("", StringComparison.OrdinalIgnoreCase)

            text.EndsWith('a');
            text.EndsWith("a", StringComparison.Ordinal);

            var result1 = text.EndsWith("abc", StringComparison.Ordinal);
            var result2 = text.EndsWith(value, StringComparison.Ordinal);

            var result3 = text.EndsWith("a", StringComparison.CurrentCulture);
            var result4 = text.EndsWith("a", StringComparison.CurrentCulture);
        }
    }
}