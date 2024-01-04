﻿using System;
using System.Collections.Generic;

namespace NonTargetTyped
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            var var1 = new List<int>();
            var var2 = new List<int> { a, b, c };
            var var3 = new List<int>(3);
            var var4 = new List<int>(3) { a, b, c };
            var var5 = new List<int>(seq);
            var var6 = new List<int>(seq) { a, b, c };
        }
    }

    public class GenericClass<T>
    {
        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            var var1 = new List<T>();
            var var2 = new List<T> { a, b, c };
            var var3 = new List<T>(8);
            var var4 = new List<T>(8) { a, b, c };
            var var5 = new List<T>(seq);
            var var6 = new List<T>(seq) { a, b, c };
        }
    }
}