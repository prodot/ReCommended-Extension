using System;

namespace Test
{
    public class Strings
    {
        public void EmptyArray(string text, StringSplitOptions options)
        {
            var result = text.Split(new[] { "a", "a{caret}" }, options);
        }
    }
}