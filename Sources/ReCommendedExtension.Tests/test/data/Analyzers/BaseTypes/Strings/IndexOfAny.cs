using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArguments(string text, char[] c, string s)
        {
            var result1 = text.IndexOfAny(c, 0);
            var result2 = text.IndexOfAny(c, startIndex: 0);
        }

        public void RedundantArguments_Nullable(string? text, char[] c, string s)
        {
            var result1 = text?.IndexOfAny(c, 0);
            var result2 = text?.IndexOfAny(c, startIndex: 0);
        }

        public void ExpressionResult(string text)
        {
            var result = text.IndexOfAny([]);
        }

        public void DuplicateItems(string text, int startIndex, int count)
        {
            var result1 = text.IndexOfAny(['a', 'a']);
            var result2 = text.IndexOfAny(['a', 'a'], startIndex);
            var result3 = text.IndexOfAny(['a', 'a'], startIndex, count);
        }

        public void DuplicateItems_Nullable(string? text, int startIndex, int count)
        {
            var result1 = text?.IndexOfAny(['a', 'a']);
            var result2 = text?.IndexOfAny(['a', 'a'], startIndex);
            var result3 = text?.IndexOfAny(['a', 'a'], startIndex, count);
        }

        public void OtherMethod(string text, char c, int startIndex, int count)
        {
            var result1 = text.IndexOfAny([c]);
            var result2 = text.IndexOfAny([c], startIndex);
            var result3 = text.IndexOfAny([c], startIndex, count);
        }

        public void OtherMethod_Nullable(string? text, char c, int startIndex, int count)
        {
            var result1 = text?.IndexOfAny([c]);
            var result2 = text?.IndexOfAny([c], startIndex);
            var result3 = text?.IndexOfAny([c], startIndex, count);
        }

        public void NoDetection(string text, char[] c, int startIndex)
        {
            var result11 = text.IndexOfAny([c, c]);

            var result21 = text.IndexOfAny(c, 1);
            var result22 = text.IndexOfAny(c, startIndex);

            text.IndexOfAny([]);
        }

        public void NoDetection_Nullable(string? text, char[] c, int startIndex)
        {
            var result1 = text?.IndexOfAny([]);
        }
    }
}