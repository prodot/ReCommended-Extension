namespace Test
{
    public class Int32s
    {
        public void Clamp(int number)
        {
            var result = int.Clamp{caret}(number, 1, 0x01);
        }
    }
}