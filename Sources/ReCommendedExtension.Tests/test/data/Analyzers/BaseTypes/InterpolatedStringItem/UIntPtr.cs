namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(nuint number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:g}";
            var result4 = $"{number:g0}";
        }

        public void RedundantFormatSpecifier(nuint? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:g}";
            var result4 = $"{number:g0}";
        }

        public void RedundantFormatPrecisionSpecifier(nuint number)
        {
            var result11 = $"{number:E6}";
            var result12 = $"{number:e6}";

            var result21 = $"{number:D0}";
            var result22 = $"{number:D1}";
            var result23 = $"{number:d0}";
            var result24 = $"{number:d1}";

            var result31 = $"{number:B0}";
            var result32 = $"{number:B1}";
            var result33 = $"{number:b0}";
            var result34 = $"{number:b1}";

            var result41 = $"{number:X0}";
            var result42 = $"{number:X1}";
            var result43 = $"{number:x0}";
            var result44 = $"{number:x1}";
        }

        public void RedundantFormatPrecisionSpecifier(nuint? number)
        {
            var result11 = $"{number:E6}";
            var result12 = $"{number:e6}";

            var result21 = $"{number:D0}";
            var result22 = $"{number:D1}";
            var result23 = $"{number:d0}";
            var result24 = $"{number:d1}";

            var result31 = $"{number:B0}";
            var result32 = $"{number:B1}";
            var result33 = $"{number:b0}";
            var result34 = $"{number:b1}";

            var result41 = $"{number:X0}";
            var result42 = $"{number:X1}";
            var result43 = $"{number:x0}";
            var result44 = $"{number:x1}";
        }

        public void SuspiciousFormatSpecifier(nuint number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void SuspiciousFormatSpecifier(nuint? number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void NoDetection(nuint number)
        {
            var result1 = $"{number:E}";
            var result2 = $"{number:E5}";
            var result3 = $"{number:D}";
            var result4 = $"{number:D2}";
            var result5 = $"{number:b8}";
            var result6 = $"{number:x8}";
        }

        public void NoDetection(nuint? number)
        {
            var result1 = $"{number:E}";
            var result2 = $"{number:E5}";
            var result3 = $"{number:D}";
            var result4 = $"{number:D2}";
            var result5 = $"{number:b8}";
            var result6 = $"{number:x8}";
        }
    }
}