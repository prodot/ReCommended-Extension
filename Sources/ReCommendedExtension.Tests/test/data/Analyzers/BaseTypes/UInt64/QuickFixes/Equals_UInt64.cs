namespace Test
{
    public class UInt64s
    {
        public void Equals(ulong number, ulong obj)
        {
            var result = number.Equal{caret}s(obj);
        }
    }
}