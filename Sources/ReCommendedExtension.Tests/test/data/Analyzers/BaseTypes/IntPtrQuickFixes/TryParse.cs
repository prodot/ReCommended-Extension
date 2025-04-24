namespace Test
{
    public class IntPtrs
    {
        public void TryParse(string s)
        {
            var result = nint.TryParse(s, {caret}null, out _);
        }
    }
}