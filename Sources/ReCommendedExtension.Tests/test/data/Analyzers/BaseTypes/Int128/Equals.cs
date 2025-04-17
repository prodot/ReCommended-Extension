using System;

namespace Test
{
    public class Int128s
    {
        public void ExpressionResult(Int128 number)
        {
            var result = number.Equals(null);
        }

        public void Operator(Int128 number, Int128 obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(Int128 number, Int128 obj, Int128? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}