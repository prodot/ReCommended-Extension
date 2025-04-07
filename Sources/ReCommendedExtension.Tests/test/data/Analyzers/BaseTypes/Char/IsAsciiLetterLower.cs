namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiLetterLower(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiLetterLower(c);
        }
    }
}