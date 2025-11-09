namespace Test
{
    public class Methods
    {
        public void RangeIndexer(string text, int count)
        {
            var result = text.Remove(0, {caret}count + 1);
        }
    }
}