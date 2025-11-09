using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(TimeSpan timeSpan, TimeSpan ts, double factor)
        {
            var result11 = timeSpan.Add(ts);

            var result21 = timeSpan.Subtract(ts);

            var result31 = timeSpan.Multiply(factor);

            var result41 = timeSpan.Divide(factor);
            var result42 = timeSpan.Divide(ts);

            var result51 = timeSpan.Equals(ts);
            var result52 = TimeSpan.Equals(timeSpan, ts);
        }

        public void UnaryOperator(TimeSpan timeSpan)
        {
            var result = timeSpan.Negate();
        }

        public void NoDetection(TimeSpan timeSpan, TimeSpan ts, double factor)
        {
            timeSpan.Add(ts);

            timeSpan.Subtract(ts);

            timeSpan.Multiply(factor);

            timeSpan.Divide(factor);
            timeSpan.Divide(ts);

            timeSpan.Equals(ts);
            TimeSpan.Equals(timeSpan, ts);

            timeSpan.Negate();
        }
    }
}