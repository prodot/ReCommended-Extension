namespace Test
{
    public class UInt16s
    {
        public void Clamp()
        {
            var result = ushort.Clamp{caret}(1, ushort.MinValue, ushort.MaxValue);
        }
    }
}