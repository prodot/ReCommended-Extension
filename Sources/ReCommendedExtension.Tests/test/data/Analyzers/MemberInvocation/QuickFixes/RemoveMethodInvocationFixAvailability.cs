using System;
using System.Text;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(bool flag, StringBuilder builder, char c)
        {
            var result11 = flag.Equals(true);

            var result21 = builder.AppendJoin(c, Array.Empty<int>());

            builder.AppendJoin(c, Array.Empty<int>()).Append("s");
            builder.AppendJoin(c, Array.Empty<int>());
        }
    }
}