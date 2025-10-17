using System;
using System.Text;
using System.Collections.Generic;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(StringBuilder builder, char c, int index, string s)
        {
            var result1 = builder.Append(c, 1);

            var result2 = builder.Insert(index, s, 1);
        }

        public void OtherArgument(StringBuilder builder, int index, IEnumerable<int> valuesGenericEnumerable, string[] valuesStringArray, object[] valuesObjectArray, ReadOnlySpan<string> valuesStrings, ReadOnlySpan<object> valuesObjects)
        {
            var result11 = builder.Append("x");

            var result21 = builder.AppendJoin("x", valuesGenericEnumerable);
            var result22 = builder.AppendJoin("x", valuesStringArray);
            var result23 = builder.AppendJoin("x", valuesObjectArray);
            var result24 = builder.AppendJoin("x", valuesStrings);
            var result25 = builder.AppendJoin("x", valuesObjects);

            var result31 = builder.Insert(index, "x");
            var result32 = builder.Insert(index, "x", 1);
        }

        public void OtherArgumentRange(StringBuilder builder, int startIndex, int count)
        {
            var result1 = builder.Replace("c", "x");
            var result2 = builder.Replace("c", "x", startIndex, count);
        }

        public void NoDetection(StringBuilder builder, char c, int index, string s, int count, IEnumerable<int> valuesGenericEnumerable, string[] valuesStringArray, object[] valuesObjectArray, ReadOnlySpan<string> valuesStrings, ReadOnlySpan<object> valuesObjects)
        {
            var result11 = builder.Append(c, count);

            var result21 = builder.Insert(index, s, count);

            var result31 = builder.Append("xx");
            var result32 = builder.Append(s);

            var result41 = builder.AppendJoin("xx", valuesGenericEnumerable);
            var result42 = builder.AppendJoin(s, valuesGenericEnumerable);
            var result43 = builder.AppendJoin("xx", valuesStringArray);
            var result44 = builder.AppendJoin(s, valuesStringArray);
            var result45 = builder.AppendJoin("xx", valuesObjectArray);
            var result46 = builder.AppendJoin(s, valuesObjectArray);
            var result47 = builder.AppendJoin("xx", valuesStrings);
            var result48 = builder.AppendJoin(s, valuesStrings);
            var result49 = builder.AppendJoin("xx", valuesObjects);
            var result4A = builder.AppendJoin(s, valuesObjects);

            var result51 = builder.Insert(index, "xx");
            var result52 = builder.Insert(index, s);
            var result53 = builder.Insert(index, "x", count);

            var result61 = builder.Replace("cc", "x");
            var result62 = builder.Replace("c", "xx");
            var result63 = builder.Replace(s, "x");
            var result64 = builder.Replace("c", s);
            var result65 = builder.Replace("cc", "x", index, count);
            var result66 = builder.Replace("c", "xx", index, count);
            var result67 = builder.Replace(s, "x", index, count);
            var result68 = builder.Replace("c", s, index, count);
        }
    }
}