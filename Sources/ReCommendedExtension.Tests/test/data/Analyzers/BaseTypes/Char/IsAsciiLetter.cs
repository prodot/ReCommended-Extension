namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            var result = char.IsAsciiLetter(c);
        }

        public void NoDetection(char c)
        {
            char.IsAsciiLetter(c);
        }
    }
}