﻿using System;

namespace Test
{
    public class Strings
    {
        public void PadLeft(string text, char c, string s)
        {
            var result1 = text.PadLeft(0);
            var result2 = text.PadLeft(0, c);
        }
    }
}