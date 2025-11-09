using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(DateTime dateTime)
        {
            var result1 = new DateTime(0);

            DateTime result2 = new(0);

            var result3 = dateTime.Equals(null);

            var result4 = dateTime.GetTypeCode();
        }

        public void NoDetection(DateTime dateTime, long ticks, object obj)
        {
            var result1 = new DateTime(ticks);

            var result = dateTime.Equals(obj);

            new DateTime(0);

            dateTime.Equals(null);

            dateTime.GetTypeCode();
        }
    }
}