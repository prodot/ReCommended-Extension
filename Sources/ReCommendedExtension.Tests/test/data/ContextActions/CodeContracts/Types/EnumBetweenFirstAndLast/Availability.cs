using System;

namespace Test
{
    internal class Availability
    {
        enum NumberOnByte : byte
        {
            One,
            Two,
            Three,
        }

        enum NumberOnSbyte : sbyte
        {
            One,
            Two,
            Three,
        }

        enum NumberOnShort : short
        {
            One,
            Two,
            Three,
        }

        enum NumberOnUshort : ushort
        {
            One,
            Two,
            Three,
        }

        enum NumberOnInt
        {
            One,
            Two,
            Three,
        }

        enum NumberOnUint : uint
        {
            One,
            Two,
            Three,
        }

        enum NumberOnLong : long
        {
            One,
            Two,
            Three,
        }

        enum NumberOnUlong : ulong
        {
            One,
            Two,
            Three,
        }

        enum NumberWithObsoleteMemberInTheMiddle
        {
            One,

            [Obsolete]
            Two,

            Three,
        }

        enum NumberWithInterruptedMembers
        {
            One = 1,
            Two = 3,
            Three = 3,
        }

        [Flags]
        enum Flagged
        {
            One = 0,
            Two = 1,
            Three = 2,
        }

        void Available(
            NumberOnByte one{on}, 
            NumberOnSbyte two{on}, 
            NumberOnShort three{on}, 
            NumberOnUshort four{on}, 
            NumberOnInt five{on}, 
            NumberOnUint six{on}, 
            NumberOnLong seven{on}, 
            NumberOnUlong eight{on}) { }

        void NotAvailable(NumberWithObsoleteMemberInTheMiddle one{off}, NumberWithInterruptedMembers two{off}, Flagged three{off}, int four{off}) { }
    }
}