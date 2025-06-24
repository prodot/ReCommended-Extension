namespace Test
{
    public class UInt32s
    {
        public void TryParse(string s)
        {
            var result = uint.TryParse(s, {caret}null, out _);
        }
    }
}