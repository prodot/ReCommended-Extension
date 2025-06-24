namespace Test
{
    public class UInt32s
    {
        public void Equals(uint number, uint obj)
        {
            var result = number.Equal{caret}s(obj);
        }
    }
}