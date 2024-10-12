using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text, char c)
        {
            var result11 = text.IndexOf(c) == 0;
            var result12 = text.IndexOf('a') == 0;

            var result21 = text.IndexOf(c) != 0;
            var result22 = text.IndexOf('a') != 0;

            var result31 = text.IndexOf(c) > -1;
            var result32 = text.IndexOf(c) != -1;
            var result33 = text.IndexOf(c) >= 0;

            var result41 = text.IndexOf(c) == -1;
            var result42 = text.IndexOf(c) < 0;
        }

        public void RedundantArguments(string text, char c)
        {
            var result1 = text.IndexOf(c, 0);
            var result2 = text.IndexOf(c, startIndex: 0);
        }

        public void NoDetection(string text, int startIndex)
        {
            var result1 = text.IndexOf(c, 1);
            var result2 = text.IndexOf(c, startIndex);
        }
    }
}