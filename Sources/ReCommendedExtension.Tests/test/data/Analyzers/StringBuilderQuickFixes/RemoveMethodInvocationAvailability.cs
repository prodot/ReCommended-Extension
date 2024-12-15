using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Append(StringBuilder builder, char c, string s, StringBuilder sb)
        {
            var result11 = builder.Append(c, 0);
            var result12 = builder.Append(c).Append(c, 0);

            builder.Append(c).Append(c, 0);
            builder.Append(c, 0);
            builder.Append(c, 0).Append(c);

            var result21 = builder.Append(null as char[]);
            var result22 = builder.Append((char[]?)null);
            var result23 = builder.Append((char[])[]);
            var result24 = builder.Append(new char[0]);
            var result25 = builder.Append(new char[] { });
            var result26 = builder.Append(Array.Empty<char>());

            builder.Append(null as char[]);
            builder.Append((char[]?)null);
            builder.Append((char[])[]);
            builder.Append(new char[0]);
            builder.Append(new char[] { });
            builder.Append(Array.Empty<char>());

            var result31 = builder.Append(null as char[], 0, 0);
            var result32 = builder.Append((char[]?)null, 0, 0);
            var result33 = builder.Append([], 0, 0);
            var result34 = builder.Append(new char[0], 0, 0);
            var result35 = builder.Append(new char[] { }, 0, 0);
            var result36 = builder.Append(Array.Empty<char>(), 0, 0);

            builder.Append(null as char[], 0, 0);
            builder.Append((char[]?)null, 0, 0);
            builder.Append([], 0, 0);
            builder.Append(new char[0], 0, 0);
            builder.Append(new char[] { }, 0, 0);
            builder.Append(Array.Empty<char>(), 0, 0);

            var result41 = builder.Append(null as string);
            var result42 = builder.Append("");

            builder.Append(null as string);
            builder.Append("");

            var result51 = builder.Append(null as string, 0, 0);
            var result52 = builder.Append(s, 10, 0);

            builder.Append(null as string, 0, 0);
            builder.Append(s, 10, 0);

            var result61 = builder.Append(null as StringBuilder);

            builder.Append(null as StringBuilder);

            var result71 = builder.Append(null as StringBuilder, 0, 0);
            var result72 = builder.Append(sb, 10, 0);

            builder.Append(null as StringBuilder, 0, 0);
            builder.Append(sb, 10, 0);

            var result81 = builder.Append(null as object);

            builder.Append(null as object);
        }

        public void AppendJoin(StringBuilder builder, string s, char c)
        {
            var result11 = builder.AppendJoin(s, (object?[])[]);
            var result12 = builder.AppendJoin(s, new object?[0]);
            var result13 = builder.AppendJoin(s, new object?[] { });
            var result14 = builder.AppendJoin(s, Array.Empty<object?>());

            var result21 = builder.AppendJoin(s, (int[])[]);
            var result22 = builder.AppendJoin(s, new int[0]);
            var result23 = builder.AppendJoin(s, new int[] { });
            var result24 = builder.AppendJoin(s, Array.Empty<int>());

            var result31 = builder.AppendJoin(s, (string?[])[]);
            var result32 = builder.AppendJoin(s, new string?[0]);
            var result33 = builder.AppendJoin(s, new string?[] { });
            var result34 = builder.AppendJoin(s, Array.Empty<string?>());

            var result41 = builder.AppendJoin(c, (object?[])[]);
            var result42 = builder.AppendJoin(c, new object?[0]);
            var result43 = builder.AppendJoin(c, new object?[] { });
            var result44 = builder.AppendJoin(c, Array.Empty<object?>());

            var result51 = builder.AppendJoin(c, (int[])[]);
            var result52 = builder.AppendJoin(c, new int[0]);
            var result53 = builder.AppendJoin(c, new int[] { });
            var result54 = builder.AppendJoin(c, Array.Empty<int>());

            var result61 = builder.AppendJoin(c, (string?[])[]);
            var result62 = builder.AppendJoin(c, new string?[0]);
            var result63 = builder.AppendJoin(c, new string?[] { });
            var result64 = builder.AppendJoin(c, Array.Empty<string?>());

            var result71 = builder.AppendJoin(s, default(ReadOnlySpan<object?>));
            var result72 = builder.AppendJoin(s, new ReadOnlySpan<object?>());

            var result81 = builder.AppendJoin(s, default(ReadOnlySpan<string?>));
            var result82 = builder.AppendJoin(s, new ReadOnlySpan<string?>());

            var result91 = builder.AppendJoin(c, default(ReadOnlySpan<object?>));
            var result92 = builder.AppendJoin(c, new ReadOnlySpan<object?>());

            var resultA1 = builder.AppendJoin(c, default(ReadOnlySpan<string?>));
            var resultA2 = builder.AppendJoin(c, new ReadOnlySpan<string?>());

            builder.AppendJoin(s, (object?[])[]);
            builder.AppendJoin(s, new object?[0]);
            builder.AppendJoin(s, new object?[] { });
            builder.AppendJoin(s, Array.Empty<object?>());

            builder.AppendJoin(s, (int[])[]);
            builder.AppendJoin(s, new int[0]);
            builder.AppendJoin(s, new int[] { });
            builder.AppendJoin(s, Array.Empty<int>());

            builder.AppendJoin(s, (string?[])[]);
            builder.AppendJoin(s, new string?[0]);
            builder.AppendJoin(s, new string?[] { });
            builder.AppendJoin(s, Array.Empty<string?>());

            builder.AppendJoin(c, (object?[])[]);
            builder.AppendJoin(c, new object?[0]);
            builder.AppendJoin(c, new object?[] { });
            builder.AppendJoin(c, Array.Empty<object?>());

            builder.AppendJoin(c, (int[])[]);
            builder.AppendJoin(c, new int[0]);
            builder.AppendJoin(c, new int[] { });
            builder.AppendJoin(c, Array.Empty<int>());

            builder.AppendJoin(c, (string?[])[]);
            builder.AppendJoin(c, new string?[0]);
            builder.AppendJoin(c, new string?[] { });
            builder.AppendJoin(c, Array.Empty<string?>());

            builder.AppendJoin(s, default(ReadOnlySpan<object?>));
            builder.AppendJoin(s, new ReadOnlySpan<object?>());

            builder.AppendJoin(s, default(ReadOnlySpan<string?>));
            builder.AppendJoin(s, new ReadOnlySpan<string?>());

            builder.AppendJoin(c, default(ReadOnlySpan<object?>));
            builder.AppendJoin(c, new ReadOnlySpan<object?>());

            builder.AppendJoin(c, default(ReadOnlySpan<string?>));
            builder.AppendJoin(c, new ReadOnlySpan<string?>());
        }

        public void Insert(StringBuilder builder, int index)
        {
            var result = builder.Insert(index, null as object);

            builder.Insert(index, null as object);
        }

        public void Replace(StringBuilder builder)
        {
            var result1 = builder.Replace("abc", "abc");
            var result2 = builder.Replace('a', 'a');

            builder.Replace("abc", "abc");
            builder.Replace('a', 'a');
        }
    }
}