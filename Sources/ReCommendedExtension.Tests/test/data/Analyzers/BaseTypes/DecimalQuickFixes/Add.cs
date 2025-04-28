namespace Test
{
    public class Decimals
    {
        public void Add(decimal d1, decimal d2)
        {
            var result = decimal.Add{caret}(d1, d2 + 1);
        }
    }
}