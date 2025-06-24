namespace Test
{
    public class UInt16s
    {
        public void Clamp(ushort number)
        {
            var result = ushort.Clamp{caret}(number, 1, 0x01);
        }
    }
}