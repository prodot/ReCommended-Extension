﻿using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result = text.TrimEnd(Array.Empty{caret}<char>());
        }
    }
}