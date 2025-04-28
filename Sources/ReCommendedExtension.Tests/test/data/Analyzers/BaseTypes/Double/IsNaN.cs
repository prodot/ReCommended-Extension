using System;
using System.Globalization;

namespace Test
{
    public class Doubles
    {
        public void FloatingPointPattern(double value)
        {
            var result11 = double.IsNaN(value);
        }

        public void NoDetection(double value)
        {
            double.IsNaN(value);
        }
    }
}