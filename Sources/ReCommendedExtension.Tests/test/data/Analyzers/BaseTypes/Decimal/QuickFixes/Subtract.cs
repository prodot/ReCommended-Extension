namespace Test
{
    public class Decimals
    {
        public void Subtract(decimal d1, decimal d2)
        {
            var result = decimal.Subtract{caret}(d1 * 2, d2 + 1) * 2;
        }
    }
}