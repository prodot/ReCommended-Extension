using System;

namespace Test
{
    public class Strings
    {
        public void PadLeft(string text, char c, string s)
        {
            var result1 = text.PadLeft(0);
            var result2 = text.PadLeft(0, c);
        }

        public void PadRight(string text, char c, string s)
        {
            var result1 = text.PadRight(0);
            var result2 = text.PadRight(0, c);
        }

        public void Remove(string text, int startIndex)
        {
            var result1 = text.Remove(startIndex, 0);
        }

        public void Replace(string text)
        {
            var result11 = text.Replace("a", "a", StringComparison.Ordinal);
            var result11 = text.Replace("aa", "aa", StringComparison.Ordinal);

            var result21 = text.Replace("a", "a");
            var result22 = text.Replace("aa", "aa");

            var result31 = text.Replace('a', 'a');
        }

        public void Substring(string text)
        {
            var result = text.Substring(0);
        }
    }
}