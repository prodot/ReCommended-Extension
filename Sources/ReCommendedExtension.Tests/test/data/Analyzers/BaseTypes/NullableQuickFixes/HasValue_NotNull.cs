namespace Test
{
    public class Nullables
    {
        public void HasValue(int? nullable)
        {
            var result = nullable.Has{caret}Value;
        }
    }
}