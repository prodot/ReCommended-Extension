using System;

namespace Test
{
    public class FormatStrings
    {
        public void Byte(byte number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G3}", number);
            var result4 = string.Format("{0:G4}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g3}", number);
            var result8 = string.Format("{0:g4}", number);
        }

        public void Byte(byte? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G3}", number);
            var result4 = string.Format("{0:G4}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g3}", number);
            var result8 = string.Format("{0:g4}", number);
        }

        public void SByte(sbyte number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G3}", number);
            var result4 = string.Format("{0:G4}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g3}", number);
            var result8 = string.Format("{0:g4}", number);
        }

        public void SByte(sbyte? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G3}", number);
            var result4 = string.Format("{0:G4}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g3}", number);
            var result8 = string.Format("{0:g4}", number);
        }

        public void Int16(short number)
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

        public void Int16(short? number)
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

        public void UInt16(ushort number)
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

        public void UInt16(ushort? number)
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

        public void Int32(int number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G10}", number);
            var result4 = string.Format("{0:G11}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g10}", number);
            var result8 = string.Format("{0:g11}", number);
        }

        public void Int32(int? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G10}", number);
            var result4 = string.Format("{0:G11}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g10}", number);
            var result8 = string.Format("{0:g11}", number);
        }

        public void UInt32(uint number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G10}", number);
            var result4 = string.Format("{0:G11}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g10}", number);
            var result8 = string.Format("{0:g11}", number);
        }

        public void UInt32(uint? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G10}", number);
            var result4 = string.Format("{0:G11}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g10}", number);
            var result8 = string.Format("{0:g11}", number);
        }

        public void Int64(long number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G19}", number);
            var result4 = string.Format("{0:G20}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g19}", number);
            var result8 = string.Format("{0:g20}", number);
        }

        public void Int64(long? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G19}", number);
            var result4 = string.Format("{0:G20}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g19}", number);
            var result8 = string.Format("{0:g20}", number);
        }

        public void UInt64(ulong number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G20}", number);
            var result4 = string.Format("{0:G21}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g20}", number);
            var result8 = string.Format("{0:g21}", number);
        }

        public void UInt64(ulong? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G20}", number);
            var result4 = string.Format("{0:G21}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g20}", number);
            var result8 = string.Format("{0:g21}", number);
        }

        public void Int128_(Int128 number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G39}", number);
            var result4 = string.Format("{0:G40}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g39}", number);
            var result8 = string.Format("{0:g40}", number);
        }

        public void Int128_(Int128? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G39}", number);
            var result4 = string.Format("{0:G40}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g39}", number);
            var result8 = string.Format("{0:g40}", number);
        }

        public void UInt128_(UInt128 number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G39}", number);
            var result4 = string.Format("{0:G40}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g39}", number);
            var result8 = string.Format("{0:g40}", number);
        }

        public void UInt128_(UInt128? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:G39}", number);
            var result4 = string.Format("{0:G40}", number);
            var result5 = string.Format("{0:g}", number);
            var result6 = string.Format("{0:g0}", number);
            var result7 = string.Format("{0:g39}", number);
            var result8 = string.Format("{0:g40}", number);
        }

        public void IntPtr(nint number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:g}", number);
            var result4 = string.Format("{0:g0}", number);
        }

        public void IntPtr(nint? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:g}", number);
            var result4 = string.Format("{0:g0}", number);
        }

        public void UIntPtr(nuint number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:g}", number);
            var result4 = string.Format("{0:g0}", number);
        }

        public void UIntPtr(nuint? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
            var result3 = string.Format("{0:g}", number);
            var result4 = string.Format("{0:g0}", number);
        }

        public void Decimal(decimal number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:g}", number);
        }

        public void Decimal(decimal? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:g}", number);
        }

        public void Double(double number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void Double(double? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void Single(float number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void Single(float? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void Half_(Half number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void Half_(Half? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void Enum_(SampleEnum value)
        {
            var result1 = string.Format("{0:G}", value);
            var result2 = string.Format("{0:g}", value);
        }

        public void Enum_(SampleEnum? value)
        {
            var result1 = string.Format("{0:G}", value);
            var result2 = string.Format("{0:g}", value);
        }
    }
}