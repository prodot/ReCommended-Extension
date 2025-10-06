using System.Text;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(StringBuilder builder, char c, int index, string s)
        {
            var result1 = builder.Append(c, 1);

            var result2 = builder.Insert(index, s, 1);
        }

        public void NoDetection(StringBuilder builder, char c, int index, string s, int count)
        {
            var result1 = builder.Append(c, count);

            var result2 = builder.Insert(index, s, count);
        }
    }
}