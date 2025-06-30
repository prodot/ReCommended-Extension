﻿using System;

namespace Test
{
    public class TimeSpans
    {
        public void ExpressionResult(TimeSpan timeSpan)
        {
            var result = timeSpan.Equals(null);
        }

        public void Operator(TimeSpan timeSpan, TimeSpan obj)
        {
            var result1 = timeSpan.Equals(obj);
            var result2 = TimeSpan.Equals(timeSpan, obj);
        }

        public void NoDetection(TimeSpan timeSpan, TimeSpan obj, TimeSpan? otherObj)
        {
            var result = timeSpan.Equals(otherObj);

            timeSpan.Equals(null);

            timeSpan.Equals(obj);
            TimeSpan.Equals(timeSpan, obj);
        }
    }
}