namespace Test
{
    public class Int64s
    {
        public void Clamp()
        {
            var result = long.Clamp{caret}(1, long.MinValue, long.MaxValue);
        }
    }
}