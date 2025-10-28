using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(TimeOnly timeOnly)
        {
            var result11 = new TimeOnly(0);
            var result12 = new TimeOnly(0, 0);
            var result13 = new TimeOnly(0, 0, 0);
            var result14 = new TimeOnly(0, 0, 0, 0);
            var result15 = new TimeOnly(0, 0, 0, 0, 0);

            TimeOnly result21 = new(0);
            TimeOnly result22 = new(0, 0);
            TimeOnly result23 = new(0, 0, 0);
            TimeOnly result24 = new(0, 0, 0, 0);
            TimeOnly result25 = new(0, 0, 0, 0, 0);

            var result31 = timeOnly.Equals(null);
        }

        public void NoDetection(TimeOnly timeOnly, long ticks, int hour, int minute, int second, int millisecond, int microsecond, object obj)
        {
            var result11 = new TimeOnly(ticks);
            var result12 = new TimeOnly(hour, minute);
            var result13 = new TimeOnly(hour, minute, second);
            var result14 = new TimeOnly(hour, minute, second, millisecond);
            var result15 = new TimeOnly(hour, minute, second, millisecond, microsecond);

            var result21 = timeOnly.Equals(obj);

            new TimeOnly(0);

            timeOnly.Equals(null);
        }
    }
}