using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgumentRange(string text)
        {
            var result1 = text.Replace("c", "x");
            var result2 = text.Replace("c", "x", StringComparison.Ordinal);
        }
    }
}