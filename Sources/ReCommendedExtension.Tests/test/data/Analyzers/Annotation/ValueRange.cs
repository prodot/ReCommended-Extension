using JetBrains.Annotations;

namespace Test
{
    public class Types
    {
        void NonNumeric([ValueRange(1)][ValueRange(1, 2)] string arg) { }

        void FromGreaterThanTo(
            [ValueRange(5, 1)] int a, 
            [ValueRange(5, 1)] uint b, 
            [ValueRange(5, 1)] long c,
            [ValueRange(5, 1)] ulong d, 
            [ValueRange(5, 1)] short e, 
            [ValueRange(5, 1)] ushort f, 
            [ValueRange(5, 1)] byte g,
            [ValueRange(5, 1)] sbyte h) { }

        void FromGreaterThanTo_2(
            [ValueRange(int.MaxValue, int.MinValue)] int a, 
            [ValueRange(uint.MaxValue, uint.MinValue)] uint b, 
            [ValueRange(long.MaxValue, long.MinValue)] long c,
            [ValueRange(ulong.MaxValue, ulong.MinValue)] ulong d, 
            [ValueRange(short.MaxValue, short.MinValue)] short e, 
            [ValueRange(ushort.MaxValue, ushort.MinValue)] ushort f, 
            [ValueRange(byte.MaxValue, byte.MinValue)] byte g,
            [ValueRange(sbyte.MaxValue, sbyte.MinValue)] sbyte h) { }

        void ToLessThanMinValue(
            [ValueRange(-2_147_483_648L - 10, -2_147_483_648L - 1)] int a, 
            [ValueRange(-1 * 3, -1)] uint b, 
            [ValueRange(-10, -1)] ulong d, 
            [ValueRange(-1_000_000, -100_000)] short e, 
            [ValueRange(-10, -1)] ushort f, 
            [ValueRange(-10, -1)] byte g,
            [ValueRange(-200, -150)] sbyte h) { }

        void ToLessThanMinValue_2(
            [ValueRange(-2_147_483_648L - 1)] int a, 
            [ValueRange(-1)] uint b, 
            [ValueRange(-1)] ulong d, 
            [ValueRange(-100_000)] short e, 
            [ValueRange(-1)] ushort f, 
            [ValueRange(-1)] byte g,
            [ValueRange(-150)] sbyte h) { }

        void FromGreaterThanMaxValue(
            [ValueRange(2_147_483_647L + 1, 2_147_483_647L + 10)] int a, 
            [ValueRange(uint.MaxValue + 1L, uint.MaxValue + 10L)] uint b, 
            [ValueRange(long.MaxValue + 1ul, long.MaxValue + 10ul)] long c,
            [ValueRange(100_000, 1_000_000)] short e, 
            [ValueRange(100_000, 1_000_000)] ushort f, 
            [ValueRange(257, 300)] byte g,
            [ValueRange(150, 200)] sbyte h) { }

        void FromGreaterThanMaxValue_2(
            [ValueRange(2_147_483_647L + 1)] int a, 
            [ValueRange(uint.MaxValue + 1L)] uint b, 
            [ValueRange(long.MaxValue + 1ul)] long c,
            [ValueRange(100_000)] short e, 
            [ValueRange(100_000)] ushort f, 
            [ValueRange(257)] byte g,
            [ValueRange(150)] sbyte h) { }

        void FromLessThanMinValue_To(
            [ValueRange(-2_147_483_648L - 1, 1)] int a, 
            [ValueRange(-1, 1)] uint b, 
            [ValueRange(-1, 1)] ulong d, 
            [ValueRange(-100_000, 1)] short e, 
            [ValueRange(-1, 1)] ushort f, 
            [ValueRange(-1, 1)] byte g,
            [ValueRange(-150, 1)] sbyte h) { }

        void From_ToGreaterThanMaxValue(
            [ValueRange(1, 2_147_483_647L + 10)] int a, 
            [ValueRange(1, uint.MaxValue + 10L)] uint b, 
            [ValueRange(1, long.MaxValue + 10ul)] long c,
            [ValueRange(1, 1_000_000)] short e, 
            [ValueRange(1, 1_000_000)] ushort f, 
            [ValueRange(1, 300)] byte g,
            [ValueRange(1, 200)] sbyte h) { }

        void FromLessThanMinValue_ToGreaterThenMaxValue(
            [ValueRange(-2_147_483_648L - 1, 2_147_483_647L + 1)] int a, 
            [ValueRange(-1, uint.MaxValue + 1L)] uint b, 
            [ValueRange(-100_000, 100_000)] short e, 
            [ValueRange(-1, 100_000)] ushort f, 
            [ValueRange(-1, 0xFFFF)] byte g,
            [ValueRange(-200, 150)] sbyte h) { }

        void From_To(
            [ValueRange(1, 5)] int a, 
            [ValueRange(1, 5)] uint b, 
            [ValueRange(1, 5)] long c,
            [ValueRange(1, 5)] ulong d, 
            [ValueRange(1, 5)] short e, 
            [ValueRange(1, 5)] ushort f, 
            [ValueRange(1, 5)] byte g,
            [ValueRange(1, 5)] sbyte h) { }

        void From_To_2(
            [ValueRange(1)] int a, 
            [ValueRange(1)] uint b, 
            [ValueRange(1)] long c,
            [ValueRange(1)] ulong d, 
            [ValueRange(1)] short e, 
            [ValueRange(1)] ushort f, 
            [ValueRange(1)] byte g,
            [ValueRange(1)] sbyte h) { }

        [ValueRange(1, 3)] const int constInt32 = 1;
        [ValueRange(1, 3)] const int constUInt32 = 1;
        [ValueRange(1, 3)] const long constInt64 = 1;
        [ValueRange(1, 3)] const long constUInt64 = 1;
        [ValueRange(1, 3)] const short constInt16 = 1;
        [ValueRange(1, 3)] const short constUInt16 = 1;
        [ValueRange(1, 3)] const sbyte constSByte = 1;
        [ValueRange(1, 3)] const sbyte constByte = 1;
    }

    public class Elements
    {
        [ValueRange(-1, 3)]
        uint Method()
        {
            return 0;
        }

        [ValueRange(-1, 3)] uint Property => 3;
        [ValueRange(-1, 3)] uint Property2 { get; set; }

        [ValueRange(-1, 3)]
        uint field;

        [ValueRange(-1, 3)]
        delegate uint Callback();
    }
}