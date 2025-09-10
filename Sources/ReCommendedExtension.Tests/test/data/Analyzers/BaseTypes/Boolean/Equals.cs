namespace Test
{
    public class Booleans
    {
        public void ExpressionResult(bool flag, bool obj)
        {
            var result11 = flag.Equals(false);
            var result12 = false.Equals(obj);

            var result21 = flag.Equals(null);
        }

        public void RedundantMethodInvocation(bool flag, bool obj)
        {
            var result1 = flag.Equals(true);
            var result2 = true.Equals(obj);
        }

        public void Operator(bool flag, bool obj)
        {
            var result = flag.Equals(obj);
        }

        public void NoDetection(bool flag, bool obj, bool? otherObj)
        {
            var result = flag.Equals(otherObj);

            flag.Equals(null);

            flag.Equals(true);
            flag.Equals(false);
            true.Equals(obj);
            false.Equals(obj);

            flag.Equals(obj);
        }
    }
}