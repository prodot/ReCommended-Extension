using System;

namespace Test
{
    public class DateTimes
    {
        public void Subtract(DateTimeOffset dateTimeOffset, DateTimeOffset dateTimeOffsetValue)
        {
            var result = dateTimeOffset.Sub{caret}tract(dateTimeOffsetValue);
        }
    }
}