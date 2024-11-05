using System;

namespace Test
{
    public class Strings
    {
        public void ExpressionResult(string text)
        {
            var result = text.Remove(0);
        }

        public void RangeIndexer(string text, int startIndex, int count)
        {
            var result1 = text.Remove(startIndex);
            var result2 = text.Remove(0, count);
        }

        public void RedundantInvocation(string text, int startIndex)
        {
            var result1 = text.Remove(startIndex, 0);
        }

        public void NoDetection(string text, int startIndex, int count)
        {
            var result1 = text.Remove(1, count);
            var result2 = text.Remove(startIndex, 1);
            var result3 = text.Remove(1, count);

            text.Remove(0);
            text.Remove(startIndex);
            text.Remove(startIndex, 0);
            text.Remove(0, count);
        }

        public void NoDetection(string? text)
        {
            var result = text?.Remove(0);
        }
    }
}