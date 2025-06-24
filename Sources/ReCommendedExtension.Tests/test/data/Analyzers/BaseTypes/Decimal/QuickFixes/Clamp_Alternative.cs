namespace Test
{
    public class Decimals
    {
        public void Clamp(decimal number)
        {
            var result = decimal.Clamp{caret}(number, 1, 0x01);
        }
    }
}