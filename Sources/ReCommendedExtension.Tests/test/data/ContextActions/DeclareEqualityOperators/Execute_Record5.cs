using System;

namespace Test
{
    internal record ClassRecord3
    {
        public required int X { get; init; }
        public required int Y { get; init; }
    }

    internal record ClassRecord{caret}4 : ClassRecord3
    {
        public required int Z { get; init; }
    }
}