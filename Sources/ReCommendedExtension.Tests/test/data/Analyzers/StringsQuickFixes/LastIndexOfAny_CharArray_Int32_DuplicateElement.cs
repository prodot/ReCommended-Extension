﻿using System;

namespace Test
{
    public class Strings
    {
        public void LastIndexOfAny(string text, int startIndex)
        {
            var result = text.LastIndexOfAny(['a', '{caret}a'], startIndex);
        }
    }
}