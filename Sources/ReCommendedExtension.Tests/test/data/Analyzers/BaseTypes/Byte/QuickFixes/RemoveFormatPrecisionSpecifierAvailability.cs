using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void ToString(byte number, IFormatProvider provider)
        {
            var result11 = number.ToString("E6");
            var result12 = number.ToString("e6");

            var result21 = number.ToString("D0");
            var result22 = number.ToString("D1");
            var result23 = number.ToString("d0");
            var result24 = number.ToString("d1");

            var result31 = number.ToString("B0");
            var result32 = number.ToString("B1");
            var result33 = number.ToString("b0");
            var result34 = number.ToString("b1");
            var result35 = number.ToString("X0");
            var result36 = number.ToString("X1");
            var result37 = number.ToString("x0");
            var result38 = number.ToString("x1");

            var result41 = number.ToString("E6", provider);
            var result42 = number.ToString("e6", provider);

            var result51 = number.ToString("D0", provider);
            var result52 = number.ToString("D1", provider);
            var result53 = number.ToString("d0", provider);
            var result54 = number.ToString("d1", provider);

            var result61 = number.ToString("B0", provider);
            var result62 = number.ToString("B1", provider);
            var result63 = number.ToString("b0", provider);
            var result64 = number.ToString("b1", provider);
            var result65 = number.ToString("X0", provider);
            var result66 = number.ToString("X1", provider);
            var result67 = number.ToString("x0", provider);
            var result68 = number.ToString("x1", provider);

            const string formatSpecifier = "E6";

            var result71 = number.ToString(formatSpecifier);
            var result72 = number.ToString(formatSpecifier, provider);

            var result81 = number.ToString(@"E6");
            var result82 = number.ToString($@"E6");
            var result83 = number.ToString(@$"E6");
            var result84 = number.ToString("""E06""");
            var result85 = number.ToString($"""E06""");
            var result86 = number.ToString($$"""E06""");
            var result87 = number.ToString("""
                                           E006
                                           """);
        }
    }
}