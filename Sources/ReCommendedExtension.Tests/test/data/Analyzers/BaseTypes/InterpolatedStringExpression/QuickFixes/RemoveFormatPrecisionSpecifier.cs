namespace Test
{
    public class InterpolatedStringItems
    {
        public void Int32(int number)
        {
            var result = $"{number:E{caret}6}";
        }
    }
}