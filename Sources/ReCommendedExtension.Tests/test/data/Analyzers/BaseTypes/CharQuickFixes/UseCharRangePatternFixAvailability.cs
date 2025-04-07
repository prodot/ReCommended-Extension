namespace Test
{
    public class Characters
    {
        public void IsAsciiDigit(char c)
        {
            var result = char.IsAsciiDigit(c);
        }

        public void IsAsciiHexDigit(char c)
        {
            var result = char.IsAsciiHexDigit(c);
        }

        public void IsAsciiHexDigitLower(char c)
        {
            var result = char.IsAsciiHexDigitLower(c);
        }

        public void IsAsciiHexDigitUpper(char c)
        {
            var result = char.IsAsciiHexDigitUpper(c);
        }

        public void IsAsciiLetter(char c)
        {
            var result = char.IsAsciiLetter(c);
        }

        public void IsAsciiLetterLower(char c)
        {
            var result = char.IsAsciiLetterLower(c);
        }

        public void IsAsciiLetterOrDigit(char c)
        {
            var result = char.IsAsciiLetterOrDigit(c);
        }

        public void IsAsciiLetterUpper(char c)
        {
            var result = char.IsAsciiLetterUpper(c);
        }

        public void CharRangePattern(char c)
        {
            var result = char.IsBetween(c, 'a', 'c');
        }
    }
}