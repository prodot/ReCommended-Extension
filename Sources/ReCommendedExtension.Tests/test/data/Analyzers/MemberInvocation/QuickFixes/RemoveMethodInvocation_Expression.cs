namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(bool flag)
        {
            var result = flag.Equals{caret}(true);
        }
    }
}