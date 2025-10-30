namespace Test
{
    public class Properties
    {
        public void HasValue((int, bool)? nullable)
        {
            var result = nullable.Has{caret}Value;
        }
    }
}