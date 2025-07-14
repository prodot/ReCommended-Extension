using System;

namespace Test
{
    public class TimeSpans
    {
        public void _Constructors()
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

        public void Equals(TimeSpan timeSpan)
        {
            var result = timeSpan.Equals(null);
        }

        public void FromDays()
        {
            var result1 = TimeSpan.FromDays(0);
            var result2 = TimeSpan.FromDays(0, 0);
            var result3 = TimeSpan.FromDays(seconds: 0, days: 0);
        }

        public void FromHours()
        {
            var result1 = TimeSpan.FromHours(0);
            var result2 = TimeSpan.FromHours(0, 0);
            var result3 = TimeSpan.FromHours(seconds: 0, hours: 0);
        }

        public void FromMicroseconds()
        {
            var result = TimeSpan.FromMicroseconds(0);
        }

        public void FromMilliseconds()
        {
            var result1 = TimeSpan.FromMilliseconds(0);
            var result2 = TimeSpan.FromMilliseconds(microseconds: 0, milliseconds: 0);
        }

        public void FromMinutes()
        {
            var result1 = TimeSpan.FromMinutes(0);
            var result2 = TimeSpan.FromMinutes(0, 0);
            var result3 = TimeSpan.FromMinutes(seconds: 0, minutes: 0);
        }

        public void FromSeconds()
        {
            var result1 = TimeSpan.FromSeconds(0);
            var result2 = TimeSpan.FromSeconds(0, 0);
            var result3 = TimeSpan.FromSeconds(microseconds: 0, seconds: 0);
        }

        public void FromTicks()
        {
            var result1 = TimeSpan.FromTicks(0);
            var result2 = TimeSpan.FromTicks(long.MinValue);
            var result3 = TimeSpan.FromTicks(long.MaxValue);
        }
    }
}