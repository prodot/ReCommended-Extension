﻿using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result = text.Trim(Array.Empty{caret}<char>());
        }
    }
}