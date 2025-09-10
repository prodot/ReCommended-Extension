using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class Types
    {
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
    }
}