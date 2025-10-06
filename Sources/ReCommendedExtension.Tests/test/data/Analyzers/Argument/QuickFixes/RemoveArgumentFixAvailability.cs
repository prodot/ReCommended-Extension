using System.Text;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(StringBuilder builder, char c)
        {
            var result = builder.Append(c, 1);
        }
    }
}