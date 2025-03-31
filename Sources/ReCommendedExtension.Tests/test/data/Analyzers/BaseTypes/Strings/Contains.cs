using System;

namespace Test
{
    public class Strings
    {
        public void Empty(string text, StringComparison comparison)
        {
            var result0 = text.Contains(string.Empty);
            var result1 = text.Contains("");
            var result2 = text.Contains(value: "");

            var result3 = text.Contains("", comparison);
            var result4 = text.Contains(value: "", comparison);
        }

        public void SingleCharacter(string text, StringComparison comparison)
        {
            var result1 = text.Contains("a");
            var result2 = text.Contains(value: "a");

            var result3 = text.Contains("a", comparison);
            var result4 = text.Contains(value: "a", comparison);
        }

        public void NoDetection(string text, string value, StringComparison comparison)
        {
            text.Contains("");
            text.Contains("", comparison);

            var result1 = text.Contains("abc");
            var result2 = text.Contains(value);

            var result3 = text.Contains("abc", comparison);
            var result4 = text.Contains(value, comparison);
        }

        public void NoDetection(string? text, StringComparison comparison)
        {
            var result1 = text?.Contains("");
            var result2 = text?.Contains("", comparison);
        }
    }
}