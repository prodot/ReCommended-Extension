using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void AppendJoin(StringBuilder? builder, string s)
        {
            var result = builder?.AppendJoin{caret}(s, new object[0]);
        }
    }
}