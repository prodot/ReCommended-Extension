namespace Test
{
    public class Decimals
    {
        public void ExpressionResult(decimal number)
        {
            var result = number.Equals(null);
        }

        public void Operator(decimal number, decimal obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(decimal number, decimal obj, decimal? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}