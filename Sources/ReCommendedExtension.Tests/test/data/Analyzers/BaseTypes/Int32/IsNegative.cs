namespace Test
{
    public class Int32s
    {
        public void Operator(int number)
        {
            var result = int.IsNegative(number);
        }

        public void NoDetection(int number)
        {
            int.IsNegative(number);
        }
    }
}