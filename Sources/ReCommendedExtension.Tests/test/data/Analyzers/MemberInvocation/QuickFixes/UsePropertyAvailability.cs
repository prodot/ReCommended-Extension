using System;

namespace Test
{
    public class Methods
    {
        public void LastIndexOf(string text, StringComparison comparisonType, object someObject)
        {
            var result1 = text.LastIndexOf("");
            var result2 = text.LastIndexOf("", comparisonType);

            var result3 = someObject.ToString().LastIndexOf("", comparisonType);
        }
    }
}