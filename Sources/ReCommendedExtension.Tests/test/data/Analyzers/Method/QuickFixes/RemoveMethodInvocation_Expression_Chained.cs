using System;
using System.Text;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(StringBuilder builder, char c, string s)
        {
            var result = builder.AppendJoin{caret}(c, Array.Empty<int>()).Append(s);
        }
    }
}