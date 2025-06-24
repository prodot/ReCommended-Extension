namespace Test
{
    public class IntPtrs
    {
        public void Equals(nint number, nint obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(nint number)
        {
            var result = nint.IsNegative(number);
        }

        public void IsPositive(nint number)
        {
            var result = nint.IsPositive(number);
        }
    }
}