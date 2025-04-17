namespace Test
{
    public class SBytes
    {
        public void Clamp()
        {
            var result = sbyte.Clamp{caret}(1, sbyte.MinValue, sbyte.MaxValue);
        }
    }
}