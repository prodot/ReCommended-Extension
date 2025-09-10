namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result = number.Equals(null);
        }

        public void Operator(sbyte number, sbyte obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(sbyte number, sbyte obj, sbyte? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}