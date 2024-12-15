using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Append(StringBuilder? builder, char c)
        {
            var result = builder?.Append{caret}(c, 0);
        }
    }
}