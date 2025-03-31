using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.Trim([]);
            var result12 = text.Trim(new char[0]);
            var result13 = text.Trim(new char[] { });
            var result14 = text.Trim(Array.Empty<char>());
            var result15 = text.Trim(null);
        }

        public void DuplicateElements(string text)
        {
            var result11 = text.Trim('a', 'b', 'a');
            var result12 = text.Trim(['a', 'b', 'a']);
            var result13 = text.Trim(new[] { 'a', 'b', 'a' });
        }

        public void NoDetection(string text, char c)
        {
            var result11 = text.Trim('a', 'b', c);
            var result12 = text.Trim(['a', 'b', 'c']);
            var result13 = text.Trim(new[] { 'a', 'b', c });
        }
    }
}