namespace Test
{
    public class UIntPtrs
    {
        public void TryParse(string s)
        {
            var result = nuint.TryParse(s, {caret}null, out _);
        }
    }
}