namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(bool flag)
        {
            var result = flag.Equals(true);
        }

        public void NoDetection(bool flag, bool value)
        {
            var result = flag.Equals(value);

            flag.Equals(true);
        }
    }
}