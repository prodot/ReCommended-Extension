using System;

namespace Test
{
    public class Strings
    {
        public void EndsWith(string text, char c)
        {
            var result1 = text.EndsWith('a');
            var result2 = text.EndsWith(value: 'a');

            var result3 = text.EndsWith(c);
            var result4 = text.EndsWith(value: c);

            var result5 = text.EndsWith("a", StringComparison.Ordinal);
            var result6 = text.EndsWith("a", StringComparison.OrdinalIgnoreCase);
            var result7 = text.EndsWith("ß", StringComparison.OrdinalIgnoreCase);
        }

        public void IndexOf(string text, char c)
        {
            var result11 = text.IndexOf(c) == 0;
            var result12 = text.IndexOf('a') == 0;

            var result21 = text.IndexOf(c) != 0;
            var result22 = text.IndexOf('a') != 0;
        }
    }
}