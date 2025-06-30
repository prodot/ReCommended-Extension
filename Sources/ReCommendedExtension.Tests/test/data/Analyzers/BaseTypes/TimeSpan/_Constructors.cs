using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result11 = new TimeSpan(0);
            var result12 = new TimeSpan(0, 0, 0);
            var result13 = new TimeSpan(0, 0, 0, 0);
            var result14 = new TimeSpan(0, 0, 0, 0, 0);
            var result15 = new TimeSpan(0, 0, 0, 0, 0, 0);

            var result21 = new TimeSpan(long.MinValue);
            var result22 = new TimeSpan(long.MaxValue);

            TimeSpan result31 = new(0);
            TimeSpan result32 = new(0, 0, 0);
            TimeSpan result33 = new(0, 0, 0, 0);
            TimeSpan result34 = new(0, 0, 0, 0, 0);
            TimeSpan result35 = new(0, 0, 0, 0, 0, 0);

            TimeSpan result41 = new(long.MinValue);
            TimeSpan result42 = new(long.MaxValue);
        }

        public void RedundantArgument(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            var result11 = new TimeSpan(0, hours, minutes, seconds);
            var result12 = new TimeSpan(days, hours, minutes, seconds, 0);
            var result13 = new TimeSpan(days, hours, minutes, seconds, milliseconds, 0);

            TimeSpan result21 = new(0, hours, minutes, seconds);
            TimeSpan result22 = new(days, hours, minutes, seconds, 0);
            TimeSpan result23 = new(days, hours, minutes, seconds, milliseconds, 0);
        }

        public void NoDetection()
        {
            new TimeSpan(0);
            new TimeSpan(0, 0, 0);
            new TimeSpan(0, 0, 0, 0);
            new TimeSpan(0, 0, 0, 0, 0, 0);

            new TimeSpan(long.MinValue);
            new TimeSpan(long.MaxValue);
        }
    }
}