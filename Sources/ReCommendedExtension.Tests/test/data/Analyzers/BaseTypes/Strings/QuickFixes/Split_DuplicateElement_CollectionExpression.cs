using System;

namespace Test
{
    public class Strings
    {
        public void EmptyArray(string text, int count, StringSplitOptions options)
        {
            var result = text.Split(["aa", "bb", "aa{caret}"], count, options);
        }
    }
}