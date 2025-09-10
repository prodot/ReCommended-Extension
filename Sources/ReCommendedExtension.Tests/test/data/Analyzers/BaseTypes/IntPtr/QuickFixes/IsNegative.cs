namespace Test
{
    public class IntPtrs
    {
        public void IsNegative(nint number)
        {
            var result = !nint.Is{caret}Negative(number % 2 == 0 ? number / 2 : number * 3 + 1);
        }
    }
}