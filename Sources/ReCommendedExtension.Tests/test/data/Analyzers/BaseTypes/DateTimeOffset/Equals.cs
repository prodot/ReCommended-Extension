﻿using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void ExpressionResult(DateTimeOffset dateTimeOffset)
        {
            var result = dateTimeOffset.Equals(null);
        }

        public void Operator(DateTimeOffset dateTimeOffset, DateTimeOffset other)
        {
            var result1 = dateTimeOffset.Equals(other);
            var result2 = DateTimeOffset.Equals(dateTimeOffset, other);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, DateTimeOffset other, DateTimeOffset? otherValue)
        {
            var result = dateTimeOffset.Equals(otherValue);

            dateTimeOffset.Equals(null);

            dateTimeOffset.Equals(other);
            DateTimeOffset.Equals(dateTimeOffset, other);
        }
    }
}