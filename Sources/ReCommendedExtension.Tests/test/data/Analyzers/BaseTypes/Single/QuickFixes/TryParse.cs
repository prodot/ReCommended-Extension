namespace Test
{
    public class Singles
    {
        public void TryParse(string s)
        {
            var result = float.TryParse(s, {caret}null, out _);
        }
    }
}