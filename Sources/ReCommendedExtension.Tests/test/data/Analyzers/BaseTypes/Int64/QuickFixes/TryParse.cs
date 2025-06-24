namespace Test
{
    public class Int64s
    {
        public void TryParse(string s)
        {
            var result = long.TryParse(s, {caret}null, out _);
        }
    }
}