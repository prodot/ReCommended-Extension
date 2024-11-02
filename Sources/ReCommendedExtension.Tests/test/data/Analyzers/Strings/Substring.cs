using System;

namespace Test
{
    public class Strings
    {
        public void RedundantInvocation(string text)
        {
            var result = text.Substring(0);
        }

        public void Empty(string text, int startIndex)
        {
            var result = text.Substring(startIndex, 0);
        }

        public void NoDetection(string text, int startIndex, int length)
        {
            text.Substring(0);
            text.Substring(startIndex, 0);

            var result11 = text.Substring(1);
            var result12 = text.Substring(startIndex);

            var result21 = text.Substring(startIndex, 1);
            var result22 = text.Substring(startIndex, length);
        }
    }
}