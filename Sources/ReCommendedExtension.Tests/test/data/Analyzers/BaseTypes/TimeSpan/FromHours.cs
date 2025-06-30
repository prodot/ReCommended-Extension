using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult()
        {
            var result1 = TimeSpan.FromHours(0);
            var result2 = TimeSpan.FromHours(0, 0);
            var result3 = TimeSpan.FromHours(seconds: 0, hours: 0);
        }

        public void NoDetection(int hours, int minutes)
        {
            var result1 = TimeSpan.FromHours(hours);
            var result2 = TimeSpan.FromHours(0, minutes);

            TimeSpan.FromHours(0);
            TimeSpan.FromHours(0, 0);
            TimeSpan.FromHours(seconds: 0, hours: 0);
        }
    }
}