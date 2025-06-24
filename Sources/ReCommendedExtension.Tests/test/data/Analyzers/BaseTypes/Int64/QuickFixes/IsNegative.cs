namespace Test
{
    public class Int64s
    {
        public void IsNegative(long number)
        {
            var result = long.Is{caret}Negative(number % 2 == 0 ? number / 2 : number * 3 + 1);
        }
    }
}