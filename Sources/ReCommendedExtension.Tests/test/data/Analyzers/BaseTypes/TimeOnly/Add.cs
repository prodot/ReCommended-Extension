using System;

namespace Test
{
    public class TimesOnly
    {
        public void RedundantArgument(TimeOnly timeOnly, TimeSpan value)
        {
            var result1 = timeOnly.Add(value, out _);
            var result2 = timeOnly.Add(value, out int _);
        }

        public void NoDetection(TimeOnly timeOnly, TimeSpan value, out int wrappedDays)
        {
            var result1 = timeOnly.Add(value, out wrappedDays);

            int _;
            var result2 = timeOnly.Add(value, out _);
        }

        public void NoDetection(TimeOnly timeOnly, TimeSpan value)
        {
            var result = timeOnly.Add(value, out var _);
        }
    }
}