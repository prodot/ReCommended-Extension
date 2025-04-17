namespace Test
{
    public class UInt64s
    {
        public void Clamp(ulong number)
        {
            var result = ulong.Clamp{caret}(number, 1, 0x01);
        }
    }
}