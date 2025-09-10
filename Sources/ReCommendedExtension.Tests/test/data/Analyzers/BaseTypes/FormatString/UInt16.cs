namespace Test
{
    public class FormatStrings
    {
        public void RedundantFormatSpecifier(ushort number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G5}", number);
            var result4 = string.Format("{0:G6}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g5}", number);
            var result8 = string.Format("{0:g6}", number);
        }

        public void RedundantFormatSpecifier(ushort? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G5}", number);
            var result4 = string.Format("{0:G6}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g5}", number);
            var result8 = string.Format("{0:g6}", number);
        }

        public void RedundantFormatPrecisionSpecifier(ushort number)
        {
            var result11 = string.Format("{0:E6}", number);
            var result12 = string.Format("{0:e6}", number);

            var result21 = string.Format("{0:D0}", number);
            var result22 = string.Format("{0:D1}", number);
            var result23 = string.Format("{0:d0}", number);
            var result24 = string.Format("{0:d1}", number);

            var result31 = string.Format("{0:B0}", number);
            var result32 = string.Format("{0:B1}", number);
            var result33 = string.Format("{0:b0}", number);
            var result34 = string.Format("{0:b1}", number);

            var result41 = string.Format("{0:X0}", number);
            var result42 = string.Format("{0:X1}", number);
            var result43 = string.Format("{0:x0}", number);
            var result44 = string.Format("{0:x1}", number);
        }

        public void RedundantFormatPrecisionSpecifier(ushort? number)
        {
            var result11 = string.Format("{0:E6}", number);
            var result12 = string.Format("{0:e6}", number);

            var result21 = string.Format("{0:D0}", number);
            var result22 = string.Format("{0:D1}", number);
            var result23 = string.Format("{0:d0}", number);
            var result24 = string.Format("{0:d1}", number);

            var result31 = string.Format("{0:B0}", number);
            var result32 = string.Format("{0:B1}", number);
            var result33 = string.Format("{0:b0}", number);
            var result34 = string.Format("{0:b1}", number);

            var result41 = string.Format("{0:X0}", number);
            var result42 = string.Format("{0:X1}", number);
            var result43 = string.Format("{0:x0}", number);
            var result44 = string.Format("{0:x1}", number);
        }

        public void SuspiciousFormatSpecifier(ushort number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void SuspiciousFormatSpecifier(ushort? number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void NoDetection(ushort number)
        {
            var result1 = string.Format("{0:G4}", number);
            var result2 = string.Format("{0:E}", number);
            var result3 = string.Format("{0:E5}", number);
            var result4 = string.Format("{0:D}", number);
            var result5 = string.Format("{0:D2}", number);
            var result6 = string.Format("{0:b8}", number);
            var result7 = string.Format("{0:x8}", number);
        }

        public void NoDetection(ushort? number)
        {
            var result1 = string.Format("{0:G4}", number);
            var result2 = string.Format("{0:E}", number);
            var result3 = string.Format("{0:E5}", number);
            var result4 = string.Format("{0:D}", number);
            var result5 = string.Format("{0:D2}", number);
            var result6 = string.Format("{0:b8}", number);
            var result7 = string.Format("{0:x8}", number);
        }
    }
}