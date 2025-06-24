namespace Test
{
    public class IntPtrs
    {
        public void IsPositive(nint number)
        {
            var result = !nint.Is{caret}Positive(number % 2 == 0 ? number / 2 : number * 3 + 1);
        }
    }
}