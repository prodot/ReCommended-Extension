﻿namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result11 = text.Trim(new[] { 'a', 'b', 'a{caret}' });
        }
    }
}