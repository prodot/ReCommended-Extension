﻿using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text)
        {
            var result = text.LastIndexOf("{caret}a", StringComparison.Ordinal);
        }
    }
}