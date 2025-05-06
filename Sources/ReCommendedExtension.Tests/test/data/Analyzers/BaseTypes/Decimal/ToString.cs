using System;

namespace Test
{
    public class Decimals
    {
        public void RedundantArgument(decimal number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(null as string);
            var result12 = number.ToString("");

            var result21 = number.ToString("G");
            var result22 = number.ToString("G0");
            var result23 = number.ToString("G29");
            var result24 = number.ToString("g");
            var result25 = number.ToString("g0");
            var result26 = number.ToString("g29");

            var result31 = number.ToString(null as IFormatProvider);
            var result32 = number.ToString(null, provider);
            var result33 = number.ToString("", provider);
            var result34 = number.ToString(format, null);
            var result35 = number.ToString("", null);

            var result41 = number.ToString("G", provider);
            var result42 = number.ToString("G0", provider);
            var result43 = number.ToString("G29", provider);
            var result44 = number.ToString("g", provider);
            var result45 = number.ToString("g0", provider);
            var result46 = number.ToString("g29", provider);
        }

        public void RedundantFormatPrecisionSpecifier(decimal number, IFormatProvider provider)
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

        public void SuspiciousFormatSpecifier(decimal number, IFormatProvider provider)
        {
            var result11 = number.ToString("R");
            var result12 = number.ToString("R3");
            var result13 = number.ToString("r");
            var result14 = number.ToString("r3");

            var result21 = number.ToString("R", provider);
            var result22 = number.ToString("R3", provider);
            var result23 = number.ToString("r", provider);
            var result24 = number.ToString("r3", provider);
        }

        public void NoDetection(decimal number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(format);
            var result12 = number.ToString("G2");
            var result13 = number.ToString("E");
            var result14 = number.ToString("E5");
            var result15 = number.ToString("C");
            var result16 = number.ToString("C2");

            var result21 = number.ToString(provider);
            var result22 = number.ToString(format, provider);
            var result23 = number.ToString("G2", provider);
            var result24 = number.ToString("E", provider);
            var result25 = number.ToString("E5", provider);
            var result26 = number.ToString("C", provider);
            var result27 = number.ToString("C2", provider);
        }
    }
}