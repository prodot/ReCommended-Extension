﻿using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void RedundantInvocation(StringBuilder builder, char c, string s, StringBuilder sb)
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

        public void RedundantInvocation_Nullable(StringBuilder? builder, char c, string s, StringBuilder sb)
        {
            var result1 = builder?.Append(c, 0);
            var result2 = builder?.Append(c)?.Append(c, 0);

            builder?.Append(c)?.Append(c, 0);
            builder?.Append(c, 0);
            builder?.Append(c, 0)?.Append(c);

            var result21 = builder?.Append(null as char[]);
            var result22 = builder?.Append((char[]?)null);
            var result23 = builder?.Append((char[])[]);
            var result24 = builder?.Append(new char[0]);
            var result25 = builder?.Append(new char[] { });
            var result26 = builder?.Append(Array.Empty<char>());

            builder?.Append(null as char[]);
            builder?.Append((char[]?)null);
            builder?.Append((char[])[]);
            builder?.Append(new char[0]);
            builder?.Append(new char[] { });
            builder?.Append(Array.Empty<char>());

            var result31 = builder?.Append(null as char[], 0, 0);
            var result32 = builder?.Append((char[]?)null, 0, 0);
            var result33 = builder?.Append([], 0, 0);
            var result34 = builder?.Append(new char[0], 0, 0);
            var result35 = builder?.Append(new char[] { }, 0, 0);
            var result36 = builder?.Append(Array.Empty<char>(), 0, 0);

            builder?.Append(null as char[], 0, 0);
            builder?.Append((char[]?)null, 0, 0);
            builder?.Append([], 0, 0);
            builder?.Append(new char[0], 0, 0);
            builder?.Append(new char[] { }, 0, 0);
            builder?.Append(Array.Empty<char>(), 0, 0);

            var result41 = builder?.Append(null as string);
            var result42 = builder?.Append("");

            builder?.Append(null as string);
            builder?.Append("");

            var result51 = builder?.Append(null as string, 0, 0);
            var result52 = builder?.Append(s, 10, 0);

            builder?.Append(null as string, 0, 0);
            builder?.Append(s, 10, 0);

            var result61 = builder?.Append(null as StringBuilder);

            builder?.Append(null as StringBuilder);

            var result71 = builder?.Append(null as StringBuilder, 0, 0);
            var result72 = builder?.Append(sb, 10, 0);

            builder?.Append(null as StringBuilder, 0, 0);
            builder?.Append(sb, 10, 0);

            var result81 = builder?.Append(null as object);

            builder?.Append(null as object);
        }

        public void RedundantArgument(StringBuilder builder, char c)
        {
            var result = builder.Append(c, 1);

            builder.Append(c, 1);
        }

        public void RedundantArgument_Nullable(StringBuilder? builder, char c)
        {
            var result = builder?.Append(c, 1);

            builder?.Append(c, 1);
        }

        public void SingleCharacter(StringBuilder builder)
        {
            var result1 = builder.Append("a");
            var result2 = builder.Append("abcde", 1, 1);

            builder.Append("a");
            builder.Append("abcde", 1, 1);
        }

        public void SingleCharacter_Nullable(StringBuilder? builder)
        {
            var result1 = builder?.Append("a");
            var result2 = builder?.Append("abcde", 1, 1);

            builder?.Append("a");
            builder?.Append("abcde", 1, 1);
        }

        public void NoDetection(StringBuilder builder, char c, int repeatCount, string s, string? sn, int startIndex, int count, StringBuilder sb, StringBuilder? sbn, object obj)
        {
            var result1 = builder.Append(c, repeatCount);

            var result2 = builder.Append([c]);

            var result31 = builder.Append([c], 0, 0);
            var result32 = builder.Append([], 1, 0);
            var result33 = builder.Append([], 0, 1);

            var result41 = builder.Append(s);

            var result51 = builder.Append(s, startIndex, 0);
            var result52 = builder.Append(s, 0, count);
            var result53 = builder.Append(s, startIndex, count);
            var result54 = builder.Append(sn, 1, 0);
            var result55 = builder.Append(s, 2, 1);
            var result56 = builder.Append("abc", startIndex, 1);
            var result57 = builder.Append("abc", 1, count);

            var result61 = builder.Append(sb);

            var result71 = builder.Append(sb, startIndex, 0);
            var result72 = builder.Append(sb, 0, count);
            var result73 = builder.Append(sb, startIndex, count);
            var result74 = builder.Append(sbn, 1, 0);
            var result75 = builder.Append(sb, 2, 1);

            var result81 = builder.Append(obj);
        }
    }
}