using System;

namespace Test
{
    public class Strings
    {
        public void EmptyArray(string text)
        {
            var result = text.Split('a', 'b', 'a{caret}');
        }
    }
}