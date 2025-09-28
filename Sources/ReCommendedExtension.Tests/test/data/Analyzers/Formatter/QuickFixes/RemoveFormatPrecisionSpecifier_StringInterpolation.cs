namespace Test
{
    public class Formatters
    {
        public void Availability(int number)
        {
            var result = $"{number:E{caret}6}";
        }
    }
}