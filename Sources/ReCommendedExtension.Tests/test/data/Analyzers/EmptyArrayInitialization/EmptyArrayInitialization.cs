using System;

namespace Test
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ArrayAttribute : Attribute
    {
        public ArrayAttribute(int[] array = null) => Array = array;

        public int[] Array { get; set; }
    }

    public class NonGenericClass
    {
        int[] field = { };

        int[] field_ = { 1, 2, 3 };

        [Array(new int[] { })]
        void Method()
        {
            var array1 = new int[] { };
            var array1_ = new[] { 1, 2, 3 };

            var array2 = new int[0];
            var array2_ = new int[3];

            var array3 = new int[0x0] { };
            var array3_ = new[3] { 1, 2, 3 };

            int[] array4 = { };
            int[] array4_ = { 1, 2, 3 };

            var array5_ = new int[2, 3];
            var array6_ = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };

            var array7_ = new int[][] { };
            var array8_ = new int[][,] { };

            var array9_ = new int[,][] { };
        }

        [Array(Array = new int[] { })]
        int[] Property { get; } = { };

        int[] Property_ { get; } = { 1, 2, 3 };

        int[] Property2 { get; set; } = { };

        int[] Property2_ { get; set; } = { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        T[] field = { };

        T[] field_ = { new T() };

        void Method()
        {
            var array1 = new T[] { };
            var array1_ = new[] { new T() };

            var array2 = new T[0b0];
            var array2_ = new T[1];

            var array3 = new T[default] { };
            var array3_ = new[1] { new T() };

            T[] array4 = { };
            T[] array4_ = { new T() };
        }

        T[] Property { get; } = { };

        T[] Property_ { get; } = { new T() };

        T[] Property2 { get; set; } = { };

        T[] Property2_ { get; set; } = { new T() };
    }
}