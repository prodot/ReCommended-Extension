using System;

namespace Test
{
    public class Strings
    {
        public void RedundantInvocation(string text, char c, int totalWidth)
        {
            var result1 = text.PadLeft(0);
            var result2 = text.PadLeft(0, c);
            var result3 = text.PadLeft(totalWidth, ' ');

            var result4 = text.PadLeft(0, ' ');
        }

        public void NoDetection(string text, char c, int totalWidth)
        {
            var result1 = text.PadLeft(1);
            var result2 = text.PadLeft(1, c);
            var result3 = text.PadLeft(totalWidth, 'a');

            text.PadLeft(0);
            text.PadLeft(0, c);
            text.PadLeft(totalWidth, ' ');
        }
    }
}