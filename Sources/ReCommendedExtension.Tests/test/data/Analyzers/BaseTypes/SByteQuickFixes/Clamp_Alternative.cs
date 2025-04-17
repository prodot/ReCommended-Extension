namespace Test
{
    public class SBytes
    {
        public void Clamp(sbyte number)
        {
            var result = sbyte.Clamp{caret}(number, 1, 0x01);
        }
    }
}