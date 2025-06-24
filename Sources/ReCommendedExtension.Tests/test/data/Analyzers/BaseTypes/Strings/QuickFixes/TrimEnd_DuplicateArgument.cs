namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.TrimEnd('a', 'b', 'a{caret}');
        }
    }
}