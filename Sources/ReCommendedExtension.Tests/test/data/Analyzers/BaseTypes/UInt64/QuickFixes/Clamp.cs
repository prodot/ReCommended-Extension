namespace Test
{
    public class UInt64s
    {
        public void Clamp()
        {
            var result = ulong.Clamp{caret}(1, ulong.MinValue, ulong.MaxValue);
        }
    }
}