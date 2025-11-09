using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string text)
        {
            var result = text.LastIndexOf(comparisonType: StringComparison.Ordinal, value: "c{caret}");
        }
    }
}