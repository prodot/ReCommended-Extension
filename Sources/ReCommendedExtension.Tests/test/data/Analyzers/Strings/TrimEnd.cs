using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.TrimEnd([]);
            var result12 = text.TrimEnd(new char[0]);
            var result13 = text.TrimEnd(new char[] { });
            var result14 = text.TrimEnd(Array.Empty<char>());
            var result15 = text.TrimEnd(null);
        }

        public void DuplicateElements(string text)
        {
            var result11 = text.TrimEnd('a', 'b', 'a');
            var result12 = text.TrimEnd(['a', 'b', 'a']);
            var result13 = text.TrimEnd(new[] { 'a', 'b', 'a' });
        }

        public void NoDetection(string text, char c)
        {
            var result11 = text.TrimEnd('a', 'b', c);
            var result12 = text.TrimEnd(['a', 'b', 'c']);
            var result13 = text.TrimEnd(new[] { 'a', 'b', c });
        }
    }
}