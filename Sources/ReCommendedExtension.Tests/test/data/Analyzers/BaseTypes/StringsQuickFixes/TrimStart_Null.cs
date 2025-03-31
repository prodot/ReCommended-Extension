namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result = text.TrimStart(nu{caret}ll);
        }
    }
}