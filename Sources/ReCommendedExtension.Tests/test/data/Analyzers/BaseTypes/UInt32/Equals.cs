namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint number)
        {
            var result = number.Equals(null);
        }

        public void Operator(uint number, uint obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(uint number, uint obj, uint? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}