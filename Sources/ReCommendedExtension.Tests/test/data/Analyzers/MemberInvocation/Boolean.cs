namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(bool flag)
        {
            var result = flag.Equals(true);
        }

        public void BinaryOperator(bool flag, bool value)
        {
            var result = flag.Equals(value);
        }

        public void NoDetection(bool flag, bool value)
        {
            flag.Equals(true);
            flag.Equals(value);
        }
    }
}