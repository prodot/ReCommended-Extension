using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void AppendJoin(StringBuilder? builder, string?[] stringItems)
        {
            var result = builder?.AppendJoin(",{caret}", stringItems);
        }
    }
}