namespace Test
{
    public class UInt32s
    {
        public void Clamp(uint number)
        {
            var result = uint.Clamp{caret}(number, 1, 0x01);
        }
    }
}