using System;

namespace Test
{
    public class Booleans
    {
        public void RedundantArgument(bool flag, IFormatProvider provider)
        {
            var result = flag.ToString(provider);
        }

        public void NoDetection()
        {
        }
    }
}