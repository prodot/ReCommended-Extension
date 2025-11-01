using System;
using static System.DateTime;

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

        public void StaticProperty()
        {
            var result1 = DateTime.Now.Date;
            var result2 = Now.Date;
        }

        public void NoDetection(DateTime dateTime, long value, TimeSpan timeSpan, DateTime dt)
        {
            var result11 = dateTime.AddTicks(value);

            var result21 = nameof(DateTime.Now.Date);
            var result22 = nameof(Now.Date);
            var result23 = dt.Date;

            dateTime.AddTicks(0);

            dateTime.Add(timeSpan);

            dateTime.Subtract(dt);
            dateTime.Subtract(timeSpan);

            dateTime.Equals(dt);
            DateTime.Equals(dateTime, dt);
        }

        public void NoDetectionWithErrors(DateTime dt)
        {
            dt.Date = dt;
        }
    }
}