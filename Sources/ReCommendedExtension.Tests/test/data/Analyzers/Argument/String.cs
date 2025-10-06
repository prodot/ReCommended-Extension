using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string text, char c, char[] charArray, string s, StringComparison comparison, int totalWidth)
        {
            var result11 = text.IndexOf(c, 0);
            var result12 = text.IndexOf(s, 0);
            var result13 = text.IndexOf(s, 0, comparison);

            var result21 = text.IndexOfAny(charArray, 0);

            var result31 = text.PadLeft(totalWidth, ' ');

            var result41 = text.PadRight(totalWidth, ' ');

            var result51 = text.Split(';', ';', ';');

            var result61 = text.Trim(null);
            var result62 = text.Trim([]);
            var result63 = text.Trim('.', '.', '.');

            var result71 = text.TrimEnd(null);
            var result72 = text.TrimEnd([]);
            var result73 = text.TrimEnd('.', '.', '.');

            var result81 = text.TrimStart(null);
            var result82 = text.TrimStart([]);
            var result82 = text.TrimStart('.', '.', '.');
        }

        public void NoDetection(string text, char c, char[] charArray, string s, int startIndex, StringComparison comparison, int totalWidth)
        {
            var result11 = text.IndexOf(c, startIndex);
            var result12 = text.IndexOf(s, startIndex);
            var result13 = text.IndexOf(s, startIndex, comparison);

            var result21 = text.IndexOfAny(charArray, startIndex);

            var result31 = text.PadLeft(totalWidth, c);

            var result41 = text.PadRight(totalWidth, c);

            var result51 = text.Split(';', '.');

            var result61 = text.Trim(charArray);
            var result62 = text.Trim(';', '.');

            var result71 = text.TrimEnd(charArray);
            var result72 = text.TrimEnd(';', '.');

            var result81 = text.TrimStart(charArray);
            var result82 = text.TrimStart(';', '.');
        }
    }
}