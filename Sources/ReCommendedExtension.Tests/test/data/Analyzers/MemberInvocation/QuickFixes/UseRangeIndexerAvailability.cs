namespace Test
{
    public class Methods
    {
        public void RangeIndexer(string text, string? textNullable, int startIndex, int count)
        {
            var result1 = text.Remove(startIndex);
            var result2 = text.Remove(1);
            var result3 = textNullable?.Remove(startIndex);
            var result4 = textNullable?.Remove(1);
            var result5 = text.Remove(0, count);
            var result6 = textNullable?.Remove(0, count);
        }
    }
}