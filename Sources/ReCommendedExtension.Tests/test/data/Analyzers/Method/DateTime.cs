using System;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(DateTime dateTime)
        {
            var result = dateTime.AddTicks(0);
        }

        public void NoDetection(DateTime dateTime, long value)
        {
            var result = dateTime.AddTicks(value);

            dateTime.AddTicks(0);
        }
    }
}