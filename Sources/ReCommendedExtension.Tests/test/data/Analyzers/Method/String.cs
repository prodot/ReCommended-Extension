using System;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(string text, char c)
        {
            var result11 = text.PadLeft(0);
            var result12 = text.PadLeft(0, c);

            var result21 = text.PadRight(0);
            var result22 = text.PadRight(0, c);

            var result31 = text.Replace('c', 'c');
            var result32 = text.Replace("cd", "cd");
            var result33 = text.Replace("cd", "cd", StringComparison.Ordinal);

            var result41 = text.Substring(0);
        }

        public void NoDetection(string text, int totalWidth, char c, string s, int startIndex)
        {
            var result11 = text.PadLeft(totalWidth);
            var result12 = text.PadLeft(totalWidth, c);

            var result21 = text.PadRight(totalWidth);
            var result22 = text.PadRight(totalWidth, c);

            var result31 = text.Replace('c', c);
            var result32 = text.Replace(c, 'c');
            var result33 = text.Replace(c, c);
            var result34 = text.Replace("", "");
            var result35 = text.Replace("cd", s);
            var result36 = text.Replace(s, "cd");
            var result37 = text.Replace(s, s);

            var result41 = text.Substring(startIndex);
        }
    }
}