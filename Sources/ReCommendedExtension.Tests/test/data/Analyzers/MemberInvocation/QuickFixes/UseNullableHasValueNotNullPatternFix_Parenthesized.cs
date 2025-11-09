namespace Test
{
    public class Properties
    {
        public void HasValue(int? nullable)
        {
            var result = nullable.Has{caret}Value.ToString();
        }
    }
}