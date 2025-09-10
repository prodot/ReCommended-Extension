namespace Test
{
    public class UInt64s
    {
        public void Max()
        {
            var result = ulong.Max{caret}(10, 10);
        }
    }
}