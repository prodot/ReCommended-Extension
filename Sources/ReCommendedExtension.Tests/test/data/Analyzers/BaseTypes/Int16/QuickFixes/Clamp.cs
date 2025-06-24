namespace Test
{
    public class Int16s
    {
        public void Clamp()
        {
            var result = short.Clamp{caret}(1, short.MinValue, short.MaxValue);
        }
    }
}