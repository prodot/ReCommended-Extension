using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string text)
        {
            var result = text.LastIndexOf("c{caret}", comparisonType: StringComparison.Ordinal);
        }
    }
}