namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint number)
        {
            var result = number.Equals(null);
        }

        public void Operator(nint number, nint obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(nint number, nint obj, nint? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}