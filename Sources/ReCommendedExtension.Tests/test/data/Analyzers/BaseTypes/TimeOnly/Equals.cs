using System;

namespace Test
{
    public class TimesOnly
    {
        public void ExpressionResult(TimeOnly timeOnly)
        {
            var result = timeOnly.Equals(null);
        }

        public void Operator(TimeOnly timeOnly, TimeOnly value)
        {
            var result = timeOnly.Equals(value);
        }

        public void NoDetection(TimeOnly timeOnly, TimeOnly value, TimeOnly? otherValue)
        {
            var result = timeOnly.Equals(otherValue);

            timeOnly.Equals(null);

            timeOnly.Equals(value);
        }
    }
}