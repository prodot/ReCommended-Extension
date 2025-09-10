namespace Test
{
    public class Doubles
    {
        public void ExpressionResult(double number)
        {
            var result = number.Equals(null);
        }

        public void NoDetection(double number, double obj, double? otherObj)
        {
            var result1 = number.Equals(obj);
            var result2 = number.Equals(otherObj);

            number.Equals(null);
        }
    }
}