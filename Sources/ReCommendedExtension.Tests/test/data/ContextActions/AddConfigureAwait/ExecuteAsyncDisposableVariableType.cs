﻿using System.IO;
using System.Threading.Tasks;

namespace Test
{
    internal class Execute
    {
        async Task AsyncDisposable_Variable()
        {
            aw{caret}ait using (Stream m = new MemoryStream()) { }
        }
    }
}