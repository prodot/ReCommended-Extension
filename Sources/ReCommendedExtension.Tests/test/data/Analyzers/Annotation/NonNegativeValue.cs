using JetBrains.Annotations;

namespace Test
{
    public class Types
    {
        void NonNumeric([NonNegativeValue] string arg) { }

        void NumericUnsigned([NonNegativeValue] uint a, [NonNegativeValue] ulong b, [NonNegativeValue] ushort c, [NonNegativeValue] byte d) { }

        void NumericSigned([NonNegativeValue] int a, [NonNegativeValue] long b, [NonNegativeValue] short c, [NonNegativeValue] sbyte d) { }

        [NonNegativeValue] const int constInt32 = 1;
        [NonNegativeValue] const long constInt64 = 1;
        [NonNegativeValue] const short constInt16 = 1;
        [NonNegativeValue] const sbyte constSByte = 1;
    }

    public class Elements
    {
        [NonNegativeValue]
        uint Method()
        {
            return 0;
        }

        [NonNegativeValue] uint Property => 3;
        [NonNegativeValue] uint Property2 { get; set; }

        [NonNegativeValue]
        uint field;

        [NonNegativeValue]
        delegate uint Callback();
    }
}