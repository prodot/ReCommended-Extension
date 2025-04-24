namespace Test
{
    public class IntPtrs
    {
        public void Operator(nint number)
        {
            var result = nint.IsNegative(number);
        }

        public void NoDetection(nint number)
        {
            nint.IsNegative(number);
        }
    }
}