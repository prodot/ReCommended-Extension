using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.TrimStart([]);
            var result12 = text.TrimStart(new char[0]);
            var result13 = text.TrimStart(new char[] { });
            var result14 = text.TrimStart(Array.Empty<char>());
            var result15 = text.TrimStart(null);
        }

        public void DuplicateElements(string text)
        {
            var result11 = text.TrimStart('a', 'b', 'a');
            var result12 = text.TrimStart(['a', 'b', 'a']);
            var result13 = text.TrimStart(new[] { 'a', 'b', 'a' });
        }

        public void NoDetection(string text, char c)
        {
            var result11 = text.TrimStart('a', 'b', c);
            var result12 = text.TrimStart(['a', 'b', 'c']);
            var result13 = text.TrimStart(new[] { 'a', 'b', c });
        }
    }
}