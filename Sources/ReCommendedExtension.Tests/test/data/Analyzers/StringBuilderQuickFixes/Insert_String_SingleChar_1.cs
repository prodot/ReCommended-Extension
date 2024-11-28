using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Insert(StringBuilder builder, int index)
        {
            var result = builder.Insert(index, "a{caret}", 1);
        }
    }
}