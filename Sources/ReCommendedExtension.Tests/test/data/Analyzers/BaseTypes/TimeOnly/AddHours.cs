using System;

namespace Test
{
    public class TimesOnly
    {
        public void RedundantArgument(TimeOnly timeOnly, double value)
        {
            var result1 = timeOnly.AddHours(value, out _);
            var result2 = timeOnly.AddHours(value, out int _);
        }

        public void NoDetection(TimeOnly timeOnly, double value, out int wrappedDays)
        {
            var result1 = timeOnly.AddHours(value, out wrappedDays);

            int _;
            var result2 = timeOnly.AddHours(value, out _);
        }

        public void NoDetection(TimeOnly timeOnly, double value)
        {
            var result = timeOnly.AddHours(value, out var _);
        }
    }
}