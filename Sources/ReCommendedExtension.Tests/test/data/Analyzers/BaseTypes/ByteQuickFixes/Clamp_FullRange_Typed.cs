namespace Test
{
    public class Bytes
    {
        public void Clamp(byte number)
        {
            var result = byte.Clamp{caret}(number, byte.MinValue, 0xFF);
        }
    }
}