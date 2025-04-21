using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void RedundantInvocation(StringBuilder builder)
        {
            var result1 = builder.Replace("abc", "abc");
            var result2 = builder.Replace('a', 'a');

            builder.Replace("abc", "abc");
            builder.Replace('a', 'a');
        }

        public void RedundantInvocation_Nullable(StringBuilder? builder)
        {
            var result = builder?.Replace("abc", "abc");

            builder?.Replace("abc", "abc");
        }

        public void SingleCharacter(StringBuilder builder, int startIndex, int count)
        {
            var result1 = builder.Replace("a", "b");
            var result2 = builder.Replace("a", "b", startIndex, count);

            builder.Replace("a", "b");
            builder.Replace("a", "b", startIndex, count);
        }

        public void SingleCharacter_Nullable(StringBuilder? builder, int startIndex, int count)
        {
            var result1 = builder?.Replace("a", "b");
            var result2 = builder?.Replace("a", "b", startIndex, count);

            builder?.Replace("a", "b");
            builder?.Replace("a", "b", startIndex, count);
        }

        public void NoDetection(StringBuilder builder)
        {
            var result1 = builder.Replace("ab", "abc");
            var result2 = builder.Replace('a', 'b');
        }
    }
}