namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int number)
        {
            var result = number.Equals(null);
        }

        public void Operator(int number, int obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(int number, int obj, int? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}