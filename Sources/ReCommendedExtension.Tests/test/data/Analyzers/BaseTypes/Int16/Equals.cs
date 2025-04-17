namespace Test
{
    public class Int16s
    {
        public void ExpressionResult(short number)
        {
            var result = number.Equals(null);
        }

        public void Operator(short number, short obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(short number, short obj, short? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}