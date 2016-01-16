using System;

namespace Test
{
    internal class Availability
    {
        [Flags]
        enum NumberOnByte : byte
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnSbyte : sbyte
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnShort : short
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnUshort : ushort
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnInt
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnUint : uint
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnLong : long
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumberOnUlong : ulong
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
        }

        [Flags]
        enum NumbersWithoutZero
        {
            One = 1,
            Two = 2,
            Four = 4,
        }

        [Flags]
        enum NumbersWithZeroSingleMember
        {
            Zero = 0,
        }

        [Flags]
        enum NumbersWithObsoleteMemberInTheMiddle
        {
            None = 0,
            One = 1,

            [Obsolete]
            Two = 2,

            Four = 4,
        }

        [Flags]
        enum NumbersWithInterruptedMembers
        {
            None = 0,
            One = 1,
            Two = 2,
            Eight = 8,
        }

        [Flags]
        enum NumbersWithNegative
        {
            MinusOne = -1,
            One = 1,
            Two = 2,
            Four = 4,
        }

        enum NonFlagged
        {
            One,
            Two,
            Three,
        }

        void Available(
            NumberOnByte one{on}, 
            NumberOnSbyte two{on}, 
            NumberOnShort three{on}, 
            NumberOnUshort four{on}, 
            NumberOnInt five{on}, 
            NumberOnUint six{on}, 
            NumberOnLong seven{on}, 
            NumberOnUlong eight{on},
            NumbersWithoutZero nine{on}, 
            NumbersWithZeroSingleMember ten{on}) { }

        void NotAvailable(
            NumbersWithObsoleteMemberInTheMiddle one{off}, 
            NumbersWithInterruptedMembers two{off}, 
            NonFlagged three{off}, 
            int four{off},
            NumbersWithNegative five{off}) { }
    }
}