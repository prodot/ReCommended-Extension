namespace Test
{
    public class Characters
    {
        public void CharRangePattern(char c)
        {
            const char minInclusive = 'a';
            var result = char.IsBetween(c, minInclusive, '\u0096');
        }

        public void NoDetection(char c, char minInclusive, char maxInclusive)
        {
            var result1 = char.IsBetween(c, minInclusive, 'c');
            var result2 = char.IsBetween(c, 'a', maxInclusive);

            char.IsBetween(c, 'a', 'c');
        }
    }
}