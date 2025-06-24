namespace Test
{
    public class UInt32s
    {
        public void Clamp()
        {
            var result = uint.Clamp{caret}(1, uint.MinValue, uint.MaxValue);
        }
    }
}