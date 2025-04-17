namespace Test
{
    public class Int64s
    {
        public void Clamp(long number)
        {
            var result = long.Clamp{caret}(number, 1, 0x01);
        }
    }
}