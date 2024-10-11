using System;

namespace Test
{
    public class Strings
    {
        public void Empty(string text)
        {
            var result1 = text.Contains("");
            var result2 = text.Contains(value: "");

            var result3 = text.Contains("", StringComparison.Ordinal);
            var result4 = text.Contains(value: "", StringComparison.CurrentCulture);
        }

        public void SingleCharacter(string text)
        {
            var result1 = text.Contains("a");
            var result2 = text.Contains(value: "a");

            var result3 = text.Contains("a", StringComparison.Ordinal);
            var result4 = text.Contains(value: "a", StringComparison.CurrentCulture);
        }

        public void NoDetection(string text, string value)
        {
            text.Contains("");
            text.Contains("", StringComparison.Ordinal);

            var result1 = text.Contains("abc");
            var result2 = text.Contains(value);

            var result3 = text.Contains("abc", StringComparison.CurrentCulture);
            var result4 = text.Contains(value, StringComparison.CurrentCulture);
        }
    }
}