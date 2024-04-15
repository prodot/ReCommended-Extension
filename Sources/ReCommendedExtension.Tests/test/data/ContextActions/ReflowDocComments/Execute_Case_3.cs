using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test;

public static class Execute
{
    /// <remarks>
    /// <c>IEnumerable&lt;int&gt;
    /// .Count()</c>{caret}
    /// </remarks>
    static void Method() { }
}