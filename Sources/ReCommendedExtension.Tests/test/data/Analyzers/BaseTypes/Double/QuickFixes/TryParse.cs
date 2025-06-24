namespace Test
{
    public class Doubles
    {
        public void TryParse(string s)
        {
            var result = double.TryParse(s, {caret}null, out _);
        }
    }
}