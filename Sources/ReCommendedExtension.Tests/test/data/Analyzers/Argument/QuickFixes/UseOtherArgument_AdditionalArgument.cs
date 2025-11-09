namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string text)
        {
            var result = text.IndexOf("c{caret}");
        }
    }
}