using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(TimeOnly timeOnly, TimeOnly value)
        {
            var result = timeOnly.Equals(value);
        }

        public void NoDetection(TimeOnly timeOnly, TimeOnly value)
        {
            timeOnly.Equals(value);
        }
    }
}