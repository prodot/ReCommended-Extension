namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long number)
        {
            var result = number.Equals(null);
        }

        public void Operator(long number, long obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(long number, long obj, long? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}