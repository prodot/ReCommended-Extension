using System;

namespace Test
{
    public class Halves
    {
        public void RedundantArgument(Half number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(null as string);
            var result12 = number.ToString("");

            var result21 = number.ToString("G");
            var result22 = number.ToString("G0");

            var result31 = number.ToString(null as IFormatProvider);
            var result32 = number.ToString(null, provider);
            var result33 = number.ToString("", provider);
            var result34 = number.ToString(format, null);
            var result35 = number.ToString("", null);

            var result41 = number.ToString("G", provider);
            var result42 = number.ToString("G0", provider);
        }

        public void RedundantFormatPrecisionSpecifier(Half number, IFormatProvider provider)
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

        public void NoDetection(Half number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(format);
            var result12 = number.ToString("G2");
            var result13 = number.ToString("g");
            var result14 = number.ToString("g0");
            var result15 = number.ToString("E");
            var result16 = number.ToString("E5");
            var result17 = number.ToString("F");
            var result18 = number.ToString("F2");
            var result19 = number.ToString("R");
            var result1A = number.ToString("r");

            var result21 = number.ToString(provider);
            var result22 = number.ToString(format, provider);
            var result23 = number.ToString("G2", provider);
            var result24 = number.ToString("g", provider);
            var result25 = number.ToString("g0", provider);
            var result26 = number.ToString("E", provider);
            var result27 = number.ToString("E5", provider);
            var result28 = number.ToString("F", provider);
            var result29 = number.ToString("F2", provider);
            var result2A = number.ToString("R", provider);
            var result2B = number.ToString("r", provider);
        }
    }
}