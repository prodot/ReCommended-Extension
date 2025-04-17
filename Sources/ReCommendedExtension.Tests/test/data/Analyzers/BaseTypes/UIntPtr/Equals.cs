namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint number)
        {
            var result = number.Equals(null);
        }

        public void Operator(nuint number, nuint obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(nuint number, nuint obj, nuint? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}