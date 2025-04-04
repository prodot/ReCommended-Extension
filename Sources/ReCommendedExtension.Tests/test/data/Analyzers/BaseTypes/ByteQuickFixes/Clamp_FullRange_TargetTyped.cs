namespace Test
{
    public class Bytes
    {
        public void Clamp()
        {
            byte result = byte.Clamp{caret}(1, byte.MinValue, 0xFF);
        }
    }
}