using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArgument(string text)
        {
            var result = text.TrimStart(Array.Empty{caret}<char>());
        }
    }
}