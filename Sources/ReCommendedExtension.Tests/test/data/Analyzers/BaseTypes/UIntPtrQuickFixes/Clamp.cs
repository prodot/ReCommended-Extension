namespace Test
{
    public class UIntPtrs
    {
        public void Clamp(nuint number)
        {
            var result = nuint.Clamp{caret}(number, 1, 0x01);
        }
    }
}