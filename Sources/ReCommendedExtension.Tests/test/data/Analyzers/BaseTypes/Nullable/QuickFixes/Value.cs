namespace Test
{
    public class Nullables
    {
        public void Value(int? nullable)
        {
            var result = nullable.{caret}Value;
        }
    }
}