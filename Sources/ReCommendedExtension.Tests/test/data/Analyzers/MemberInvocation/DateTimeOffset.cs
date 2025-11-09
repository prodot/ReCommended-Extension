using System;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(DateTimeOffset dateTimeOffset)
        {
            var result = dateTimeOffset.AddTicks(0);
        }

        public void BinaryOperator(DateTimeOffset dateTimeOffset, TimeSpan timeSpan, DateTimeOffset dto)
        {
            var result11 = dateTimeOffset.Add(timeSpan);

            var result21 = dateTimeOffset.Subtract(dto);
            var result22 = dateTimeOffset.Subtract(timeSpan);

            var result31 = dateTimeOffset.Equals(dto);
            var result32 = DateTimeOffset.Equals(dateTimeOffset, dto);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, long value, TimeSpan timeSpan, DateTimeOffset dto)
        {
            var result = dateTimeOffset.AddTicks(value);

            dateTimeOffset.AddTicks(0);

            dateTimeOffset.Add(timeSpan);

            dateTimeOffset.Subtract(dto);
            dateTimeOffset.Subtract(timeSpan);

            dateTimeOffset.Equals(dto);
            DateTimeOffset.Equals(dateTimeOffset, dto);
        }
    }
}