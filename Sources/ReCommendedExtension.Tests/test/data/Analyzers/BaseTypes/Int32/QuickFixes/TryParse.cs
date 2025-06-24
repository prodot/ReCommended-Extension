namespace Test
{
    public class Int32s
    {
        public void TryParse(string s)
        {
            var result = int.TryParse(s, {caret}null, out _);
        }
    }
}