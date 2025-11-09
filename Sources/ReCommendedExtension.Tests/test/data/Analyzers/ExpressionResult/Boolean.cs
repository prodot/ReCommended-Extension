namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(bool flag)
        {
            var result11 = flag.Equals(false);
            var result12 = true.Equals(flag);
            var result13 = false.Equals(flag);
            var result14 = flag.Equals(null);

            var result21 = flag.GetTypeCode();
        }

        public void NoDetection(bool flag, bool value, object obj)
        {
            var result1 = flag.Equals(value);
            var result2 = flag.Equals(obj);

            flag.Equals(false);
            true.Equals(flag);
            false.Equals(flag);
            flag.Equals(null);

            flag.GetTypeCode();
        }
    }
}