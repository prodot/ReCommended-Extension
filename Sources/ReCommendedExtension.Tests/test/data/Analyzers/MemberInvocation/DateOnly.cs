using System;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(DateOnly dateOnly)
        {
            var result = dateOnly.AddDays(0);
        }

        public void BinaryOperator(DateOnly dateOnly, DateOnly value)
        {
            var result = dateOnly.Equals(value);
        }

        public void NoDetection(DateOnly dateOnly, int value)
        {
            var result = dateOnly.AddDays(value);

            dateOnly.AddDays(0);
            dateOnly.Equals(value);
        }
    }
}