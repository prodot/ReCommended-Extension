﻿using System;

namespace Test
{
    public class UInt128s
    {
        public void ExpressionResult(UInt128 number)
        {
            var result = number.Equals(null);
        }

        public void Operator(UInt128 number, UInt128 obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(UInt128 number, UInt128 obj, UInt128? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}