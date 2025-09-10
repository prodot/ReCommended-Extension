using System;
using System.Globalization;

namespace Test
{
    public class Halves
    {
        public void ToString(Half number, IFormatProvider provider)
        {
            var result11 = number.ToString("E6");
            var result12 = number.ToString("e6");
            var result13 = number.ToString("R3");
            var result14 = number.ToString("r3");

            var result21 = number.ToString("E6", provider);
            var result22 = number.ToString("e6", provider);
            var result23 = number.ToString("R3", provider);
            var result24 = number.ToString("r3", provider);

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