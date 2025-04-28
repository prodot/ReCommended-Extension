using System;
using System.Globalization;

namespace Test
{
    public class Singles
    {
        public void FloatingPointPattern(float value)
        {
            var result11 = float.IsNaN(value);
        }

        public void NoDetection(float value)
        {
            float.IsNaN(value);
        }
    }
}