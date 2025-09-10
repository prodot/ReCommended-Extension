using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result1 = TimeSpan.FromDays(0);
            var result2 = TimeSpan.FromDays(0, 0);
            var result3 = TimeSpan.FromDays(seconds: 0, days: 0);
        }

        public void NoDetection(int days, int hours)
        {
            var result1 = TimeSpan.FromDays(days);
            var result2 = TimeSpan.FromDays(0, hours);

            TimeSpan.FromDays(0);
            TimeSpan.FromDays(0, 0);
            TimeSpan.FromDays(seconds: 0, days: 0);
        }
    }
}