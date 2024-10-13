using System;

namespace Test
{
    public class Strings
    {
        public void ExpressionResult(string text, char c)
        {
            var result = text.LastIndexOf(c, 0);
        }

        public void StringProperty(string text)
        {
            var result1 = text.LastIndexOf("");
            var result2 = text.LastIndexOf("", StringComparison.OrdinalIgnoreCase);
        }

        public void AsCharacter(string text)
        {
            var result = text.LastIndexOf("a", StringComparison.Ordinal);
        }

        public void NoDetection(string text, char c, int startIndex, StringComparison sc)
        {
            var result1 = text.LastIndexOf(c, 1);
            var result2 = text.LastIndexOf(c, startIndex);

            var result3 = text.LastIndexOf("abc");
            var result4 = text.LastIndexOf("abc", StringComparison.OrdinalIgnoreCase);

            var result5 = text.LastIndexOf("a", StringComparison.OrdinalIgnoreCase);
            var result6 = text.LastIndexOf("a", sc);

            text.LastIndexOf(c, 0);
            text.LastIndexOf("");
            text.LastIndexOf("", StringComparison.OrdinalIgnoreCase)
        }
    }
}