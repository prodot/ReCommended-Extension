using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(DateOnly dateOnly)
        {
            var result = dateOnly.Equals(null);
        }

        public void NoDetection(DateOnly dateOnly, object obj)
        {
            var result = dateOnly.Equals(obj);

            dateOnly.Equals(null);
        }
    }
}