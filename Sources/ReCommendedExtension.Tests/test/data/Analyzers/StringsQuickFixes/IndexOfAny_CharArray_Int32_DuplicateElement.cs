﻿using System;

namespace Test
{
    public class Strings
    {
        public void IndexOfAny(string text, int startIndex)
        {
            var result = text.IndexOfAny(['a', '{caret}a'], startIndex);
        }
    }
}