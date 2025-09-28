namespace Test
{
    public class Formatters
    {
        public void Availability(double number)
        {
            var result = string.Format("{0:R{caret}}", number);
        }
    }
}