namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort number)
        {
            var result = number.Equals(null);
        }

        public void Operator(ushort number, ushort obj)
        {
            var result = number.Equals(obj);
        }

        public void NoDetection(ushort number, ushort obj, ushort? otherObj)
        {
            var result = number.Equals(otherObj);

            number.Equals(null);

            number.Equals(obj);
        }
    }
}