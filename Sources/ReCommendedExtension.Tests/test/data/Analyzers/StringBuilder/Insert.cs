﻿using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void RedundantInvocation(StringBuilder builder, int index)
        {
            var result = builder.Insert(index, null as object);

            builder.Insert(index, null as object);
        }

        public void RedundantInvocation_Nullable(StringBuilder? builder, int index)
        {
            var result = builder?.Insert(index, null as object);

            builder?.Insert(index, null as object);
        }

        public void RedundantArgument(StringBuilder builder, int index, string s)
        {
            var result = builder.Insert(index, s, 1);

            builder.Insert(index, s, 1);
        }

        public void RedundantArgument_Nullable(StringBuilder? builder, int index, string s)
        {
            var result = builder?.Insert(index, s, 1);

            builder?.Insert(index, s, 1);
        }

        public void SingleCharacter(StringBuilder builder, int index)
        {
            var result1 = builder.Insert(index, "a", 1);
            var result2 = builder.Insert(index, "a");

            builder.Insert(index, "a", 1);
            builder.Insert(index, "a");
        }

        public void SingleCharacter_Nullable(StringBuilder? builder, int index)
        {
            var result1 = builder?.Insert(index, "a", 1);
            var result2 = builder?.Insert(index, "a");

            builder?.Insert(index, "a", 1);
            builder?.Insert(index, "a");
        }

        public void NoDetection(StringBuilder builder, int index, string s, int repeatCount, object obj)
        {
            var result11 = builder.Insert(index, s, repeatCount);
            var result12 = builder.Insert(index, "abc", repeatCount);

            var result21 = builder.Insert(index, "abc");

            var result31 = builder.Insert(index, obj);
        }
    }
}