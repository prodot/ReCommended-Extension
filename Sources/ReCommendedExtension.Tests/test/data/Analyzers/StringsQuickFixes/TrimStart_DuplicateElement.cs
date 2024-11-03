namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.TrimStart(new[] { 'a', 'b', 'a{caret}' });
        }
    }
}