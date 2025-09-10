using System;

namespace Test
{
    public class Strings
    {
        public void SingleCharacters(string text)
        {
            var result1 = text.Replace("a", "b", StringComparison.Ordinal);
            var result2 = text.Replace("a", "b");
        }

        public void RedundantInvocation(string text)
        {
            var result11 = text.Replace("a", "a", StringComparison.Ordinal);
            var result12 = text.Replace("aa", "aa", StringComparison.Ordinal);

            var result21 = text.Replace("a", "a");
            var result22 = text.Replace("aa", "aa");

            var result31 = text.Replace('a', 'a');
        }

        public void NoDetection(string text, string oldValue, string newValue, StringComparison comparisonType, char oldChar, char newChar)
        {
            var result11 = text.Replace("aa", "b", StringComparison.Ordinal);
            var result12 = text.Replace("a", "bb", StringComparison.Ordinal);
            var result13 = text.Replace("a", "", StringComparison.Ordinal);
            var result14 = text.Replace("a", "b", StringComparison.OrdinalIgnoreCase);
            var result15 = text.Replace("a", "b", comparisonType);
            var result16 = text.Replace(oldValue, "b", StringComparison.Ordinal);
            var result17 = text.Replace("a", newValue, StringComparison.Ordinal);

            var result21 = text.Replace("aa", "b");
            var result22 = text.Replace("a", "bb");
            var result23 = text.Replace("a", "");
            var result24 = text.Replace(oldValue, "b");
            var result25 = text.Replace("a", newValue);

            var result31 = text.Replace('a', 'b');
            var result32 = text.Replace('a', newChar);
            var result33 = text.Replace(oldChar, 'b');

            var result41 = text.Replace("a", "a", StringComparison.OrdinalIgnoreCase);
            var result42 = text.Replace("aa", "aa", StringComparison.OrdinalIgnoreCase);

            text.Replace("aa", "aa", StringComparison.Ordinal);
            text.Replace("aa", "aa");
            text.Replace('a', 'a');
        }
    }
}