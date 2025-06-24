namespace Test
{
    public class Int32s
    {
        public void Clamp(int number)
        {
            var result = 2 * int.Clamp{caret}(number + 1, int.MinValue, int.MaxValue);
        }
    }
}