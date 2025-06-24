namespace Test
{
    public class Nullables
    {
        public void HasValue((int, bool)? nullable)
        {
            var result = nullable.Has{caret}Value;
        }
    }
}