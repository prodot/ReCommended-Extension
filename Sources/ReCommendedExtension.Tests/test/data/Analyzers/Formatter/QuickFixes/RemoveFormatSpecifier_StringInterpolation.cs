namespace Test
{
    public class Formatters
    {
        public void Availability(int number)
        {
            var result = $"{number:{caret}G}";
        }
    }
}