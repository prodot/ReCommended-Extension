using System;

namespace Test
{
    public class Halves
    {
        public void ExpressionResult(Half number)
        {
            var result = number.Equals(null);
        }

        public void NoDetection(Half number, Half obj, Half? otherObj)
        {
            var result1 = number.Equals(obj);
            var result2 = number.Equals(otherObj);

            number.Equals(null);
        }
    }
}