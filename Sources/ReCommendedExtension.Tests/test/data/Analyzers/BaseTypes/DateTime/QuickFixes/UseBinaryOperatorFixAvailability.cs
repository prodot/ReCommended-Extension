using System;

namespace Test
{
    public class DateTimes
    {
        public void Add(DateTime dateTime, TimeSpan value)
        {
            var result = dateTime.Add(value);
        }

        public void Equals(DateTime dateTime, DateTime value)
        {
            var result1 = dateTime.Equals(value);
            var result2 = DateTime.Equals(dateTime, value);
        }

        public void Subtract(DateTime dateTime, DateTime dateTimeValue, TimeSpan timeSpanValue)
        {
            var result1 = dateTime.Subtract(dateTimeValue);
            var result2 = dateTime.Subtract(timeSpanValue);
        }
    }
}