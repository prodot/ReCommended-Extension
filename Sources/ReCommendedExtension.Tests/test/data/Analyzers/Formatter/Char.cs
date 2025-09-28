using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantProvider(char character, IFormatProvider provider)
        {
            var result = character.ToString(provider);
        }

        public void NoDetection()
        {
        }
    }
}