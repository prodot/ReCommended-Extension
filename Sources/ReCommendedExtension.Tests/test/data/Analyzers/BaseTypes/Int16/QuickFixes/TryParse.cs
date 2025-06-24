namespace Test
{
    public class Int16s
    {
        public void TryParse(string s)
        {
            var result = short.TryParse(s, {caret}null, out _);
        }
    }
}