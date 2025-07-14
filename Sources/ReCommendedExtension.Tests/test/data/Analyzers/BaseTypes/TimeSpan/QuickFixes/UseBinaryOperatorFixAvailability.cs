using System;

namespace Test
{
    public class TimeSpans
    {
        public void Add(TimeSpan timeSpan, TimeSpan ts)
        {
            var result = timeSpan.Add(ts);
        }

        public void Divide(TimeSpan timeSpan, double divisor, TimeSpan ts)
        {
            var result1 = timeSpan.Divide(divisor);
            var result2 = timeSpan.Divide(ts);
        }

        public void Equals(TimeSpan timeSpan, TimeSpan obj)
        {
            var result1 = timeSpan.Equals(obj);
            var result2 = TimeSpan.Equals(timeSpan, obj);
        }

        public void Multiply(TimeSpan timeSpan, double factor)
        {
            var result = timeSpan.Multiply(factor);
        }

        public void Subtract(TimeSpan timeSpan, TimeSpan ts)
        {
            var result = timeSpan.Subtract(ts);
        }
    }
}