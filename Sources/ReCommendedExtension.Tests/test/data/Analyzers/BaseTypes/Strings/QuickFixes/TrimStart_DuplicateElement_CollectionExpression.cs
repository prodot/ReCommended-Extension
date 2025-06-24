namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.TrimStart(['a', 'b', 'a{caret}']);
        }
    }
}