namespace Test
{
    public class IntPtrs
    {
        public void Clamp(nint number)
        {
            var result = nint.Clamp{caret}(number, 1, 0x01);
        }
    }
}