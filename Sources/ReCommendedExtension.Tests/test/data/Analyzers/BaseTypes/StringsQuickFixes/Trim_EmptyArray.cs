namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result = text.Trim(new {caret}char[0]);
        }
    }
}