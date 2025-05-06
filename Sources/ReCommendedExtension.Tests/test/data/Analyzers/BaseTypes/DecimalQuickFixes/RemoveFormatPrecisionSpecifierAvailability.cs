using System;
using System.Globalization;

namespace Test
{
    public class Decimals
    {
        public void ToString(decimal number, IFormatProvider provider)
        {
            var result11 = number.ToString("E6");
            var result12 = number.ToString("e6");

            var result21 = number.ToString("E6", provider);
            var result22 = number.ToString("e6", provider);

            const string formatSpecifier = "E6";

            var result31 = number.ToString(formatSpecifier);
            var result32 = number.ToString(formatSpecifier, provider);

            var result41 = number.ToString(@"E6");
            var result42 = number.ToString($@"E6");
            var result43 = number.ToString(@$"E6");
            var result44 = number.ToString("""E06""");
            var result45 = number.ToString($"""E06""");
            var result46 = number.ToString($$"""E06""");
            var result47 = number.ToString("""
                                           E006
                                           """);
        }
    }
}