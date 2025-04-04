namespace Test
{
    public class Bytes
    {
        public void TryParse(string s)
        {
            var result = byte.TryParse(s, null{caret}, out _);
        }
    }
}