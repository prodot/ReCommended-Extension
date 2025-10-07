using System.Text;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(StringBuilder builder, char c)
        {
            var result1 = builder.Append(c, 1);
            var result2 = builder.Append(repeatCount: 1, value: c);
        }
    }
}