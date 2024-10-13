using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text)
        {
            var result = text.IndexOf(value: "{caret}a", StringComparison.CurrentCulture);
        }
    }
}