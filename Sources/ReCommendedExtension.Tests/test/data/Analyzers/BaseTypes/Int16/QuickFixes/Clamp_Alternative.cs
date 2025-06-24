namespace Test
{
    public class Int16s
    {
        public void Clamp(short number)
        {
            var result = short.Clamp{caret}(number, 1, 0x01);
        }
    }
}