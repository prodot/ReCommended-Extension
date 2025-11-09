using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(TimeSpan timeSpan)
        {
            var result11 = new TimeSpan(0);
            var result12 = new TimeSpan(long.MaxValue);
            var result13 = new TimeSpan(long.MinValue);
            var result14 = new TimeSpan(0, 0, 0);
            var result15 = new TimeSpan(0, 0, 0, 0);
            var result16 = new TimeSpan(0, 0, 0, 0, 0);
            var result17 = new TimeSpan(0, 0, 0, 0, 0, 0);

            TimeSpan result21 = new(0);
            TimeSpan result22 = new(9223372036854775807);
            TimeSpan result23 = new(-9223372036854775808);
            TimeSpan result24 = new(0, 0, 0);
            TimeSpan result25 = new(0, 0, 0, 0);
            TimeSpan result26 = new(0, 0, 0, 0, 0);
            TimeSpan result27 = new(0, 0, 0, 0, 0, 0);

            var result31 = timeSpan.Equals(null);

            var result41 = TimeSpan.FromDays(0);
            var result42 = TimeSpan.FromDays(0, 0);
            var result43 = TimeSpan.FromDays(seconds: 0, days: 0);

            var result51 = TimeSpan.FromHours(0);
            var result52 = TimeSpan.FromHours(0, 0);
            var result53 = TimeSpan.FromHours(seconds: 0, hours: 0);

            var result61 = TimeSpan.FromMicroseconds(0);

            var result71 = TimeSpan.FromMilliseconds(0);

            var result81 = TimeSpan.FromMinutes(0);
            var result82 = TimeSpan.FromMinutes(0, 0);
            var result83 = TimeSpan.FromMinutes(seconds: 0, minutes: 0);

            var result91 = TimeSpan.FromSeconds(0);
            var result92 = TimeSpan.FromSeconds(0, 0);
            var result93 = TimeSpan.FromSeconds(microseconds: 0, seconds: 0);

            var resultA1 = TimeSpan.FromTicks(0);
            var resultA2 = TimeSpan.FromTicks(long.MaxValue);
            var resultA3 = TimeSpan.FromTicks(long.MinValue);
        }

        public void NoDetection(TimeSpan timeSpan, long ticks, object obj)
        {
            var result11 = new TimeSpan(ticks);
            var result14 = new TimeSpan(0, 0, 1);
            var result15 = new TimeSpan(0, 0, 0, 1);
            var result16 = new TimeSpan(0, 0, 0, 0, 1);
            var result17 = new TimeSpan(0, 0, 0, 0, 0, 1);

            var result21 = timeSpan.Equals(obj);

            var result31 = TimeSpan.FromDays(1);
            var result32 = TimeSpan.FromDays(0, 1);
            var result33 = TimeSpan.FromDays(seconds: 1, days: 1);

            var result41 = TimeSpan.FromHours(1);
            var result42 = TimeSpan.FromHours(0, 1);
            var result43 = TimeSpan.FromHours(seconds: 1, hours: 1);

            var result51 = TimeSpan.FromMicroseconds(1);

            var result61 = TimeSpan.FromMilliseconds(1);

            var result71 = TimeSpan.FromMinutes(1);
            var result72 = TimeSpan.FromMinutes(0, 1);
            var result73 = TimeSpan.FromMinutes(seconds: 1, minutes: 1);

            var result81 = TimeSpan.FromSeconds(1);
            var result82 = TimeSpan.FromSeconds(0, 1);
            var result83 = TimeSpan.FromSeconds(microseconds: 1, seconds: 1);

            var resultA1 = TimeSpan.FromTicks(ticks);

            new TimeSpan(0);
            new TimeSpan(long.MaxValue);
            new TimeSpan(long.MinValue);
            new TimeSpan(0, 0, 0);
            new TimeSpan(0, 0, 0, 0);
            new TimeSpan(0, 0, 0, 0, 0);
            new TimeSpan(0, 0, 0, 0, 0, 0);

            timeSpan.Equals(null);

            TimeSpan.FromDays(0);
            TimeSpan.FromDays(0, 0);
            TimeSpan.FromDays(seconds: 0, days: 0);

            TimeSpan.FromHours(0);
            TimeSpan.FromHours(0, 0);
            TimeSpan.FromHours(seconds: 0, hours: 0);

            TimeSpan.FromMicroseconds(0);

            TimeSpan.FromMilliseconds(0);

            TimeSpan.FromMinutes(0);
            TimeSpan.FromMinutes(0, 0);
            TimeSpan.FromMinutes(seconds: 0, minutes: 0);

            TimeSpan.FromSeconds(0);
            TimeSpan.FromSeconds(0, 0);
            TimeSpan.FromSeconds(microseconds: 0, seconds: 0);

            TimeSpan.FromTicks(0);
            TimeSpan.FromTicks(long.MaxValue);
            TimeSpan.FromTicks(long.MinValue);
        }
    }
}