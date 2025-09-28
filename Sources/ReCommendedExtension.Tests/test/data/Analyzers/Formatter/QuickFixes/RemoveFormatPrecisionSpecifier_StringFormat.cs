namespace Test
{
    public class Formatters
    {
        public void Availability(int number)
        {
            var result = string.Format("{0:E{caret}6}", number);
        }
    }
}