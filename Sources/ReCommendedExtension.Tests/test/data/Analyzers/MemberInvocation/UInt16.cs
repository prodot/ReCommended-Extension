namespace Test
{
    public class Methods
    {
        public void BinaryOperator(ushort number, ushort value)
        {
            var result = number.Equals(value);
        }

        public void NoDetection(ushort number, ushort value)
        {
            number.Equals(value);
        }
    }
}