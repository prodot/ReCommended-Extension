using System;

namespace Test
{
    public class DateTimes
    {
        public void _Constructors(long ticks)
        {
            var result = new DateTime(ticks, DateTimeKind.{caret}Unspecified);
        }
    }
}