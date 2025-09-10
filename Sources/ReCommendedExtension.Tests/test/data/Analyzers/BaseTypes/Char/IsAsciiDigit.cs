namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiDigit(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiDigit(c);
        }
    }
}