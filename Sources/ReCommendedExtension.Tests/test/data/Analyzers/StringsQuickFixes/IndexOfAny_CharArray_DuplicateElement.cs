﻿using System;

namespace Test
{
    public class Strings
    {
        public void IndexOfAny(string text)
        {
            var result = text.IndexOfAny(['a', '{caret}a']);
        }
    }
}