using System;

namespace Test
{
    internal record ClassRecord3(int X, int Y);

    internal record ClassRecord{caret}4(int X, int Y, int Z) : ClassRecord3(X, Y);
}