using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(StringBuilder builder, char c, string s, StringBuilder b, int startIndex, int index)
        {
            var result11 = builder.Append(null as string);
            var result12 = builder.Append("");
            var result13 = builder.Append(null as char[]);
            var result14 = builder.Append(Array.Empty<char>());
            var result15 = builder.Append(null as object);
            var result16 = builder.Append(null as StringBuilder);
            var result17 = builder.Append(c, 0);
            var result18 = builder.Append(null as string, 0, 0);
            var result19 = builder.Append(s, startIndex, 0);
            var result1A = builder.Append(null as StringBuilder, 0, 0);
            var result1B = builder.Append(b, startIndex, 0);
            var result1C = builder.Append(null as char[], 0, 0);
            var result1D = builder.Append(Array.Empty<char>(), 0, 0);

            builder.Append(null as string).Append(s);
            builder.Append(null as string);
            builder.Append("").Append(s);
            builder.Append("");
            builder.Append(null as char[]).Append(s);
            builder.Append(null as char[]);
            builder.Append(Array.Empty<char>()).Append(s);
            builder.Append(Array.Empty<char>());
            builder.Append(null as object).Append(s);
            builder.Append(null as object);
            builder.Append(null as StringBuilder).Append(s);
            builder.Append(null as StringBuilder);
            builder.Append(c, 0).Append(s);
            builder.Append(c, 0);
            builder.Append(null as string, 0, 0).Append(s);
            builder.Append(null as string, 0, 0);
            builder.Append(s, startIndex, 0).Append(s);
            builder.Append(s, startIndex, 0);
            builder.Append(null as StringBuilder, 0, 0).Append(s);
            builder.Append(null as StringBuilder, 0, 0);
            builder.Append(b, startIndex, 0).Append(s);
            builder.Append(b, startIndex, 0);
            builder.Append(null as char[], 0, 0).Append(s);
            builder.Append(null as char[], 0, 0);
            builder.Append(Array.Empty<char>(), 0, 0).Append(s);
            builder.Append(Array.Empty<char>(), 0, 0);

            var result21 = builder.AppendJoin(c, Array.Empty<int>());
            var result22 = builder.AppendJoin(c, Array.Empty<string>());
            var result23 = builder.AppendJoin(c, Array.Empty<object>());
            var result24 = builder.AppendJoin(c, default(ReadOnlySpan<string>));
            var result25 = builder.AppendJoin(c, new ReadOnlySpan<string>());
            var result26 = builder.AppendJoin(c, new ReadOnlySpan<string> { });
            var result27 = builder.AppendJoin(c, (ReadOnlySpan<string>)[]);
            var result28 = builder.AppendJoin(c, default(ReadOnlySpan<object>));
            var result29 = builder.AppendJoin(c, new ReadOnlySpan<object>());
            var result2A = builder.AppendJoin(c, new ReadOnlySpan<object> { });
            var result2B = builder.AppendJoin(c, (ReadOnlySpan<object>)[]);
            var result2C = builder.AppendJoin(s, Array.Empty<int>());
            var result2D = builder.AppendJoin(s, Array.Empty<string>());
            var result2E = builder.AppendJoin(s, Array.Empty<object>());
            var result2F = builder.AppendJoin(s, default(ReadOnlySpan<string>));
            var result2G = builder.AppendJoin(s, new ReadOnlySpan<string>());
            var result2H = builder.AppendJoin(s, new ReadOnlySpan<string> { });
            var result2I = builder.AppendJoin(s, (ReadOnlySpan<string>)[]);
            var result2J = builder.AppendJoin(s, default(ReadOnlySpan<object>));
            var result2K = builder.AppendJoin(s, new ReadOnlySpan<object>());
            var result2L = builder.AppendJoin(s, new ReadOnlySpan<object> { });
            var result2M = builder.AppendJoin(s, (ReadOnlySpan<object>)[]);

            builder.AppendJoin(c, Array.Empty<int>()).Append(s);
            builder.AppendJoin(c, Array.Empty<int>());
            builder.AppendJoin(c, Array.Empty<string>()).Append(s);
            builder.AppendJoin(c, Array.Empty<string>());
            builder.AppendJoin(c, Array.Empty<object>()).Append(s);
            builder.AppendJoin(c, Array.Empty<object>());
            builder.AppendJoin(c, default(ReadOnlySpan<string>)).Append(s);
            builder.AppendJoin(c, default(ReadOnlySpan<string>));
            builder.AppendJoin(c, new ReadOnlySpan<string>()).Append(s);
            builder.AppendJoin(c, new ReadOnlySpan<string>());
            builder.AppendJoin(c, new ReadOnlySpan<string> { }).Append(s);
            builder.AppendJoin(c, new ReadOnlySpan<string> { });
            builder.AppendJoin(c, (ReadOnlySpan<string>)[]).Append(s);
            builder.AppendJoin(c, (ReadOnlySpan<string>)[]);
            builder.AppendJoin(c, default(ReadOnlySpan<object>)).Append(s);
            builder.AppendJoin(c, default(ReadOnlySpan<object>));
            builder.AppendJoin(c, new ReadOnlySpan<object>()).Append(s);
            builder.AppendJoin(c, new ReadOnlySpan<object>());
            builder.AppendJoin(c, new ReadOnlySpan<object> { }).Append(s);
            builder.AppendJoin(c, new ReadOnlySpan<object> { });
            builder.AppendJoin(c, (ReadOnlySpan<object>)[]).Append(s);
            builder.AppendJoin(c, (ReadOnlySpan<object>)[]);
            builder.AppendJoin(s, Array.Empty<int>()).Append(s);
            builder.AppendJoin(s, Array.Empty<int>());
            builder.AppendJoin(s, Array.Empty<string>()).Append(s);
            builder.AppendJoin(s, Array.Empty<string>());
            builder.AppendJoin(s, Array.Empty<object>()).Append(s);
            builder.AppendJoin(s, Array.Empty<object>());
            builder.AppendJoin(s, default(ReadOnlySpan<string>)).Append(s);
            builder.AppendJoin(s, default(ReadOnlySpan<string>));
            builder.AppendJoin(s, new ReadOnlySpan<string>()).Append(s);
            builder.AppendJoin(s, new ReadOnlySpan<string>());
            builder.AppendJoin(s, new ReadOnlySpan<string> { }).Append(s);
            builder.AppendJoin(s, new ReadOnlySpan<string> { });
            builder.AppendJoin(s, (ReadOnlySpan<string>)[]).Append(s);
            builder.AppendJoin(s, (ReadOnlySpan<string>)[]);
            builder.AppendJoin(s, default(ReadOnlySpan<object>)).Append(s);
            builder.AppendJoin(s, default(ReadOnlySpan<object>));
            builder.AppendJoin(s, new ReadOnlySpan<object>()).Append(s);
            builder.AppendJoin(s, new ReadOnlySpan<object>());
            builder.AppendJoin(s, new ReadOnlySpan<object> { }).Append(s);
            builder.AppendJoin(s, new ReadOnlySpan<object> { });
            builder.AppendJoin(s, (ReadOnlySpan<object>)[]).Append(s);
            builder.AppendJoin(s, (ReadOnlySpan<object>)[]);

            var result31 = builder.Insert(index, null as object);

            builder.Insert(index, null as object).Append(s);
            builder.Insert(index, null as object);

            var result41 = builder.Replace('c', 'c');
            var result42 = builder.Replace("cd", "cd");

            builder.Replace('c', 'c').Append(s);
            builder.Replace('c', 'c');
            builder.Replace("cd", "cd").Append(s);
            builder.Replace("cd", "cd");
        }

        public void OtherMethodInvocation(StringBuilder builder, char c, string s, int number, object obj)
        {
            var result11 = builder.AppendJoin(c, (IEnumerable<int>)[number]);
            var result12 = builder.AppendJoin(s, (IEnumerable<int>)[number]);

            var result21 = builder.AppendJoin(c, (string[])[s]);
            var result22 = builder.AppendJoin(c, s);
            var result23 = builder.AppendJoin(s, (string[])[s]);
            var result24 = builder.AppendJoin(s, s);

            var result31 = builder.AppendJoin(c, (object[])[obj]);
            var result32 = builder.AppendJoin(c, obj);
            var result33 = builder.AppendJoin(s, (object[])[obj]);
            var result34 = builder.AppendJoin(s, obj);

            var result41 = builder.AppendJoin(c, (ReadOnlySpan<string>)[s]);
            var result42 = builder.AppendJoin(c, s);
            var result43 = builder.AppendJoin(s, (ReadOnlySpan<string>)[s]);
            var result44 = builder.AppendJoin(s, s);

            var result51 = builder.AppendJoin(c, (ReadOnlySpan<object>)[obj]);
            var result52 = builder.AppendJoin(c, obj);
            var result53 = builder.AppendJoin(s, (ReadOnlySpan<object>)[obj]);
            var result54 = builder.AppendJoin(s, obj);
        }

        public void NoDetection(StringBuilder builder, char[] chars, char c, string s, string? sNullable, object obj, StringBuilder b, StringBuilder? bNullable, int repeatCount, int startIndex, int index, int count, IEnumerable<int> numbers, string[] stringArray, object[] objectArray, ReadOnlySpan<string> strings, ReadOnlySpan<object> objects)
        {
            var result11 = builder.Append(s);
            var result12 = builder.Append(chars);
            var result13 = builder.Append(obj);
            var result14 = builder.Append(b);
            var result15 = builder.Append(c, repeatCount);
            var result16 = builder.Append(s, startIndex, count);
            var result17 = builder.Append(sNullable, startIndex, 0);
            var result18 = builder.Append(b, startIndex, count);
            var result19 = builder.Append(bNullable, startIndex, 0);
            var result1A = builder.Append(chars, startIndex, count);

            var result21 = builder.AppendJoin(c, numbers);
            var result22 = builder.AppendJoin(c, stringArray);
            var result23 = builder.AppendJoin(c, objectArray);
            var result24 = builder.AppendJoin(c, strings);
            var result25 = builder.AppendJoin(c, objects);
            var result26 = builder.AppendJoin(s, numbers);
            var result27 = builder.AppendJoin(s, stringArray);
            var result28 = builder.AppendJoin(s, objectArray);
            var result29 = builder.AppendJoin(s, strings);
            var result2A = builder.AppendJoin(s, objects);

            var result31 = builder.Insert(index, obj);

            var result41 = builder.Replace('c', c);
            var result42 = builder.Replace(c, 'c');
            var result43 = builder.Replace(c, c);
            var result44 = builder.Replace("", "");
            var result45 = builder.Replace("cd", s);
            var result46 = builder.Replace(s, "cd");
            var result47 = builder.Replace(s, s);
        }
    }
}