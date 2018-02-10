namespace Test
{
    public class NonGenericClass
    {
        int[] field = { };

        void Method()
        {
            var array1 = new int[] { };

            var array2 = new int[0];
        }
    }

    public class GenericClass<T>
    {
        T[] field = { };

        void Method()
        {
            var array1 = new T[] { };

            var array2 = new T[0];
        }
    }
}