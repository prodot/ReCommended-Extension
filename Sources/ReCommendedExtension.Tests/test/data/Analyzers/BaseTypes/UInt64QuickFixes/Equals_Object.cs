namespace Test
{
    public class UInt64s
    {
        public void Equals(ulong number)
        {
            var result = number.Equal{caret}s(null);
        }
    }
}