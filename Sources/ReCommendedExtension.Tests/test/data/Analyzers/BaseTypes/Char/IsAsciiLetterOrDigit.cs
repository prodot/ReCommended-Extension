namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiLetterOrDigit(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiLetterOrDigit(c);
        }
    }
}