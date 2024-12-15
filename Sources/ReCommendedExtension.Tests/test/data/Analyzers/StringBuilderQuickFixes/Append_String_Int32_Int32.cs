using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Append(StringBuilder builder, char c)
        {
            var result = builder.Append("abcd{caret}e", 2, 1);
        }
    }
}