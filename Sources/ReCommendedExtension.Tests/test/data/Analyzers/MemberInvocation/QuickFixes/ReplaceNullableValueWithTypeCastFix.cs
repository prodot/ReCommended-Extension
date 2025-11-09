namespace Test
{
    public class Properties
    {
        public void Value(int? nullable)
        {
            var result = nullable.{caret}Value;
        }
    }
}