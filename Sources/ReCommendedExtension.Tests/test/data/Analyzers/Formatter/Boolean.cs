using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantProvider(bool flag, IFormatProvider provider)
        {
            var result = flag.ToString(provider);
        }

        public void NoDetection()
        {
        }
    }
}