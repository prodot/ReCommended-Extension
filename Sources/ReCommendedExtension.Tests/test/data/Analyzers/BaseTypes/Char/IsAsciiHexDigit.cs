namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiHexDigit(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiHexDigit(c);
        }
    }
}