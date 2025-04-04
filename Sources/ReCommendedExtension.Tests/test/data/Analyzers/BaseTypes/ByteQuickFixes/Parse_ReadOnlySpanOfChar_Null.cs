using System;

namespace Test
{
    public class Bytes
    {
        public void Parse(ReadOnlySpan<char> s)
        {
            var result = byte.Parse(s, n{caret}ull);
        }
    }
}