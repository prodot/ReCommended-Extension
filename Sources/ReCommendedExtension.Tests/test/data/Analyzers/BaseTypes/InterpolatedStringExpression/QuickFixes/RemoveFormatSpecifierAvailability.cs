using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void Byte(byte number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G3}";
            var result4 = $"{number:G4}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g3}";
            var result8 = $"{number:g4}";
        }

        public void Byte(byte? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G3}";
            var result4 = $"{number:G4}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g3}";
            var result8 = $"{number:g4}";
        }

        public void SByte(sbyte number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G3}";
            var result4 = $"{number:G4}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g3}";
            var result8 = $"{number:g4}";
        }

        public void SByte(sbyte? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G3}";
            var result4 = $"{number:G4}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g3}";
            var result8 = $"{number:g4}";
        }

        public void Int16(short number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G5}";
            var result4 = $"{number:G6}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g5}";
            var result8 = $"{number:g6}";
        }

        public void Int16(short? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G5}";
            var result4 = $"{number:G6}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g5}";
            var result8 = $"{number:g6}";
        }

        public void UInt16(ushort number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G5}";
            var result4 = $"{number:G6}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g5}";
            var result8 = $"{number:g6}";
        }

        public void UInt16(ushort? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G5}";
            var result4 = $"{number:G6}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g5}";
            var result8 = $"{number:g6}";
        }

        public void Int32(int number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G10}";
            var result4 = $"{number:G11}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g10}";
            var result8 = $"{number:g11}";
        }

        public void Int32(int? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G10}";
            var result4 = $"{number:G11}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g10}";
            var result8 = $"{number:g11}";
        }

        public void UInt32(uint number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G10}";
            var result4 = $"{number:G11}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g10}";
            var result8 = $"{number:g11}";
        }

        public void UInt32(uint? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G10}";
            var result4 = $"{number:G11}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g10}";
            var result8 = $"{number:g11}";
        }

        public void Int64(long number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G19}";
            var result4 = $"{number:G20}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g19}";
            var result8 = $"{number:g20}";
        }

        public void Int64(long? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G19}";
            var result4 = $"{number:G20}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g19}";
            var result8 = $"{number:g20}";
        }

        public void UInt64(ulong number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G20}";
            var result4 = $"{number:G21}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g20}";
            var result8 = $"{number:g21}";
        }

        public void UInt64(ulong? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G20}";
            var result4 = $"{number:G21}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g20}";
            var result8 = $"{number:g21}";
        }

        public void Int128_(Int128 number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G39}";
            var result4 = $"{number:G40}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g39}";
            var result8 = $"{number:g40}";
        }

        public void Int128_(Int128? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G39}";
            var result4 = $"{number:G40}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g39}";
            var result8 = $"{number:g40}";
        }

        public void UInt128_(UInt128 number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G39}";
            var result4 = $"{number:G40}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g39}";
            var result8 = $"{number:g40}";
        }

        public void UInt128_(UInt128? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:G39}";
            var result4 = $"{number:G40}";
            var result5 = $"{number:g}";
            var result6 = $"{number:g0}";
            var result7 = $"{number:g39}";
            var result8 = $"{number:g40}";
        }

        public void IntPtr(nint number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:g}";
            var result4 = $"{number:g0}";
        }

        public void IntPtr(nint? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:g}";
            var result4 = $"{number:g0}";
        }

        public void UIntPtr(nuint number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:g}";
            var result4 = $"{number:g0}";
        }

        public void UIntPtr(nuint? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
            var result3 = $"{number:g}";
            var result4 = $"{number:g0}";
        }

        public void Decimal(decimal number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:g}";
        }

        public void Decimal(decimal? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:g}";
        }

        public void Double(double number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void Double(double? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void Single(float number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void Single(float? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void Half_(Half number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void Half_(Half? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void Enum_(SampleEnum value)
        {
            var result1 = $"{value:G}";
            var result2 = $"{value:g}";
        }

        public void Enum_(SampleEnum? value)
        {
            var result1 = $"{value:G}";
            var result2 = $"{value:g}";
        }

        public void Guid_(Guid value)
        {
            var result1 = $"{value:D}";
            var result2 = $"{value:d}";
        }

        public void Guid_(Guid? value)
        {
            var result1 = $"{value:D}";
            var result2 = $"{value:d}";
        }

        public void TimeSpan_(TimeSpan value)
        {
            var result1 = $"{value:c}";
            var result2 = $"{value:t}";
            var result3 = $"{value:T}";
        }

        public void TimeSpan_(TimeSpan? value)
        {
            var result1 = $"{value:c}";
            var result2 = $"{value:t}";
            var result3 = $"{value:T}";
        }

        public void DateOnly_(DateOnly value)
        {
            var result = $"{value:d}";
        }

        public void DateOnly_(DateOnly? value)
        {
            var result = $"{value:d}";
        }

        public void TimeOnly_(TimeOnly value)
        {
            var result = $"{value:t}";
        }

        public void TimeOnly_(TimeOnly? value)
        {
            var result = $"{value:t}";
        }
    }
}