namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiLetterUpper(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiLetterUpper(c);
        }
    }
}