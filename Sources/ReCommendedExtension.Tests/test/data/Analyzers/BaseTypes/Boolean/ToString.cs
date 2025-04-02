using System;

namespace Test
{
    public class Booleans
    {
        public void ExpressionResult(bool flag, IFormatProvider provider)
        {
            var result = flag.ToString(provider);
        }

        public void NoDetection()
        {
        }
    }
}