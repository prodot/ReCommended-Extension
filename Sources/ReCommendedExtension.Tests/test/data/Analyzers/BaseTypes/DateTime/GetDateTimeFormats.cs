using System;

namespace Test
{
    public class DateTimes
    {
        public void RedundantArgument(DateTime dateTime, char format)
        {
            var result1 = dateTime.GetDateTimeFormats(null);
            var result2 = dateTime.GetDateTimeFormats(format, null);
        }

        public void NoDetection(DateTime dateTime, char format, IFormatProvider provider)
        {
            var result1 = dateTime.GetDateTimeFormats(provider);
            var result2 = dateTime.GetDateTimeFormats(format, provider);
        }
    }
}