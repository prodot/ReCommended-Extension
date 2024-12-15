namespace Test
{
    public class Strings
    {
        public void Available(string? text)
        {
            var result = string.IsNull{on}OrEmpty(text);
        }

        public void Unavailable(string? text)
        {
            string.IsNull{off}OrEmpty(text);
        }
    }
