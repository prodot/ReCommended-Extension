using System;

namespace Test
{
    public class DateTimes
    {
        public void Operator(DateTime dateTime, DateTime dateTimeValue, TimeSpan timeSpanValue)
        {
            var result1 = dateTime.Subtract(dateTimeValue);
            var result2 = dateTime.Subtract(timeSpanValue);
        }

        public void NoDetection(DateTime dateTime, DateTime dateTimeValue, TimeSpan timeSpanValue)
        {
            dateTime.Subtract(dateTimeValue);
            dateTime.Subtract(timeSpanValue);
        }
    }
}