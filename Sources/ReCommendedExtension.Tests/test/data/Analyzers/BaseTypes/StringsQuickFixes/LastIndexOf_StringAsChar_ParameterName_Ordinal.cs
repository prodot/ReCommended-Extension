using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text)
        {
            var result = text.LastIndexOf(value: "{caret}a", StringComparison.Ordinal);
        }
    }
}