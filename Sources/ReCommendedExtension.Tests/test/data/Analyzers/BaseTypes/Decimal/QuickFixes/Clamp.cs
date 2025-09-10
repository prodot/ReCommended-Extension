namespace Test
{
    public class Decimals
    {
        public void Clamp()
        {
            var result = decimal.Clamp{caret}(1, decimal.MinValue, decimal.MaxValue);
        }
    }
}