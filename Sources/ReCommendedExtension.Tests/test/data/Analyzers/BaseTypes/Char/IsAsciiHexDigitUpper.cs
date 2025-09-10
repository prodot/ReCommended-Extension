namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiHexDigitUpper(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiHexDigitUpper(c);
        }
    }
}