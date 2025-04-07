using System;

namespace Test
{
    public class Characters
    {
        public void RedundantArgument(char character, IFormatProvider provider)
        {
            var result = character.ToString(provider);
        }

        public void NoDetection()
        {
        }
    }
}