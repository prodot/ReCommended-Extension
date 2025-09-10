namespace Test
{
    public class NonGenericClass
    {
        int[] field = { };

        void Method()
        {
            var array1 = new int[] { };
        }
    }

    public class GenericClass<T>
    {
        T[] field = { };

        void Method()
        {
            var array1 = new T[] { };
        }
    }
}