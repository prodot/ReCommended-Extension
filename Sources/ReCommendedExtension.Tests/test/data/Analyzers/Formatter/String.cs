using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantProvider(string text, IFormatProvider provider)
        {
            var result1 = text.ToString(provider);
            var result2 = text.ToString(null);
        }
    }
}