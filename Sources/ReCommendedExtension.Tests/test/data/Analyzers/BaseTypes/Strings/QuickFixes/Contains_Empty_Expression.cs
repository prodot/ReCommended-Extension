using System;

namespace Test
{
    public class Strings
    {
        public void Contains(object someObject)
        {
            var result = $"{some{caret}Object}".Contains("", StringComparison.OrdinalIgnoreCase);
        }
    }
}