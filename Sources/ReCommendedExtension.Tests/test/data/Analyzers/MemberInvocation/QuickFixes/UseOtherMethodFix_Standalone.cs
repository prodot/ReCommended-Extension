namespace Test
{
    public class Methods
    {
        public void OtherMethodInvocation(string text, char c, int startIndex)
        {
            var result = text.IndexOfAny([c], {caret}startIndex);
        }
    }
}