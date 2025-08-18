using System;

namespace Test
{
    public class DatesOnly
    {
        public void ExpressionResult(DateOnly dateOnly)
        {
            var result = dateOnly.Equals(null);
        }

        public void Operator(DateOnly dateOnly, DateOnly value)
        {
            var result = dateOnly.Equals(value);
        }

        public void NoDetection(DateOnly dateOnly, DateOnly value, DateOnly? otherValue)
        {
            var result = dateOnly.Equals(otherValue);

            dateOnly.Equals(null);

            dateOnly.Equals(value);
        }
    }
}