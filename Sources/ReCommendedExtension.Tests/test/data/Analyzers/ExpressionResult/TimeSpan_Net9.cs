using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult()
        {
            var result = TimeSpan.FromMilliseconds(0);
        }

        public void NoDetection()
        {
            var result = TimeSpan.FromMilliseconds(1);

            TimeSpan.FromMilliseconds(0);
        }
    }
}