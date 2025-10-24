using System;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(DateTime dateTime)
        {
            var result = dateTime.AddTicks(0);
        }

        public void BinaryOperator(DateTime dateTime, TimeSpan timeSpan, DateTime dt)
        {
            var result11 = dateTime.Add(timeSpan);

            var result21 = dateTime.Subtract(dt);
            var result22 = dateTime.Subtract(timeSpan);

            var result31 = dateTime.Equals(dt);
            var result32 = DateTime.Equals(dateTime, dt);
        }

        public void NoDetection(DateTime dateTime, long value, TimeSpan timeSpan, DateTime dt)
        {
            var result = dateTime.AddTicks(value);

            dateTime.AddTicks(0);

            dateTime.Add(timeSpan);

            dateTime.Subtract(dt);
            dateTime.Subtract(timeSpan);

            dateTime.Equals(dt);
            DateTime.Equals(dateTime, dt);
        }
    }
}