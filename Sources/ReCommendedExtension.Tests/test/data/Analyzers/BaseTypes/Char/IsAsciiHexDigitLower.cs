namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiHexDigitLower(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiHexDigitLower(c);
        }
    }
}