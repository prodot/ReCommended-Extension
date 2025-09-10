namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.Trim(['a', 'b', 'a{caret}']);
        }
    }
}