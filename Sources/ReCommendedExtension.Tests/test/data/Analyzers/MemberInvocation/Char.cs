namespace Test
{
    public class Methods
    {
        public void BinaryOperator(char c, char value)
        {
            var result = c.Equals(value);
        }

        public void Pattern(char c)
        {
            var result1 = char.IsAsciiDigit(c);
            var result2 = char.IsAsciiHexDigit(c);
            var result3 = char.IsAsciiHexDigitLower(c);
            var result4 = char.IsAsciiHexDigitUpper(c);
            var result5 = char.IsAsciiLetter(c);
            var result6 = char.IsAsciiLetterLower(c);
            var result7 = char.IsAsciiLetterOrDigit(c);
            var result8 = char.IsAsciiLetterUpper(c);
            var result9 = char.IsBetween(c, 'a', 'c');
        }

        public void NoDetection(char c, char value, char minInclusive, char maxInclusive)
        {
            var result = char.IsBetween(c, minInclusive, maxInclusive);

            c.Equals(value);

            char.IsAsciiDigit(c);
            char.IsAsciiHexDigit(c);
            char.IsAsciiHexDigitLower(c);
            char.IsAsciiHexDigitUpper(c);
            char.IsAsciiLetter(c);
            char.IsAsciiLetterLower(c);
            char.IsAsciiLetterOrDigit(c);
            char.IsAsciiLetterUpper(c);
            char.IsBetween(c, 'a', 'c');
        }
    }
}