﻿using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void Replace(StringBuilder builder)
        {
            var result = builder.Replace("a", "{caret}b");
        }
    }
}