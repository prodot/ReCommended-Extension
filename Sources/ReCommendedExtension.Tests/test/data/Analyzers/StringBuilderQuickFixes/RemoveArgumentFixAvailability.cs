using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Append(StringBuilder builder, char c)
        {
            var result = builder.Append(c, 1);

            builder.Append(c, 1);
        }

        public void Insert(StringBuilder builder, int index, string s)
        {
            var result = builder.Insert(index, s, 1);

            builder.Insert(index, s, 1);
        }
    }
}