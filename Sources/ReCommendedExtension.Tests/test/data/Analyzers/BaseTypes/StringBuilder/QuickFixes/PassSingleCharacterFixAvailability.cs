using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Append(StringBuilder builder)
        {
            var result1 = builder.Append("a");
            var result2 = builder.Append("abcde", 1, 1);

            builder.Append("a");
            builder.Append("abcde", 1, 1);
        }

        public void AppendJoin(StringBuilder builder, object?[] objectItems, int[] intItems, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings)
        {
            var result1 = builder.AppendJoin(",", objectItems);
            var result2 = builder.AppendJoin(",", intItems);
            var result3 = builder.AppendJoin(",", stringItems);
            var result4 = builder.AppendJoin(",", spanOfObjects);
            var result5 = builder.AppendJoin(",", spanOfStrings);

            builder.AppendJoin(",", objectItems);
            builder.AppendJoin(",", intItems);
            builder.AppendJoin(",", stringItems);
            builder.AppendJoin(",", spanOfObjects);
            builder.AppendJoin(",", spanOfStrings);
        }

        public void Insert(StringBuilder builder, int index)
        {
            var result1 = builder.Insert(index, "a", 1);
            var result2 = builder.Insert(index, "a");

            builder.Insert(index, "a", 1);
            builder.Insert(index, "a");
        }

        public void Replace(StringBuilder builder, int startIndex, int count)
        {
            var result1 = builder.Replace("a", "b");
            var result2 = builder.Replace("a", "b", startIndex, count);

            builder.Replace("a", "b");
            builder.Replace("a", "b", startIndex, count);
        }
    }
}