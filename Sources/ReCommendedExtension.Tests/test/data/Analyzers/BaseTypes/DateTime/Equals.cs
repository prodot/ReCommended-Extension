﻿using System;

namespace Test
{
    public class DateTimes
    {
        public void ExpressionResult(DateTime dateTime)
        {
            var result = dateTime.Equals(null);
        }

        public void Operator(DateTime dateTime, DateTime value)
        {
            var result1 = dateTime.Equals(value);
            var result2 = DateTime.Equals(dateTime, value);
        }

        public void NoDetection(DateTime dateTime, DateTime value, DateTime? otherValue)
        {
            var result = dateTime.Equals(otherValue);

            dateTime.Equals(null);

            dateTime.Equals(value);
            DateTime.Equals(dateTime, value);
        }
    }
}