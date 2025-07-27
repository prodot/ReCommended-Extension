using System;

namespace Test
{
    public class DateTimes
    {
        public void Operator(DateTime dateTime, TimeSpan value)
        {
            var result = dateTime.Add(value);
        }

        public void NoDetection(DateTime dateTime, TimeSpan value)
        {
            dateTime.Add(value);
        }
    }
}