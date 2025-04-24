namespace Test
{
    public class Int64s
    {
        public void IsPositive(int number)
        {
            var result = long.Is{caret}Positive(number % 2 == 0 ? number / 2 : number * 3 + 1);
        }
    }
}