namespace Test
{
    public class Methods
    {
        public void OtherMethodInvocation(string text, string textNullable, char c, int startIndex, int count)
        {
            var result11 = text.IndexOf(c) != -1;
            var result12 = -1 != text.IndexOf(c);

            var result21 = text.IndexOf(c) == -1;
            var result22 = -1 == text.IndexOf(c);

            var result31 = textNullable?.IndexOfAny([c], startIndex, count);
        }
    }
}