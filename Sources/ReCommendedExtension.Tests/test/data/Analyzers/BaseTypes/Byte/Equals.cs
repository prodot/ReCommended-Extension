namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte number)
        {
            var result = number.Equals(null);
        }

        public void Operator(byte number, byte obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(byte number, byte obj, byte? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}