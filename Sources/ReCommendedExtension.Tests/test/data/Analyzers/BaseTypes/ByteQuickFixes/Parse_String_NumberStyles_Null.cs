using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void Parse(string s, NumberStyles style)
        {
            var result = byte.Parse(s, style, nu{caret}ll);
        }
    }
}