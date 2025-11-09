using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(DateTimeOffset dateTimeOffset)
        {
            var result = dateTimeOffset.Equals(null);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, object obj)
        {
            var result = dateTimeOffset.Equals(obj);

            dateTimeOffset.Equals(null);
        }
    }
}