using System;

namespace Test
{
    public class Strings
    {
        public void RedundantInvocation(string text, IFormatProvider provider)
        {
            var result1 = text.To{caret}String(provider);
        }
    }
}