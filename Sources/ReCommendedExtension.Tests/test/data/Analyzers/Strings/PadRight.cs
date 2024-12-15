using System;

namespace Test
{
    public class Strings
    {
        public void RedundantInvocation(string text, char c, int totalWidth)
        {
            var result1 = text.PadRight(0);
            var result2 = text.PadRight(0, c);
            var result3 = text.PadRight(totalWidth, ' ');

            var result4 = text.PadRight(0, ' ');
        }

        public void NoDetection(string text, char c, int totalWidth)
        {
            var result1 = text.PadRight(1);
            var result2 = text.PadRight(1, c);
            var result3 = text.PadRight(totalWidth, 'a');

            text.PadRight(0);
            text.PadRight(0, c);
            text.PadRight(totalWidth, ' ');
        }
    }
}