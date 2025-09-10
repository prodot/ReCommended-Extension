namespace Test
{
    public class Booleans
    {
        public void Equals(bool flag, bool obj)
        {
            var result11 = flag.Equals(false);
            var result12 = false.Equals(obj);

            var result21 = flag.Equals(null);
        }

        public void GetTypeCode(bool flag)
        {
            var result = flag.GetTypeCode();
        }
    }
}