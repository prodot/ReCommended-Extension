using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string text)
        {
            var result = text.Replace(comparisonType: StringComparison.Ordinal, newValue: "x", oldValue: "c"{caret});
        }
    }
}