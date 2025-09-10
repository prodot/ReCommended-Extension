using System;

namespace Test
{
    public class DateTimes
    {
        public void ExpressionResult(DateTime dateTime)
        {
            var result = dateTime.GetTypeCode();
        }

        public void NoDetection(DateTime dateTime, char format, IFormatProvider provider)
        {
            dateTime.GetTypeCode();
        }
    }
}