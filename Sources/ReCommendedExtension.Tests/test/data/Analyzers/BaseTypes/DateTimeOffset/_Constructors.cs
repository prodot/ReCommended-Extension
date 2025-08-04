using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void RedundantArgument(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, 0, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);

            DateTimeOffset result21 = new(year, month, day, hour, minute, second, 0, offset);
            DateTimeOffset result22 = new(year, month, day, hour, minute, second, millisecond, 0, offset);
            DateTimeOffset result23 = new(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);
        }

        public void NoDetection(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, Calendar calendar, TimeSpan offset)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, microsecond, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, microsecond, calendar, offset);
        }
    }
}