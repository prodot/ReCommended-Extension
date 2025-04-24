namespace Test
{
    public class IntPtrs
    {
        public void Operator(nint number)
        {
            var result = nint.IsPositive(number);
        }

        public void NoDetection(nint number)
        {
            nint.IsPositive(number);
        }
    }
}