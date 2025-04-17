using System;

namespace Test
{
    public class Strings
    {
        public void ExpressionResult(string text, char[] c)
        {
            var result1 = text.LastIndexOfAny([]);
            var result2 = text.LastIndexOfAny(c, 0);
            var result3 = text.LastIndexOfAny(c, 0, 0);
            var result4 = text.LastIndexOfAny(c, 0, 1);
        }

        public void DuplicateItems(string text, int startIndex, int count)
        {
            var result1 = text.LastIndexOfAny(['a', 'a']);
            var result2 = text.LastIndexOfAny(['a', 'a'], startIndex);
            var result3 = text.LastIndexOfAny(['a', 'a'], startIndex, count);
        }

        public void DuplicateItems_Nullable(string? text, int startIndex, int count)
        {
            var result1 = text?.LastIndexOfAny(['a', 'a']);
            var result2 = text?.LastIndexOfAny(['a', 'a'], startIndex);
            var result3 = text?.LastIndexOfAny(['a', 'a'], startIndex, count);
        }

        public void OtherMethod(string text, char c, int startIndex, int count)
        {
            var result1 = text.LastIndexOfAny([c]);
            var result2 = text.LastIndexOfAny([c], startIndex);
            var result3 = text.LastIndexOfAny([c], startIndex, count);
        }

        public void OtherMethod_Nullable(string? text, char c, int startIndex, int count)
        {
            var result1 = text?.LastIndexOfAny([c]);
            var result2 = text?.LastIndexOfAny([c], startIndex);
            var result3 = text?.LastIndexOfAny([c], startIndex, count);
        }

        public void NoDetection(string text, char[] c, int startIndex)
        {
            var result11 = text.LastIndexOfAny([c, c]);

            var result21 = text.LastIndexOfAny(c, 1);
            var result22 = text.LastIndexOfAny(c, startIndex);

            text.LastIndexOfAny([]);
            text.LastIndexOfAny(c, 0);
            text.LastIndexOfAny(c, 0, 0);
            text.LastIndexOfAny(c, 0, 1);
        }

        public void NoDetection_Nullable(string? text, char[] c)
        {
            var result1 = text?.LastIndexOfAny([]);
            var result2 = text?.LastIndexOfAny(c, 0);
            var result3 = text?.LastIndexOfAny(c, 0, 0);
            var result4 = text?.LastIndexOfAny(c, 0, 1);
        }
    }
}