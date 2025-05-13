namespace Test
{
    public class FormatStrings
    {
        public void Int32(int number)
        {
            var result = string.Format("{0:G{caret}11}", number);
        }
    }
}