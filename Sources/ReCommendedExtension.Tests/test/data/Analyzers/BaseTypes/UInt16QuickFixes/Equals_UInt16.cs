namespace Test
{
    public class UInt16s
    {
        public void Equals(ushort number, ushort obj)
        {
            var result = number.Equal{caret}s(obj);
        }
    }
}