using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class GenericClass<T> where T : new()
    {
        IEnumerable<T> Property7 => new{caret}[] { default, default(T), new() };
    }
}