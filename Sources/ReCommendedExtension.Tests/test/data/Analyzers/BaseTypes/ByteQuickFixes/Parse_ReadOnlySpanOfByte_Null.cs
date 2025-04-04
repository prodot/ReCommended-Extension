using System;

namespace Test
{
    public class Bytes
    {
        public void Parse(ReadOnlySpan<byte> utf8Text)
        {
            var result = byte.Parse(utf8Text, nu{caret}ll);
        }
    }
}