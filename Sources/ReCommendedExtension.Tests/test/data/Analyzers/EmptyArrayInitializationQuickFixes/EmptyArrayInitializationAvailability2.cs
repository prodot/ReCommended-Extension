namespace Test
{
    public class NonGenericClass
    {
        int[] field = new int[0];

        void Method()
        {
            int[] array1 = new int[default];

            Method2(new int[0]);
        }

        void Method2(int[] array) { }
    }

    public class GenericClass<T>
    {
        T[] field = new T[0];

        void Method()
        {
            T[] array1 = new T[default];

            Method2(new T[0]);
        }

        void Method2(T[] array) { }
    }
}