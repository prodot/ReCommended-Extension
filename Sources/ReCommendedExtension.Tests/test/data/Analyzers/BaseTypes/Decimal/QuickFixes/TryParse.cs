namespace Test
{
    public class Decimals
    {
        public void TryParse(string s)
        {
            var result = decimal.TryParse(s, {caret}null, out _);
        }
    }
}