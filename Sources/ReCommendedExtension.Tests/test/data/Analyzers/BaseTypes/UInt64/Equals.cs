namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong number)
        {
            var result = number.Equals(null);
        }

        public void Operator(ulong number, ulong obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(ulong number, ulong obj, ulong? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}