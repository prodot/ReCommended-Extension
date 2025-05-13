namespace Test
{
    public class FormatStrings
    {
        public void Double(double number)
        {
            var result = string.Format("{0:R{caret}}", number);
        }
    }
}