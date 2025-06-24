namespace Test
{
    public class FormatStrings
    {
        public void Int32(int number)
        {
            var result = string.Format("{0:E{caret}6}", number);
        }
    }
}